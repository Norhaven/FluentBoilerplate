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

using FluentBoilerplate.Contexts;
using FluentBoilerplate.Traits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBoilerplate.Runtime.Contexts
{
    internal sealed class VoidReturnContractContext:
        ContractContextBase<IVoidReturnContractContext>,
        IVoidReturnContractContext,
        ICopyableTrait<IVoidReturnContractContext>
    {
        private readonly ICopyableTrait<IBoilerplateContext> originalContext;

        public VoidReturnContractContext(IContextBundle bundle,
                                         IContractBundle contractBundle,
                                         ICopyableTrait<IBoilerplateContext> originalContext)
            : base(bundle, contractBundle)
        {   
            this.originalContext = originalContext;
        }

        public override IVoidReturnContractContext Copy(IContextBundle bundle = null, IContractBundle contractBundle = null)
        {
            return new VoidReturnContractContext(bundle ?? this.bundle,
                                                 contractBundle ?? this.contractBundle,
                                                 this.originalContext);
        }

        public IVoidReturnContractContext Handles<TException>(Action<TException> action = null) where TException : Exception
        {
            var elevatedErrorContext = this.bundle.Errors.RegisterExceptionHandler<TException>(action);
            var elevatedBundle = this.bundle.Copy(errorContext: elevatedErrorContext);
            return Copy(bundle: elevatedBundle);
        }
        
        public IBoilerplateContext EndContract()
        {
            return this.originalContext.Copy(this.bundle, this.contractBundle);
        }    
    }
}
