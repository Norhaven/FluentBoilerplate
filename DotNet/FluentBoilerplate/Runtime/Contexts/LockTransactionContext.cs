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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBoilerplate.Runtime.Contexts
{
    internal sealed class LockTransactionContext
    {
        private readonly IContractBundle contractBundle;
        private readonly ILockTransactionMember[] transactionMembers;
        private readonly LockOrder lockOrder;
        private readonly bool isValidTransaction;

        public LockTransactionContext(IContractBundle contractBundle)
        {
            this.contractBundle = contractBundle ?? new ContractBundle();

            var transactionContractBundle = this.contractBundle as LockTransactionContractBundle;
            this.isValidTransaction = transactionContractBundle != null;
            this.transactionMembers = this.isValidTransaction ? transactionContractBundle.TransactionMembers : new ILockTransactionMember[0];
            this.lockOrder = this.isValidTransaction ? transactionContractBundle.LockOrder : LockOrder.Unknown;
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
            var pendingLocks = OrderLocksBy(this.lockOrder);
            var acquiredLocks = new Stack<ILockTransactionMember>();
            try
            {
                foreach(var pending in pendingLocks)
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
            var pendingLocks = OrderLocksBy(this.lockOrder);

            foreach (var pending in pendingLocks)
                pending.ReleaseLock();
        }

        private IEnumerable<ILockTransactionMember> OrderLocksBy(LockOrder order)
        {
            switch(order)
            {
                case LockOrder.Default: return this.transactionMembers.OrderBy(x => x.Id);
                case LockOrder.HashCode: return this.transactionMembers.OrderBy(x => x.GetHashCode());
                case LockOrder.ParameterOrder: return this.transactionMembers;
                default:
                    throw new ArgumentOutOfRangeException("order", order, "Lock order for transaction is unknown or invalid");
            }
        }
    }
}
