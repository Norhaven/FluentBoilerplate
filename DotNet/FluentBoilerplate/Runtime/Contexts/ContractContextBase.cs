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
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBoilerplate.Runtime.Contexts
{
    internal abstract class ContractContextBase<TContext>:
        IContractualTrait<TContext>,
        ICopyableTrait<TContext>
    {
        protected readonly IContextBundle bundle;
        protected readonly IContractBundle contractBundle;

        public ContractContextBase(IContextBundle bundle,
                                   IContractBundle contractBundle)
        {   
            this.bundle = bundle;
            this.contractBundle = contractBundle;
        }

        public abstract TContext Copy(IContextBundle bundle = null,
                                      IContractBundle contractBundle = null);

        public TContext RequireRights(params IRight[] rights)
        {
            var elevatedPermissions = this.bundle.Permissions.Merge(requiredRights: rights.ToImmutableHashSet());
            var elevatedSettings = this.bundle.Copy(permissionsProvider: elevatedPermissions);
            return Copy(bundle: elevatedSettings);
        }

        public TContext MustNotHaveRights(params IRight[] rights)
        {
            var elevatedPermissions = this.bundle.Permissions.Merge(restrictedRights: rights.ToImmutableHashSet());
            var elevatedSettings = this.bundle.Copy(permissionsProvider: elevatedPermissions);
            return Copy(bundle: elevatedSettings);
        }

        public TContext RequireRoles(params IRole[] roles)
        {
            var elevatedPermissions = this.bundle.Permissions.Merge(requiredRoles: roles.ToImmutableHashSet());
            var elevatedSettings = this.bundle.Copy(permissionsProvider: elevatedPermissions);
            return Copy(bundle: elevatedSettings);
        }

        public TContext MustNotHaveRoles(params IRole[] roles)
        {
            var elevatedPermissions = this.bundle.Permissions.Merge(restrictedRoles: roles.ToImmutableHashSet());
            var elevatedSettings = this.bundle.Copy(permissionsProvider: elevatedPermissions);
            return Copy(bundle: elevatedSettings);
        }
        
        public TContext RequireValidInstanceOf<TType>(params TType[] instances)
        {
            var localProvider = this.bundle.Validation;
            Action validate = () =>
            {
                foreach (var instance in instances)
                {
                    var result = localProvider.Validate<TType>(instance);

                    if (result.IsApplicable)
                    {
                        if (!result.IsSuccess)
                            throw new ContractViolationException(result.Message);
                    }
                }
            };

            var elevatedInstanceValidations = this.contractBundle.AddInstanceValidation(validate);
            return Copy(contractBundle: elevatedInstanceValidations);
        }

        public TContext Require(Func<bool> condition, string message = null)
        {
            var contractCondition = new DefaultContractCondition(condition, message);
            var elevatedPreconditions = this.contractBundle.AddPrecondition(contractCondition);
            return Copy(contractBundle: elevatedPreconditions);
        }

        public TContext Require<TException>(Func<bool> condition, Func<TException> createException = null) where TException : Exception
        {
            var contractCondition = new CustomExceptionContractCondition<TException>(condition, createException);
            var elevatedPreconditions = this.contractBundle.AddPrecondition(contractCondition);
            return Copy(contractBundle: elevatedPreconditions);
        }

        public TContext EnsureOnReturn(Func<bool> condition, string message = null)
        {
            var contractCondition = new DefaultContractCondition(condition, message);
            var elevatedPostconditionsOnReturn = this.contractBundle.AddPostconditionOnReturn(contractCondition);
            return Copy(contractBundle: elevatedPostconditionsOnReturn);
        }

        public TContext EnsureOnReturn<TException>(Func<bool> condition, Func<TException> createException = null) where TException : Exception
        {
            var contractCondition = new CustomExceptionContractCondition<TException>(condition, createException);
            var elevatedPostconditionsOnReturn = this.contractBundle.AddPostconditionOnReturn(contractCondition);
            return Copy(contractBundle: elevatedPostconditionsOnReturn);
        }

        public TContext EnsureOnThrow(Func<bool> condition, string message = null)
        {
            var contractCondition = new DefaultContractCondition(condition, message);
            var elevatedPostconditionsOnThrow = this.contractBundle.AddPostconditionOnThrow(contractCondition);
            return Copy(contractBundle: elevatedPostconditionsOnThrow);
        }

        public TContext EnsureOnThrow<TException>(Func<bool> condition, Func<Exception, TException> createException = null) where TException : Exception
        {
            var contractCondition = new CustomExceptionContractCondition<TException>(condition, createException);
            var elevatedPostconditionsOnThrow = this.contractBundle.AddPostconditionOnThrow(contractCondition);
            return Copy(contractBundle: elevatedPostconditionsOnThrow);
        }
        
        public void VerifyPreConditions()
        {
            VerifyConditions(this.contractBundle.Preconditions);

            foreach (var validate in this.contractBundle.InstanceValidations)
                validate();
        }

        public void VerifyPostConditions(ContractExit exit)
        {
            switch (exit)
            {
                case ContractExit.Returned: VerifyConditions(this.contractBundle.PostconditionsOnReturn); break;
                case ContractExit.ThrewException: VerifyConditions(this.contractBundle.PostconditionsOnThrow); break;
                default:
                    throw new ArgumentException("Unknown contract exit condition encountered");
            }
        }

        private void VerifyConditions(IImmutableQueue<IContractCondition> conditions)
        {
            var currentConditions = conditions;

            while (!currentConditions.IsEmpty)
            {
                IContractCondition condition;
                currentConditions = currentConditions.Dequeue(out condition);

                if (!condition.IsConditionMet())
                    condition.Fail();
            }
        }
    }
}
