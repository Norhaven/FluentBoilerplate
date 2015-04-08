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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBoilerplate.Runtime.Contexts
{
#if DEBUG
    public
#else
    internal 
#endif
        abstract class ImmutableContractAwareContext<TContext> : ImmutableContext
    {
        private readonly ThreadRestrictionContext restrictionContext;
        protected readonly IContractBundle contractBundle;
        internal readonly LockTransactionContractBundle transactionContractBundle;

        public ImmutableContractAwareContext(IContractBundle contractBundle, IContextBundle bundle)
            : base(bundle)
        {
            this.contractBundle = contractBundle ?? new ContractBundle();
            this.restrictionContext = new ThreadRestrictionContext(contractBundle, bundle);

            if (this.contractBundle is LockTransactionContractBundle)
            {
                this.transactionContractBundle = (LockTransactionContractBundle)this.contractBundle;
            }
        }             

        public TResult VerifyContractIfPossible<TResult>(IIdentity identity, Func<TResult> action)
        {
            if (!this.bundle.Permissions.HasPermission(identity))
                throw new ContractViolationException("The current identity does not have permission to  perform this action");

            if (this.contractBundle == null)
            {
                Info("No contract is available to verify, performing the action as-is");

                var result = action();

                Debug("Caller's action returned", result);

                return result;
            }
            else
            {
                var contract = new ContractContext<TResult>(this.bundle, this.contractBundle, null);

                Info("Contract is present, verifying preconditions");
                contract.VerifyPreConditions();

                TResult result;
                try
                {
                    //Thread restrictions never apply to contract validations, only to the caller's action
                    result = this.restrictionContext.Get(() =>
                        {
                            return action();
                        });
                }
                catch(ContractViolationException)
                {
                    //Thread restriction contract violations don't trigger any postconditions
                    throw;
                }
                catch
                {
                    Info("An exception was thrown while executing a custom action, verifying thrown exception postconditions");
                    contract.VerifyPostConditions(ContractExit.ThrewException);
                    throw;
                }
                  

                Debug("Caller's action has returned", result);
                Info("Action completed successfully, verifying return postconditions");
                contract.VerifyPostConditions(ContractExit.Returned);

                return result;
            }
        }
    }
}
