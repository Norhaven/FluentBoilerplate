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

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentBoilerplate.Runtime.Extensions;
using FluentBoilerplate.Traits;
using FluentBoilerplate.Contexts;

namespace FluentBoilerplate.Runtime.Contexts
{
    internal sealed class ContractContext:
        ContractContextBase<IInitialContractContext>,
        IInitialContractContext,
        IVerifiableContractContext,
        IBundledContractContext,
        ICopyableTrait<IInitialContractContext>                                
    {
        private readonly IElevatableContext originalContext;

        public IContractBundle Bundle { get { return this.contractBundle; } }

        public ContractContext(IContextBundle bundle,
                               IContractBundle contractBundle,
                               IElevatableContext originalContext)
            :base(bundle, contractBundle)
        {
            this.originalContext = originalContext;
        }

        public override IInitialContractContext Copy(IContextBundle bundle = null, IContractBundle contractBundle = null)
        {
            return new ContractContext(bundle ?? this.bundle,
                                       contractBundle ?? this.contractBundle,
                                       this.originalContext);         
        }

        public IResultContractContext<TResult> Handles<TException, TResult>(Func<TException, TResult> action = null) where TException : Exception
        {
            var elevatedErrorContext = this.bundle.Errors.RegisterExceptionHandler<TException, TResult>(action);
            var elevatedBundle = this.bundle.Copy(errorContext: elevatedErrorContext);

            var context = this.originalContext.Elevate<TResult>(elevatedBundle, this.contractBundle);
            return new ContractContext<TResult>(elevatedBundle,
                                                this.contractBundle,
                                                context);
        }

        public IVoidReturnContractContext Handles<TException>(Action<TException> action = null) where TException : Exception
        {
            var elevatedErrorContext = this.bundle.Errors.RegisterExceptionHandler<TException>(action);
            var elevatedBundle = this.bundle.Copy(errorContext: elevatedErrorContext);

            return new VoidReturnContractContext(elevatedBundle,
                                                 this.contractBundle,
                                                 this.originalContext);
        }

        public IBoilerplateContext EndContract()
        {
            return this.originalContext.Copy(this.bundle, this.contractBundle);
        }        
    }    
}
