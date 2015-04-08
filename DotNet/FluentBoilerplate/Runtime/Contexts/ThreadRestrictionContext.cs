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
using FluentBoilerplate.Messages.User;
using FluentBoilerplate.Runtime.Extensions;
using FluentBoilerplate.Traits;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FluentBoilerplate.Runtime.Contexts
{
    internal sealed class ThreadRestrictionContext : ImmutableContext,
        ICopyableTrait<ThreadRestrictionContext>
    {
        private readonly IContractBundle contractBundle;
        private readonly Semaphore threadRestrictor;
        private readonly WaitHandle waitHandle;
        private readonly LockTransactionContext transactionContext;

        public ThreadRestrictionContext(IContractBundle contractBundle, IContextBundle bundle):base(bundle)
        {
            this.contractBundle = contractBundle ?? new ContractBundle();
            var threadCountRestriction = this.contractBundle.ThreadCountRestriction;
            this.threadRestrictor = (threadCountRestriction > 0) ? new Semaphore(threadCountRestriction, threadCountRestriction) : null;
            this.waitHandle = this.contractBundle.ThreadWaitHandleSignalRestriction;
            this.transactionContext = new LockTransactionContext(this.contractBundle, bundle);

            if (this.threadRestrictor != null)
                Info("Thread count restriction (count <= {0}) is in effect".WithValues(threadCountRestriction));

            if (this.waitHandle != null)
                Info("Thread wait handle restriction is in effect");            
        }

        public TResult Get<TResult>(Func<TResult> func)
        {
            TResult result = default(TResult);
            Do(() => result = func());
            return result;
        }

        public void Do(Action action)
        {
            //The order of evaluation doesn't matter for thread restrictions, it just changes the number of
            //threads that are potentially waiting on the wait handle or semaphore at one time. Both gates (if present) have to
            //be passed for the caller to do anything so we couldn't get a known performance gain by changing
            //the order one way or the other. The semaphore's release is simpler in code this way, hence the current ordering.

            if (this.waitHandle != null)
            {
                WaitForProvidedWaitHandle();
            }

            if (this.threadRestrictor != null)
            {
                WaitForThreadCountRestrictedSection();
            }            
                        
            try
            {
                this.transactionContext.Do(() => action());
            }
            finally
            {  
                if (this.threadRestrictor != null)
                {
                    LeaveThreadCountRestrictedSection();
                }
            }
        }
        
        private void LeaveThreadCountRestrictedSection()
        {
            Debug("Thread #{0} is leaving the thread count restricted section".WithValues(Thread.CurrentThread.ManagedThreadId));
            this.threadRestrictor.Release();
        }

        private void WaitForProvidedWaitHandle()
        {
            var id = Thread.CurrentThread.ManagedThreadId;

            Debug("Thread #{0} is beginning to wait for the provided WaitHandle".WithValues(id));

            var timeout = this.contractBundle.ThreadWaitHandleSignalRestrictionTimeout;
            var signalled = this.waitHandle.WaitOne(timeout.Milliseconds);
            if (!signalled)
            {
                throw new ContractViolationException(CommonErrors.ThreadTimedOut.WithValues(id, "wait handle restriction"));
            }

            Debug("Thread #{0} ended its wait successfully for the provided WaitHandle".WithValues(id));
        }

        private void WaitForThreadCountRestrictedSection()
        {
            var id = Thread.CurrentThread.ManagedThreadId;

            Debug("Thread #{0} is beginning to wait for the thread count restricted section".WithValues(id));

            var timeout = this.contractBundle.ThreadCountRestrictionTimeout;
            var signalled = this.threadRestrictor.WaitOne(timeout.Milliseconds);
            if (!signalled)
            {
                throw new ContractViolationException(CommonErrors.ThreadTimedOut.WithValues(id, "thread count restriction"));
            }

            Debug("Thread #{0} acquired access to the thread count restricted section".WithValues(id));
        }

        public ThreadRestrictionContext Copy(IContextBundle bundle = null, IContractBundle contractBundle = null)
        {
            return new ThreadRestrictionContext(contractBundle ?? this.contractBundle,
                                                bundle ?? this.bundle);
        }
    }
}
