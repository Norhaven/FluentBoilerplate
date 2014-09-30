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

using FluentBoilerplate.Messages.Developer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using FluentBoilerplate.Runtime.Extensions;
using System.Collections.Immutable;

namespace FluentBoilerplate.Runtime
{
#if DEBUG
    public
#else
    internal 
#endif 
        sealed class ILWriter
    {
        private readonly ILGenerator il;
        private int stackCount = 0;

        public ILWriter(ILGenerator il)
        {
            this.il = il;
        }

        [Conditional("DEBUG")]
        public void VerifyStack()
        {
            if (this.stackCount == 0)
                return;
            if (this.stackCount > 0)
                throw new ArgumentException(String.Format("Overflow! Stack still has '{0}' items on it", this.stackCount));
            if (this.stackCount < 0)
                throw new ArgumentException(String.Format("Underflow! Stack is missing '{0}' items", this.stackCount));
        }

        public void ReturnNull()
        {
            Emit(OpCodes.Ldnull);
            Emit(OpCodes.Ret);
        }
        public void CatchBlock<T>(Label exceptionBlock, Action blockBody)
        {
            CatchBlock(typeof(T), exceptionBlock, blockBody);
        }
        public void CatchBlock(Type exceptionType, Label exceptionBlock, Action blockBody)
        {
            il.BeginCatchBlock(exceptionType);
            blockBody();
            Emit(OpCodes.Leave, exceptionBlock);
        }

        public void Pop()
        {
            Emit(OpCodes.Pop);
        }

        public Label DefineLabel()
        {
            return this.il.DefineLabel();
        }

        public void TryCatch(Action<Label> tryBody, IEnumerable<Action<Label>> getCatchBlocks)
        {
            var catchBlocks = getCatchBlocks.ToArray();

            if (catchBlocks.Length == 0)
            {  
                var endOfTry = DefineLabel();
                tryBody(endOfTry);
                MarkLabel(endOfTry);
            }
            else
            {
                var endOfBlock = il.BeginExceptionBlock();

                tryBody(endOfBlock);

                Emit(OpCodes.Leave, endOfBlock);

                foreach (var block in catchBlocks)
                {
                    block(endOfBlock);
                }

                il.EndExceptionBlock();
            }
        }

        public void WriteIncrement(LocalBuilder local, int number)
        {
            Emit(OpCodes.Ldloc, local);
            Emit(OpCodes.Ldc_I4, number);
            Emit(OpCodes.Add);
            SetLocal(local);
        }
        public void SetArrayElement<T>(int index, Action loadValue)
        {
            Emit(OpCodes.Ldc_I4, index);
            loadValue();
            Emit(OpCodes.Stelem, typeof(T));
        }
        public void WriteArrayInit<T>(int size)
        {
            Emit(OpCodes.Ldc_I4, size);
            Emit(OpCodes.Newarr, typeof(T));
        }
        public LocalBuilder DeclareLocal<T>()
        {
            return il.DeclareLocal(typeof(T));
        }
        public LocalBuilder DeclareLocal(Type type)
        {
            return il.DeclareLocal(type);
        }
        public void LoadFirstParameter()
        {
            Emit(OpCodes.Ldarg_0);
        }
        public void LoadParameterRange(int inclusiveLowerBounds, int exclusiveUpperBounds)
        {
            for (var i = inclusiveLowerBounds; i < exclusiveUpperBounds; i++)
            {
                Emit(OpCodes.Ldarg, i);
            }
        }

        public void LoadThis()
        {
            Emit(OpCodes.Ldarg_0);
        }
        public void LoadVariable(LocalBuilder localBuilder)
        {
            Emit(OpCodes.Ldloc, localBuilder);
        }
        public void LoadField(FieldInfo field)
        {
            Emit(OpCodes.Ldfld, field);
        }
        public void TypeOf<T>()
        {
            TypeOf(typeof(T));
        }

        public void TypeOf(Type type)
        {
            Emit(OpCodes.Ldtoken, type);
            EmitCall(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle"), null);
        }

        public void New<T>(ConstructorInfo constructor = null, Type[] parameters = null)
        {
            var type = typeof(T);
            var constructorParameters = parameters ?? Type.EmptyTypes;
            var actualConstructor = constructor ?? type.GetConstructor(constructorParameters);

            Debug.Assert(actualConstructor != null, AssertFailures.InstanceShouldNotBeNull.WithValues("constructor"));

            Emit(OpCodes.Newobj, actualConstructor);
        }

        public void Cast(Type sourceType, Type targetType)
        {
            if (sourceType.IsClass && targetType.IsClass)
                Emit(OpCodes.Castclass, targetType);
            else if (sourceType.IsValueType && targetType.IsValueType)
            {
                if (sourceType == typeof(int))
                {
                    if (targetType == typeof(long))
                        Emit(OpCodes.Conv_I8);                   
                    else
                        throw new Exception();
                }
                else
                    throw new Exception();
            }
            else
                throw new Exception();
        }

        public void StaticMethodCall(MethodInfo method, params Type[] genericParameters)
        {
            if (genericParameters != null && genericParameters.Length != 0)
                method = method.MakeGenericMethod(genericParameters);

            Debug.Assert(!method.IsVirtual, AssertFailures.InstanceMethodIsInvalidForLocation);

            EmitCall(OpCodes.Call, method, null);
        }

        public void InstanceMethodCall(MethodInfo method, params Type[] genericParameters)
        {
            if (genericParameters != null && genericParameters.Length != 0)
                method = method.MakeGenericMethod(genericParameters);

            if (method.IsVirtual)
                EmitCall(OpCodes.Callvirt, method, null);
            else
                EmitCall(OpCodes.Call, method, null);
        }

        public void ActionDelegateMethodCall<T>()
        {
            EmitCall(OpCodes.Call, typeof(Action<T>).GetMethod("Invoke"), null);
        }
        public void ActionDelegateMethodCall<T1, T2>()
        {
            EmitCall(OpCodes.Call, typeof(Action<T1, T2>).GetMethod("Invoke"), null);
        }

        public void SetPropertyValue(PropertyInfo property)
        {
            var method = property.GetSetMethod();
            InstanceMethodCall(method);
        }

        public void GetPropertyValue(PropertyInfo property)
        {
            var method = property.GetGetMethod();
            InstanceMethodCall(method);
        }

        public void Return()
        {
            Emit(OpCodes.Ret);
        }

        public Label IsNotNull()
        {
            var exit = il.DefineLabel();
            Emit(OpCodes.Ldnull);
            Emit(OpCodes.Ceq);
            Emit(OpCodes.Brtrue, exit);

            return exit;
        }

        public void IfThenElse(Action condition, Action ifBlock, Action elseBlock)
        {
            var afterChoice = il.DefineLabel();
            var enterElseBlock = il.DefineLabel();

            condition();
            Emit(OpCodes.Brfalse, enterElseBlock);

            ifBlock();
            Emit(OpCodes.Br, afterChoice);

            il.MarkLabel(enterElseBlock);

            elseBlock();

            il.MarkLabel(afterChoice);
        }
        public void SetLocal(LocalBuilder local, Action writeSource = null)
        {
            if (writeSource != null)
                writeSource();
            Emit(OpCodes.Stloc, local);
        }

        public static void ForeachOver<T>(T sequenceLocation, Action<T> foreachBody)
        {
            //TODO: Write this
        }

        private void Emit(OpCode opcode) { StackEval(opcode); il.Emit(opcode); }
        private void Emit(OpCode opcode, LocalBuilder localBuilder) { StackEval(opcode); il.Emit(opcode, localBuilder); }
        private void Emit(OpCode opcode, FieldInfo field) { StackEval(opcode); il.Emit(opcode, field); }
        private void Emit(OpCode opcode, Label label) { StackEval(opcode); il.Emit(opcode, label); }
        private void Emit(OpCode opcode, ConstructorInfo constructor) { StackEval(opcode); il.Emit(opcode, constructor); }
        private void Emit(OpCode opcode, int value) { StackEval(opcode); il.Emit(opcode, value); }
        private void Emit(OpCode opcode, long value) { StackEval(opcode); il.Emit(opcode, value); }
        private void Emit(OpCode opcode, ulong value) { StackEval(opcode); il.Emit(opcode, value); }
        private void Emit(OpCode opcode, string value) { StackEval(opcode); il.Emit(opcode, value); }
        private void Emit(OpCode opcode, Type type) { StackEval(opcode); il.Emit(opcode, type); }
        private void EmitCall(OpCode opcode, MethodInfo method, Type[] optionalParameterTypes)
        {
            var argumentCount = method.GetParameters().Length;
            //this.stackCount -= argumentCount;
            StackEval(opcode, method); 
            il.EmitCall(opcode, method, optionalParameterTypes);
        }

        [Conditional("DEBUG")]
        private void StackEval(OpCode opcode, MethodInfo method)
        {
            var argumentCount = method.GetParameters().Length;
            //this.stackCount -= argumentCount;
            StackEval(opcode);
        }

        [Conditional("DEBUG")]
        private void StackEval(OpCode opcode)
        {
            EvaluateStackBehavior(opcode.StackBehaviourPop);
            EvaluateStackBehavior(opcode.StackBehaviourPush);
        }
        
        private void EvaluateStackBehavior(StackBehaviour stackBehavior)
        {
            return;
            //switch (stackBehavior)
            //{
            //    case StackBehaviour.Pop0: return;
            //    case StackBehaviour.Popi:
            //    case StackBehaviour.Pop1:
            //    case StackBehaviour.Popref:
            //    case StackBehaviour.Varpop:
            //        this.stackCount -= 1; break;
            //    case StackBehaviour.Pop1_pop1: 
            //    case StackBehaviour.Popi_pop1:
            //    case StackBehaviour.Popi_popi:
            //    case StackBehaviour.Popi_popi8:
            //    case StackBehaviour.Popi_popr4:
            //    case StackBehaviour.Popi_popr8:
            //    case StackBehaviour.Popref_pop1:
            //    case StackBehaviour.Popref_popi:                
            //        this.stackCount -= 2; break;
            //    case StackBehaviour.Popref_popi_pop1:
            //    case StackBehaviour.Popref_popi_popi:
            //    case StackBehaviour.Popref_popi_popi8:
            //    case StackBehaviour.Popref_popi_popr4:
            //    case StackBehaviour.Popref_popi_popr8:
            //    case StackBehaviour.Popref_popi_popref:
            //    case StackBehaviour.Popi_popi_popi:
            //        this.stackCount -= 3; break;

            //    case StackBehaviour.Push0: return;
            //    case StackBehaviour.Push1:
            //    case StackBehaviour.Pushi:
            //    case StackBehaviour.Pushi8:
            //    case StackBehaviour.Pushr4:
            //    case StackBehaviour.Pushr8:
            //    case StackBehaviour.Pushref:
            //    case StackBehaviour.Varpush:                    
            //        this.stackCount += 1; break;
            //    case StackBehaviour.Push1_push1:
            //        this.stackCount += 2; break;
            //    default:
            //        throw new ArgumentOutOfRangeException("Received unexpected StackBehavior value");
            //}
        }

        public void GetTypePropertyAttributes<T>()
        {
            //var memoizeType = typeof(CachingExtensions).GetMethod("Memoize", Type.EmptyTypes);
            //var validateCachedProperties = typeof(ILGenerator).GetMethod("ValidateCachedProperties");

            //Debug.Assert(memoizeType != null, AssertFailures.InstanceShouldNotBeNull.WithValues("memoizeType"));
            //Debug.Assert(validateCachedProperties != null, AssertFailures.InstanceShouldNotBeNull.WithValues("validateCachedProperties"));


            //Emit(OpCodes.Ldarg_0); //Pass in the type instance to ValidateCachedProperties
            //il.InstanceMethodCall(memoizeType, typeof(T));

            //il.WriteInstanceMethodCall(validateCachedProperties);

            //var memoizerType = typeof(PropertyAttributeMemoizer);
            //var currentMemoizer = memoizerType.GetProperty("Current", BindingFlags.Static);

            //Debug.Assert(currentMemoizer != null, AssertFailures.InstanceShouldNotBeNull.WithValues("currentMemoizer"));

            //var memoizerLocal = il.DeclareLocal(memoizerType);
            //il.WriteSetLocal(memoizerLocal, () => il.WriteGetPropertyValue(currentMemoizer));  

            //var memoizerContains = memoizerType.GetMethod("Contains");
            //var memoizerAdd = memoizerType.GetMethod("Add");
            //var memoizerGet = memoizerType.GetMethod("Get");

            //Debug.Assert(memoizerContains != null, AssertFailures.InstanceShouldNotBeNull.WithValues("memoizerContains"));
            //Debug.Assert(memoizerAdd != null, AssertFailures.InstanceShouldNotBeNull.WithValues("memoizerAdd"));
            //Debug.Assert(memoizerGet != null, AssertFailures.InstanceShouldNotBeNull.WithValues("memoizerGet"));

            //Emit(OpCodes.Ldloc, memoizerLocal);
            //Emit(OpCodes.Ldarg_0); //Send the parameter as the instance
            //il.WriteIfThenElse(() => il.WriteInstanceMethodCall(memoizerContains),
            //                   () =>
            //                   {
            //                       //The memoizer contains this instance, just get it and return it.
            //                       Emit(OpCodes.Ldloc, memoizerLocal);
            //                       il.WriteInstanceMethodCall(memoizerGet);
            //                       Emit(OpCodes.Ret);
            //                   },
            //                   () =>
            //                   {
            //                       //The memoizer does not contain this instance, we need to pull its attributes and store them
            //                       var getType = typeof(object).GetMethod("GetType");
            //                       var getProperties = typeof(Type).GetMethod("GetProperties");
            //                       var getCustomAttributes = typeof(Type).GetMethod("GetCustomAttributes");
            //                       var getPropertyType = typeof(PropertyInfo).GetProperty("PropertyType");

            //                       Debug.Assert(getType != null, AssertFailures.InstanceShouldNotBeNull.WithValues("getType"));
            //                       Debug.Assert(getProperties != null, AssertFailures.InstanceShouldNotBeNull.WithValues("getProperties"));
            //                       Debug.Assert(getCustomAttributes != null, AssertFailures.InstanceShouldNotBeNull.WithValues("getCustomAttributes"));
            //                       Debug.Assert(getPropertyType != null, AssertFailures.InstanceShouldNotBeNull.WithValues("getPropertyType"));

            //                       var properties = il.DeclareLocal(typeof(PropertyInfo[]));

            //                       Emit(OpCodes.Ldarg_0);
            //                       il.WriteInstanceMethodCall(getType);
            //                       il.WriteInstanceMethodCall(getProperties);
            //                       il.WriteSetLocal(properties);

            //                       var cachedPropertyListType = typeof(List<CachedProperty>);
            //                       var cachedProperties = il.DeclareLocal(cachedPropertyListType);
            //                       il.WriteConstructorCall<List<CachedProperty>>();
            //                       il.WriteSetLocal(cachedProperties);

            //                       var addCachedProperty = cachedPropertyListType.GetMethod("Add");
            //                       var ofType = typeof(Enumerable).GetMethod("OfType");

            //                       Debug.Assert(addCachedProperty != null, AssertFailures.InstanceShouldNotBeNull.WithValues("addCachedProperty"));
            //                       Debug.Assert(ofType != null, AssertFailures.InstanceShouldNotBeNull.WithValues("ofType"));

            //                       Action<LocalBuilder> propertyForeachBody = (property) =>
            //                           {
            //                               var attributes = il.DeclareLocal(typeof(Attribute[]));
            //                               Emit(OpCodes.Ldloc, property);
            //                               il.WriteInstanceMethodCall(getType);
            //                               il.WriteInstanceMethodCall(getCustomAttributes); 
            //                               il.WriteInstanceMethodCall(ofType, )

            //                               il.WriteSetLocal(attributes);

            //                               Action<LocalBuilder> foreachBody = (attribute) =>
            //                               {

            //                               };

            //                               il.WriteForeachOver<LocalBuilder>(attributes, foreachBody);

            //                               var cachedProperty = il.DeclareLocal(typeof(CachedProperty));
            //                               Emit(OpCodes.Ldloc, property);
            //                               Emit(OpCodes.Ldloc, attributes);
            //                               il.WriteConstructorCall<CachedProperty>(new[] { typeof(PropertyInfo), typeof(Attribute[]) });
            //                               il.WriteSetLocal(cachedProperty);

            //                               Emit(OpCodes.Ldloc, cachedProperties);
            //                               il.WriteInstanceMethodCall(addCachedProperty);
            //                           };

            //                        il.WriteForeachOver<LocalBuilder>(properties, propertyForeachBody);
            //                   });
        }

        internal void LoadNull()
        {
            Emit(OpCodes.Ldnull);
        }

        internal Label IfEqualThen()
        {
            var elseLabel = this.il.DefineLabel();

            Emit(OpCodes.Ceq);
            Emit(OpCodes.Brfalse, elseLabel);

            return elseLabel;
        }

        internal void MarkLabel(Label label)
        {
            this.il.MarkLabel(label);
        }

        internal void LoadInt32(int value)
        {
            Emit(OpCodes.Ldc_I4, value);
        }

        internal void LoadUInt64(ulong value)
        {
            Emit(OpCodes.Ldc_I8, value);
        }

        internal void LoadInt64(long value)
        {
            Emit(OpCodes.Ldc_I8, value);
        }

        internal void LoadString(string message)
        {
            Emit(OpCodes.Ldstr, message);
        }

        internal Label IfLessThan()
        {
            var label = this.il.DefineLabel();

            Emit(OpCodes.Clt);
            Emit(OpCodes.Brfalse, label);

            return label;
        }

        internal Label IfGreaterThan()
        {
            var label = this.il.DefineLabel();

            Emit(OpCodes.Cgt);
            Emit(OpCodes.Brfalse, label);

            return label;
        }

        internal Label IfTrueThen()
        {
            var label = this.il.DefineLabel();

            Emit(OpCodes.Brtrue, label);

            return label;
        }

        internal Label IfFalseThen()
        {
            var label = this.il.DefineLabel();

            Emit(OpCodes.Brfalse, label);

            return label;
        }

        internal void LoadFirstInstanceArgument()
        {
            Emit(OpCodes.Ldarg_1);
        }

        internal void LoadSecondParameter()
        {
            Emit(OpCodes.Ldarg_1);
        }

        internal void IsInstanceOf(Type type)
        {
            Emit(OpCodes.Isinst, type);
        }
    }
}
