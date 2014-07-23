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
            writer.TryCatch(
                endOfTryCatch =>
                {
                    var method = KnownMetadata.Methods.IExceptionAwareAction_Do;
                    writer.LoadFirstParameter();
                    writer.InstanceMethodCall(method);
                },
                WriteUnrolledCatchBlocks(writer, exceptionTypes)
                );

            writer.Return();
        }
        
        private IEnumerable<Action<Label>> WriteUnrolledCatchBlocks(ILWriter writer, IImmutableQueue<Type> exceptionTypes)
        {
            var handleException = KnownMetadata.Methods.IExceptionAwareAction_HandleException;
            
            foreach (var exceptionType in exceptionTypes)
            {
                yield return endOfTryCatch =>
                {
                    var handleSpecificException = handleException.MakeGenericMethod(exceptionType);

                    Action catchBlockBody = () =>
                    {
                        var localException = writer.DeclareLocal(exceptionType);
                        writer.SetLocal(localException);
                        
                        writer.LoadFirstParameter(); //Exception-aware instance
                        writer.LoadVariable(localException);
                        writer.InstanceMethodCall(handleSpecificException);
                    };

                    writer.CatchBlock(exceptionType, endOfTryCatch, catchBlockBody);
                };
            }
        }        
    }
}
