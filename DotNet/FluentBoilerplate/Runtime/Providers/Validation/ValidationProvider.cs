/*
   Copyright 2015 Chris Hannon

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

     http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 */

using FluentBoilerplate.Messages.Developer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using FluentBoilerplate.Runtime.Extensions;
using System.Reflection;
using FluentBoilerplate.Messages.User;
using FluentBoilerplate.Providers;

namespace FluentBoilerplate.Runtime.Providers.Validation
{
    internal sealed class ValidationProvider:CacheProvider<Type, IValidator>, IValidationProvider
    {
        public static IValidationProvider Empty { get { return new ValidationProvider(FunctionGenerator.Default); } }

        private readonly IFunctionGenerator functionGenerator;
        private readonly bool shouldThrowExceptions;

        public ValidationProvider(IFunctionGenerator functionGenerator, bool shouldThrowExceptions = false)
        {
            this.functionGenerator = functionGenerator;
            this.shouldThrowExceptions = shouldThrowExceptions;
        }

        public IValidationResult Validate<TType>(TType instance)
        {
            var validator = this.GetOrAdd(typeof(TType), type => CreateValidator<TType>());
            return validator.Validate(instance);
        }

        private IValidator CreateValidator<TType>()
        {
            var method = this.functionGenerator.Create<TType, IValidationResult>(writer => WriteValidatorBody<TType>(writer));
            return new Validator<TType>(method);
        }

#if DEBUG
        internal
#else
        private 
#endif
            void WriteValidatorBody<T>(ILWriter writer)
        {
            if (typeof(T).IsValueType)
            {
                WritePropertyValidation<T>(writer);

                WriteSuccessResult(writer);

                writer.Return();
            }
            else
            {
                writer.LoadFirstParameter(); //Load the parameter we're validating
                var isNull = writer.IsNotNull();

                WritePropertyValidation<T>(writer);

                writer.MarkLabel(isNull); //Ending mark for initial parameter null check

                WriteSuccessResult(writer);

                writer.Return();
            }            
        }

        private void WritePropertyValidation<T>(ILWriter writer)
        {
            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                var validationAttributes = property.GetAttributesOf<IValidationAttribute>();

                foreach (var attribute in validationAttributes)
                {
                    if (attribute.CanBe<NotNullAttribute>())
                        WriteRequireNotNull(writer, property, (NotNullAttribute)attribute);
                    else if (attribute.CanBe<StringLengthAttribute>())
                        WriteStringLength(writer, property, (StringLengthAttribute)attribute);
                    else if (attribute.CanBe<IntegerRangeAttribute>())
                        WriteIntegerRange(writer, property, (IntegerRangeAttribute)attribute);
                    else if (attribute.CanBe<IsMatchForAttribute>())
                        WriteIsMatch(writer, property, (IsMatchForAttribute)attribute);
                    else if (attribute.CanBe<CustomValidationAttribute>())
                        WriteCustomValidation(writer, property, (CustomValidationAttribute)attribute);
                    else
                        Debug.Fail("Encountered unhandled validation attribute");
                }
            }
        }

        private void WriteCustomValidation(ILWriter writer, PropertyInfo property, CustomValidationAttribute customValidationAttribute)
        {
            var validationKind = ValidationKind.Custom;

            var validatorType = typeof(ICustomValidator<>).MakeGenericType(property.PropertyType);

            if (!customValidationAttribute.ValidatorType.CanBe(validatorType))
            {
                var message = CommonErrors.CustomValidatorMustBeOfType.WithValues(property.ReflectedType.Name, property.Name, property.PropertyType.Name);
                throw new IncorrectValidationAttributeException(typeof(CustomValidationAttribute), message);
            }

            var localValidator = writer.DeclareLocal(validatorType);

            writer.New(customValidationAttribute.ValidatorType);
            writer.SetLocal(localValidator);

            var validate = KnownMetadata.Methods.ICustomValidator_OfType(property.PropertyType);
            
            writer.LoadLocal(localValidator);
            writer.LoadFirstParameter();
            writer.GetPropertyValue(property);
            writer.InstanceMethodCall(validate);

            var end = writer.IfFalseThen();

            WriteFailureResult(writer, validationKind, CommonResults.CustomValidationDidNotSucceed.WithValues(property.ReflectedType.Name, property.Name));
            writer.Return();

            writer.MarkLabel(end);
        }

        private void WriteIsMatch(ILWriter writer, PropertyInfo property, IsMatchForAttribute isMatchAttribute)
        {
            var validationKind = ValidationKind.RegularExpressionMatch;
            var local = writer.DeclareLocal<string>();

            if (property.PropertyType == typeof(string))
            {
                writer.LoadFirstParameter();
                writer.GetPropertyValue(property);
                writer.SetLocal(local);
            }
            else
            {
                var temp = writer.DeclareLocal(property.PropertyType);
                writer.LoadFirstParameter();
                writer.GetPropertyValue(property);
                writer.SetLocal(temp);

                writer.LoadLocal(temp);
                var notNull = writer.IsNotNull();

                WriteFailureResult(writer, validationKind, CommonResults.CannotValidateNullProperty.WithValues(property.ReflectedType.Name, property.Name));
                writer.Return();

                writer.MarkLabel(notNull);

                writer.LoadLocal(temp);
                writer.InstanceMethodCall(KnownMetadata.Methods.Object_ToString);
                writer.SetLocal(local);
            }

            writer.LoadLocal(local);
            writer.LoadString(isMatchAttribute.RegularExpression);
            writer.StaticMethodCall(KnownMetadata.Methods.Regex_IsMatch);
            var end = writer.IfFalseThen();

            WriteFailureResult(writer, validationKind, CommonResults.RegularExpressionWasNotMatch.WithValues(property.ReflectedType.Name, property.Name));
            writer.Return();

            writer.MarkLabel(end);
        }

        private void WriteIntegerRange(ILWriter writer, PropertyInfo property, IntegerRangeAttribute integerRangeAttribute)
        {
            var validationKind = ValidationKind.IntegerRange;

            var signedSet = new HashSet<Type>()
            {
                typeof(sbyte), 
                typeof(short), 
                typeof(int), 
                typeof(long)
            };

            var unsignedSet = new HashSet<Type>()
            {
                typeof(byte),
                typeof(ushort),
                typeof(uint),
                typeof(ulong)
            };

            LocalBuilder local;
            
            if (signedSet.Contains(property.PropertyType))
                local = writer.DeclareLocal<long>();
            else if (unsignedSet.Contains(property.PropertyType))
                local = writer.DeclareLocal<ulong>();
            else
            {
                WriteNotApplicableResult(writer, validationKind);
                writer.Return();
                return;
            }
            
            writer.LoadFirstParameter();
            writer.GetPropertyValue(property);
            writer.Cast(property.PropertyType, local.LocalType);
            writer.SetLocal(local);

            if (integerRangeAttribute.HasMinimum && integerRangeAttribute.HasMaximum)
            {
                if (integerRangeAttribute.Minimum > integerRangeAttribute.Maximum)
                {
                    var message = CommonErrors.IntegerRangeValidationHasMinGreaterThanMax.WithValues(property.ReflectedType.Name, property.Name);
                    throw new IncorrectValidationAttributeException(typeof(IntegerRangeAttribute), message);
                }
            }

            if (integerRangeAttribute.HasMinimum)
            {
                writer.LoadLocal(local);
                writer.LoadInt64(integerRangeAttribute.Minimum);

                var isGreaterThanOrEqual = writer.IfLessThan();

                WriteFailureResult(writer, validationKind, CommonResults.IntegerPropertyIsTooLow.WithValues(property.ReflectedType.Name, property.Name, integerRangeAttribute.Minimum));
                writer.Return();

                writer.MarkLabel(isGreaterThanOrEqual);
            }

            if (integerRangeAttribute.HasMaximum)
            {                
                writer.LoadLocal(local);
                writer.LoadInt64(integerRangeAttribute.Maximum);                

                var isLessThanOrEqual = writer.IfGreaterThan();

                WriteFailureResult(writer, validationKind, CommonResults.IntegerPropertyIsTooHigh.WithValues(property.ReflectedType.Name, property.Name, integerRangeAttribute.Maximum));
                writer.Return();

                writer.MarkLabel(isLessThanOrEqual);
            }           
        }
      
        private void WriteStringLength(ILWriter writer, PropertyInfo property, StringLengthAttribute attribute)
        {
            var local = writer.DeclareLocal<string>();

            if (property.PropertyType == typeof(string))
            {
                writer.LoadFirstParameter();
                writer.GetPropertyValue(property);
                writer.SetLocal(local);

                writer.LoadLocal(local);
                writer.LoadNull();
                var notNullBlock = writer.IfEqualThen();

                var isNullMessage = CommonResults.PropertyCannotBeNull.WithValues(property.ReflectedType.Name, property.Name);
                WriteFailureResult(writer, ValidationKind.StringLength, isNullMessage);
                writer.Return();

                writer.MarkLabel(notNullBlock);

                var lengthValue = writer.DeclareLocal<int>();
                var length = KnownMetadata.Properties.String_Length;
                writer.LoadLocal(local);
                writer.GetPropertyValue(length);
                writer.SetLocal(lengthValue);

                var minValue = attribute.Minimum;

                writer.LoadLocal(lengthValue);
                writer.LoadInt32((int)minValue);
                var longerThanMinBlock = writer.IfLessThan();

                var tooShortMessage = CommonResults.StringPropertyIsTooShort.WithValues(property.ReflectedType.Name, property.Name, minValue);
                WriteFailureResult(writer, ValidationKind.StringLength, tooShortMessage);
                writer.Return();

                writer.MarkLabel(longerThanMinBlock);
                                
                var maxValue = attribute.Maximum;

                //The maximum value might not be set, if so then just ignore it
                if (maxValue > 0)
                {
                    writer.LoadLocal(lengthValue);
                    writer.LoadInt32((int)maxValue);
                    var shorterThanMaxBlock = writer.IfGreaterThan();

                    var tooLongMessage = CommonResults.StringPropertyIsTooLong.WithValues(property.ReflectedType.Name, property.Name, maxValue);
                    WriteFailureResult(writer, ValidationKind.StringLength, tooLongMessage);
                    writer.Return();

                    writer.MarkLabel(shorterThanMaxBlock);
                }
            }
            else
            {
                WriteNotApplicableResult(writer, ValidationKind.StringLength);
                writer.Return();
            }
        }

        private void WriteRequireNotNull(ILWriter writer, PropertyInfo property, NotNullAttribute notNullAttribute)
        {
            if (property.PropertyType.IsClass || property.PropertyType.IsInterface)
            {
                writer.LoadFirstParameter();
                writer.GetPropertyValue(property);
                writer.LoadNull();
                var elseLabel = writer.IfEqualThen();

                var message = CommonResults.PropertyCannotBeNull.WithValues(property.ReflectedType.Name, property.Name);
                WriteFailureResult(writer, ValidationKind.RequireNotNull, message);
                writer.Return();

                writer.MarkLabel(elseLabel);
            }
            else
            {
                //Value types, pointers, everything that can't be null is here
                WriteNotApplicableResult(writer, ValidationKind.RequireNotNull); 
                writer.Return();
            }
        }

        private void WriteNotApplicableResult(ILWriter writer, ValidationKind kind)
        {
            var notApplicable = KnownMetadata.Methods.ValidationResult_NotApplicable;
            writer.LoadInt32((int)kind);
            writer.StaticMethodCall(notApplicable);
        }

        private void WriteFailureResult(ILWriter writer, ValidationKind kind, string message)
        {
            var failure = KnownMetadata.Methods.ValidationResult_Failure;
            writer.LoadInt32((int)kind);
            writer.LoadString(message);
            writer.StaticMethodCall(failure);
        }

        private void WriteSuccessResult(ILWriter writer)
        {
            var success = KnownMetadata.Methods.ValidationResult_Success;
            writer.StaticMethodCall(success);
        }
    }
}
