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

using FluentBoilerplate.Contexts;
using FluentBoilerplate.Traits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBoilerplate.Runtime
{
    internal sealed class AtomicOperationBuilder<TPublicContext, TActualContext> : IAtomicOperationBuilder<TPublicContext>
        where TActualContext :
            ICopyableTrait<TPublicContext>,
            TPublicContext
    {
        private readonly TActualContext context;
        private readonly IContextBundle bundle;
        private readonly IContractBundle contractBundle;

        public AtomicOperationBuilder(TActualContext context, IContextBundle bundle, IContractBundle contractBundle)
        {
            this.context = context;
            this.bundle = bundle;
            this.contractBundle = contractBundle;
        }

        public IAtomicOperationParametersBuilder<TPublicContext> Of<T>(Atomic<T> atomicVariable)
        {
            return new AtomicOperationParametersBuilder<TPublicContext, TActualContext>(this.context, this.bundle, this.contractBundle, atomicVariable);
        }
    }
}
