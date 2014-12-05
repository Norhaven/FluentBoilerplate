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
using FluentBoilerplate.Messages.User;
using FluentBoilerplate.Runtime.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FluentBoilerplate.Runtime.Contexts
{
    internal sealed class ThreadRestrictionContext
    {
        private readonly IContractBundle contractBundle;
        private readonly Semaphore threadRestrictor;
        private readonly WaitHandle waitHandle;

        public ThreadRestrictionContext(IContractBundle contractBundle)
        {
            this.contractBundle = contractBundle ?? new ContractBundle();
            var threadCountRestriction = this.contractBundle.ThreadCountRestriction;
            this.threadRestrictor = (threadCountRestriction > 0) ? new Semaphore(0, threadCountRestriction) : null;
            this.waitHandle = this.contractBundle.ThreadWaitHandleSignalRestriction;
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
            //threads that are potentially waiting on the wait handle at one time. Both gates (if present) have to
            //be passed for the caller to do anything so we couldn't get a known performance gain by changing
            //the order one way or the other. The semaphore's release is simpler in code this way.

            if (this.waitHandle != null)
            {
                var timeout = this.contractBundle.ThreadWaitHandleSignalRestrictionTimeout;
                var signalled = this.waitHandle.WaitOne(timeout.Milliseconds);
                if (!signalled)
                {
                    var id = Thread.CurrentThread.ManagedThreadId;
                    throw new ContractViolationException(CommonErrors.ThreadTimedOut.WithValues(id, "wait handle restriction"));
                }
            }

            if (this.threadRestrictor != null)
            {
                var timeout = this.contractBundle.ThreadCountRestrictionTimeout;
                var signalled = this.threadRestrictor.WaitOne(timeout.Milliseconds);
                if (!signalled)
                {
                    var id = Thread.CurrentThread.ManagedThreadId;
                    throw new ContractViolationException(CommonErrors.ThreadTimedOut.WithValues(id, "thread count restriction"));
                }
            }            
            
            try
            {
                action();
            }
            finally
            {
                if (this.threadRestrictor != null)
                    this.threadRestrictor.Release();
            }
        }
    }
}
