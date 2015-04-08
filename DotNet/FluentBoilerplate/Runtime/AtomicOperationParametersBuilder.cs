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
using FluentBoilerplate.Runtime.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBoilerplate.Runtime
{
    internal sealed class AtomicOperationParametersBuilder<TPublicContext, TActualContext> : IAtomicOperationParametersBuilder<TPublicContext>
        where TActualContext :
            ICopyableTrait<TPublicContext>,
            TPublicContext
    {
        private readonly TActualContext context;
        private readonly IContextBundle bundle;
        private readonly IContractBundle contractBundle;
        private readonly ILockTransactionMember[] transactionMembers;

        public IAtomicOperationOrderByBuilder<TPublicContext> OrderedBy
        {
            get { return new AtomicOperationOrderByBuilder<TPublicContext, TActualContext>(this.context, this.bundle, this.contractBundle, this.transactionMembers); }
        }

        public AtomicOperationParametersBuilder(TActualContext context, IContextBundle bundle, IContractBundle contractBundle, params ILockTransactionMember[] transactionMembers)
        {
            this.context = context;
            this.bundle = bundle;
            this.contractBundle = contractBundle;
            this.transactionMembers = transactionMembers.DefaultIfNull();
        }

        public IAtomicOperationParametersBuilder<TPublicContext> And<T>(Atomic<T> atomicVariable)
        {
            var elevatedTransactionMembers = this.transactionMembers.Add(atomicVariable);
            return new AtomicOperationParametersBuilder<TPublicContext, TActualContext>(this.context, this.bundle, this.contractBundle, elevatedTransactionMembers);
        }        
    }
}
