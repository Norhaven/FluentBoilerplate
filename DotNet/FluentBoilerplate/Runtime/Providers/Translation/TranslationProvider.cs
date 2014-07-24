/*
   Copyright 2014 Chris Hannon

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

using FluentBoilerplate.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

using FluentBoilerplate.Runtime.Extensions;
using System.Reflection;

namespace FluentBoilerplate.Runtime.Providers.Translation
{
    internal sealed class TranslationProvider : CacheProvider<TranslationMap, ITranslator>, ITranslationProvider
    {
        public static ITranslationProvider Empty { get { return new TranslationProvider(FunctionGenerator.Default); } }

        private readonly IFunctionGenerator functionGenerator;
        private readonly bool shouldThrowExceptions;

        public TranslationProvider(IFunctionGenerator functionGenerator, bool shouldThrowExceptions = false)
        {
            this.functionGenerator = functionGenerator;
            this.shouldThrowExceptions = shouldThrowExceptions;
        }
        public TTo Translate<TFrom, TTo>(TFrom instance)
        {
            if (typeof(TFrom) == typeof(TTo))
                return (TTo)(object)instance;

            if (instance == null)
            {
                if (typeof(TTo).IsValueType)
                    throw new ArgumentException("Cannot translate a null instance to a value type", "instance");

                return default(TTo);
            }
            
            var map = new TranslationMap(typeof(TFrom), typeof(TTo));
            var translator = this.GetOrAdd(map, _ => CreateTranslator<TFrom, TTo>());
            return (TTo)translator.Translate(instance); //NOTE: Dangerous
        }

        private ITranslator CreateTranslator<TFrom, TTo>()
        {
            var method = this.functionGenerator.Create<TFrom, TTo>(writer => WriteTranslatorBody<TFrom, TTo>(writer));
            return new Translator<TFrom, TTo>(method);
        }

#if DEBUG
        internal
#else
        private 
#endif
            void WriteTranslatorBody<TFrom, TTo>(ILWriter writer)
        {
            var sourceProperties = typeof(TFrom).GetPublicPropertiesWithGetter();
            var targetProperties = typeof(TTo).GetPublicPropertiesWithSetter().ToDictionary(p => p.Name);
            var targetedSourceProperties = sourceProperties.GetTranslatableProperties<TTo>();

            if (targetProperties.Count == 0)
            {
                writer.ReturnNull();
                return;
            }

            var localTargetInstance = writer.DeclareLocal<TTo>();
            writer.New<TTo>();
            writer.SetLocal(localTargetInstance);

            foreach (var sourceProperty in sourceProperties)
            {
                var explicitMapping = GetExplicitMappingIfPresent<TTo>(sourceProperty);

                var targetName = (explicitMapping == null) ? sourceProperty.Name : explicitMapping.PropertyName;

                if (!targetProperties.ContainsKey(targetName))
                {
                    if (this.shouldThrowExceptions)
                        throw new PropertyIsMissingException();
                    continue;
                }

                var targetProperty = targetProperties[targetName];

                var propertiesAreSameType = sourceProperty.PropertyType == targetProperty.PropertyType;

                if (!propertiesAreSameType && !sourceProperty.PropertyType.HasConversionTo(targetProperty.PropertyType))
                {
                    if (this.shouldThrowExceptions)
                        throw new PropertyTypeMismatchException();
                    continue;
                }

                var localSourceValue = writer.DeclareLocal(sourceProperty.PropertyType);

                writer.LoadFirstParameter();
                writer.GetPropertyValue(sourceProperty);
                writer.SetLocal(localSourceValue);

                writer.LoadVariable(localTargetInstance);
                writer.LoadVariable(localSourceValue);

                if (!propertiesAreSameType)
                {
                    writer.Cast(sourceProperty.PropertyType, targetProperty.PropertyType);
                }

                writer.SetPropertyValue(targetProperty);
            }

            writer.LoadVariable(localTargetInstance);
            writer.Return();
            writer.VerifyStack();
        }
        
        
        private static MapsToAttribute GetExplicitMappingIfPresent<TTo>(System.Reflection.PropertyInfo property)
        {            
            var explicitMappings = property.GetAttributesOf<MapsToAttribute>().ToArray();

            if (explicitMappings.Length == 0)
                return null;

            return explicitMappings.FirstOrDefault(x => x.MappedType == typeof(TTo));
        }
    }
}
