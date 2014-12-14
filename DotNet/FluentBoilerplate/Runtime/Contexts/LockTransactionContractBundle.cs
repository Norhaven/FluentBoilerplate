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
using FluentBoilerplate.Runtime.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FluentBoilerplate.Runtime.Contexts
{
    internal sealed class LockTransactionContractBundle : ContractBundle
    {
        private readonly ILockTransactionMember[] transactionMembers;
        private readonly LockOrder lockOrder;

        public ILockTransactionMember[] TransactionMembers { get { return this.transactionMembers; } }
        public LockOrder LockOrder { get { return this.lockOrder; } }

        public LockTransactionContractBundle(IContractBundle contractBundle, LockOrder lockOrder = LockOrder.Unknown, ILockTransactionMember[] transactionMembers = null)
            :this(contractBundle.Preconditions,
                  contractBundle.PostconditionsOnReturn,
                  contractBundle.PostconditionsOnThrow,
                  contractBundle.InstanceValidations,
                  contractBundle.ThreadCountRestriction,
                  contractBundle.ThreadCountRestrictionTimeout,
                  contractBundle.ThreadWaitHandleSignalRestriction,
                  contractBundle.ThreadWaitHandleSignalRestrictionTimeout,
                  transactionMembers,
                  lockOrder)
        {
                       
        }

        public LockTransactionContractBundle(IImmutableQueue<IContractCondition> preconditions = null,
                                             IImmutableQueue<IContractCondition> postconditionsOnReturn = null,
                                             IImmutableQueue<IContractCondition> postconditionsOnThrow = null,
                                             IImmutableQueue<Action> instanceValidations = null,
                                             int threadCountRestriction = 0,
                                             WaitTimeout? threadCountRestrictionTimeout = null,
                                             WaitHandle threadWaitHandleSignalRestriction = null,
                                             WaitTimeout? threadWaitHandleSignalRestrictionTimeout = null,
                                             ILockTransactionMember[] transactionMembers = null,
                                             LockOrder lockOrder = Runtime.LockOrder.Unknown)
            :base(preconditions,
                  postconditionsOnReturn,
                  postconditionsOnThrow,
                  instanceValidations,
                  threadCountRestriction, 
                  threadCountRestrictionTimeout,
                  threadWaitHandleSignalRestriction,
                  threadWaitHandleSignalRestrictionTimeout)
        {
            this.transactionMembers = transactionMembers.DefaultIfNull();
            this.lockOrder = lockOrder;
        }
    }
}
