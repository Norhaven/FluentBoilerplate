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
using System.Threading;
using System.Threading.Tasks;

namespace FluentBoilerplate.Runtime
{
    internal sealed class ThreadRestrictionBuilder<TPublicContext, TActualContext> : IThreadRestrictionBuilder<TPublicContext>
        where TActualContext :
            ICopyableTrait<TPublicContext>,
            TPublicContext
    {
        private readonly TActualContext context;
        private readonly IContextBundle bundle;
        private readonly IContractBundle contractBundle;
        
        public IAtomicOperationBuilder<TPublicContext> ByTransaction
        {
            get { return new AtomicOperationBuilder<TPublicContext, TActualContext>(this.context, this.bundle, this.contractBundle); }
        }

        public ThreadRestrictionBuilder(TActualContext context, IContextBundle bundle, IContractBundle contractBundle)
        {
            this.context = context;
            this.bundle = bundle;
            this.contractBundle = contractBundle;
        }

        public TPublicContext ByWaitingFor(WaitHandle handle, WaitTimeout timeout = default(WaitTimeout))
        {
            var elevatedContractBundle = this.contractBundle.AddThreadWaitHandleRestrictionFor(handle, timeout);
            return this.context.Copy(this.bundle, elevatedContractBundle);
        }

        public TPublicContext ToMaxOf(int number, WaitTimeout timeout = default(WaitTimeout))
        {
            var elevatedContractBundle = this.contractBundle.AddThreadCountRestrictionOf(number, timeout);
            return this.context.Copy(this.bundle, elevatedContractBundle);
        }
    }
}
