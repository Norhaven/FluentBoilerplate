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

using FluentBoilerplate.Providers;
using FluentBoilerplate.Runtime;
using FluentBoilerplate.Runtime.Providers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace FluentBoilerplate.Runtime.Providers.ErrorHandling
{   
    internal sealed class TryCatchBlockProvider : CacheProvider<IImmutableQueue<Type>, Action<IExceptionAwareAction>>, ITryCatchBlockProvider
    {
        public static ITryCatchBlockProvider Empty { get { return new TryCatchBlockProvider(FunctionGenerator.Default); } }

        private readonly IFunctionGenerator functionGenerator;
        
        public TryCatchBlockProvider(IFunctionGenerator functionGenerator)
        {
            this.functionGenerator = functionGenerator;
        }

        public ITryCatchBlock GetTryCatchFor(IExceptionHandlerProvider provider)
        {
            var tryCatch = GetOrAdd(provider.HandledTypesInCatchOrder, set => CreateTryCatchBlockFor(set));
            return new TryCatchBlock(provider.HandledExceptionTypes, tryCatch, provider);
        }

        private Action<IExceptionAwareAction> CreateTryCatchBlockFor(IImmutableQueue<Type> exceptionTypes)
        {
            return this.functionGenerator.CreateAction<IExceptionAwareAction>(writer => WriteTryCatchBlockBody(writer, exceptionTypes));            
        }

        private void WriteTryCatchBlockBody(ILWriter writer, IImmutableQueue<Type> exceptionTypes)
        {
            var currentExecutionAttempt = writer.DeclareLocal<int>();
            var shouldRetry = writer.DeclareLocal<bool>();

            var retryLabel = writer.DefineLabel();
            writer.MarkLabel(retryLabel);

            writer.WriteIncrement(currentExecutionAttempt, 1);

            writer.TryCatch(
                endOfTryCatch =>
                {
                    writer.LoadFalse();
                    writer.SetLocal(shouldRetry);

                    var method = KnownMetadata.Methods.IExceptionAwareAction_Do;
                    writer.LoadFirstParameter();
                    writer.InstanceMethodCall(method);
                },
                WriteUnrolledCatchBlocks(writer, exceptionTypes, shouldRetry, currentExecutionAttempt)
                );
                        
            writer.LoadLocal(shouldRetry);
            var shouldNotRetry = writer.IfTrueThen();

            writer.GoTo(retryLabel);

            writer.MarkLabel(shouldNotRetry);
            
            writer.Return();
        }
        
        private IEnumerable<Action<Label>> WriteUnrolledCatchBlocks(ILWriter writer, IImmutableQueue<Type> exceptionTypes, LocalBuilder shouldRetry, LocalBuilder currentExecutionAttempt)
        {
            var handleException = KnownMetadata.Methods.IExceptionAwareAction_HandleException;
            
            foreach (var exceptionType in exceptionTypes)
            {
                yield return endOfTryCatch =>
                {
                    var handleSpecificException = handleException.MakeGenericMethod(exceptionType);

                    Action catchBlockBody = () =>
                    {
                        var endOfCatch = writer.DefineLabel();
                        var localException = writer.DeclareLocal(exceptionType);
                        writer.SetLocal(localException);
                        
                        writer.LoadFirstParameter(); //Exception-aware instance
                        writer.LoadLocal(localException);
                        writer.LoadLocal(currentExecutionAttempt);
                        writer.InstanceMethodCall(handleSpecificException);

                        //The handler refusing to handle it indicates that we should perform a retry.
                        var wasHandled = writer.IfFalseThen();

                        writer.LoadTrue();
                        writer.SetLocal(shouldRetry);
                        writer.GoTo(endOfCatch);

                        writer.MarkLabel(wasHandled);

                        writer.LoadFalse();
                        writer.SetLocal(shouldRetry);

                        writer.MarkLabel(endOfCatch);
                    };

                    writer.CatchBlock(exceptionType, endOfTryCatch, catchBlockBody);
                };
            }
        }        
    }
}
