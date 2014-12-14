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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBoilerplate.Runtime.Contexts
{
    internal sealed class LockTransactionContext:ImmutableContext
    {
        private readonly IContractBundle contractBundle;
        private readonly ILockTransactionMember[] transactionMembers;
        private readonly LockOrder lockOrder;
        private readonly bool isValidTransaction;

        public LockTransactionContext(IContractBundle contractBundle, IContextBundle contextBundle):base(contextBundle)
        {
            this.contractBundle = contractBundle ?? new ContractBundle();

            var transactionContractBundle = this.contractBundle as LockTransactionContractBundle;
            this.isValidTransaction = transactionContractBundle != null;
            this.transactionMembers = this.isValidTransaction ? transactionContractBundle.TransactionMembers : new ILockTransactionMember[0];
            this.lockOrder = this.isValidTransaction ? transactionContractBundle.LockOrder : LockOrder.Unknown;

            if (this.isValidTransaction)
            {
                Info("Atomic transaction operation is in effect with {0} atomic members".WithValues(this.transactionMembers.Length));
                this.transactionMembers = OrderLocksBy(this.transactionMembers, this.lockOrder).ToArray();
            }
        }

        public void Do(Action action)
        {
            if (this.isValidTransaction)
            {                
                try
                {
                    AcquireLocksInOrder();
                    action();
                }
                finally
                {
                    ReleaseLocksInOrder();
                }
            }
            else
            {
                action();
            }
        }

        private void AcquireLocksInOrder()
        {
            var acquiredLocks = new Stack<ILockTransactionMember>();
            try
            {
                foreach(var pending in this.transactionMembers)
                {
                    pending.AcquireLock();
                    acquiredLocks.Push(pending);
                }
            }
            catch
            {
                while(acquiredLocks.Count > 0)
                {
                    var rollbackLock = acquiredLocks.Pop();
                    rollbackLock.ReleaseLock();
                }
            }
        }

        private void ReleaseLocksInOrder()
        {
            foreach (var pending in this.transactionMembers)
                pending.ReleaseLock();
        }

        private static IEnumerable<ILockTransactionMember> OrderLocksBy(ILockTransactionMember[] members, LockOrder order)
        {
            switch(order)
            {
                case LockOrder.Default: return members.OrderBy(x => x.Id);
                case LockOrder.ParameterOrder: return members;
                default:
                    throw new ArgumentOutOfRangeException("order", order, "Lock order for transaction is unknown or invalid");
            }
        }
    }
}
