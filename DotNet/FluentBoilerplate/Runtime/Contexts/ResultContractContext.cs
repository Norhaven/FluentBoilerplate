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

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentBoilerplate.Runtime.Extensions;
using FluentBoilerplate.Contexts;
using FluentBoilerplate.Traits;

namespace FluentBoilerplate.Runtime.Contexts
{
    internal sealed class ContractContext<TResult> :
        IResultContractContext<TResult>,
        IVerifiableContractContext
    {
        private readonly IContextBundle bundle;
        private readonly IContractBundle contractBundle;
        private readonly IContext<TResult> originalContext;
        
        public ContractContext(IContextBundle bundle,
                               IContractBundle contractBundle,
                               IContext<TResult> originalContext)
        {   
            this.bundle = bundle;
            this.contractBundle = contractBundle;
            this.originalContext = originalContext;
        }

        public IResultContractContext<TResult> Handles<TException>(Func<TException, TResult> action = null) where TException : Exception
        {
            var elevatedErrorContext = this.bundle.Errors.RegisterExceptionHandler<TException, TResult>(action);
            var elevatedSettings = this.bundle.Copy(errorContext: elevatedErrorContext);
            return Copy(bundle: elevatedSettings);
        }

        public IResultContractContext<TResult> RequireRights(params IRight[] rights)
        {
            var elevatedPermissions = this.bundle.Permissions.Merge(requiredRights: rights.ToImmutableHashSet());
            var elevatedSettings = this.bundle.Copy(permissionsProvider: elevatedPermissions);
            return Copy(bundle: elevatedSettings);
        }

        public IResultContractContext<TResult> MustNotHaveRights(params IRight[] rights)
        {
            var elevatedPermissions = this.bundle.Permissions.Merge(restrictedRights: rights.ToImmutableHashSet());
            var elevatedSettings = this.bundle.Copy(permissionsProvider: elevatedPermissions);
            return Copy(bundle: elevatedSettings);
        }

        public IResultContractContext<TResult> RequireRoles(params IRole[] roles)
        {
            var elevatedPermissions = this.bundle.Permissions.Merge(requiredRoles: roles.ToImmutableHashSet());
            var elevatedSettings = this.bundle.Copy(permissionsProvider: elevatedPermissions);
            return Copy(bundle: elevatedSettings);
        }

        public IResultContractContext<TResult> MustNotHaveRoles(params IRole[] roles)
        {
            var elevatedPermissions = this.bundle.Permissions.Merge(restrictedRoles: roles.ToImmutableHashSet());
            var elevatedSettings = this.bundle.Copy(permissionsProvider: elevatedPermissions);
            return Copy(bundle: elevatedSettings);
        }

        public IContext<TResult> EndContract()
        {
            return this.originalContext.Copy(this.bundle, this.contractBundle);
        }

        public IResultContractContext<TResult> RequireValidInstanceOf<TType>(params TType[] instances)
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

        public IResultContractContext<TResult> Require(Func<bool> condition, string message = null)
        {
            var contractCondition = new DefaultContractCondition(condition, message);
            var elevatedPreconditions = this.contractBundle.AddPrecondition(contractCondition);
            return Copy(contractBundle: elevatedPreconditions);
        }

        public IResultContractContext<TResult> Require<TException>(Func<bool> condition, Func<TException> createException = null) where TException : Exception
        {
            var contractCondition = new CustomExceptionContractCondition<TException>(condition, createException);
            var elevatedPreconditions = this.contractBundle.AddPrecondition(contractCondition);
            return Copy(contractBundle: elevatedPreconditions);
        }

        public IResultContractContext<TResult> EnsureOnReturn(Func<bool> condition, string message = null)
        {
            var contractCondition = new DefaultContractCondition(condition, message);
            var elevatedPostconditionsOnReturn = this.contractBundle.AddPostconditionOnReturn(contractCondition);
            return Copy(contractBundle: elevatedPostconditionsOnReturn);
        }

        public IResultContractContext<TResult> EnsureOnReturn<TException>(Func<bool> condition, Func<TException> createException = null) where TException : Exception
        {
            var contractCondition = new CustomExceptionContractCondition<TException>(condition, createException);
            var elevatedPostconditionsOnReturn = this.contractBundle.AddPostconditionOnReturn(contractCondition);
            return Copy(contractBundle: elevatedPostconditionsOnReturn);
        }

        public IResultContractContext<TResult> EnsureOnThrow(Func<bool> condition, string message = null)
        {
            var contractCondition = new DefaultContractCondition(condition, message);
            var elevatedPostconditionsOnThrow = this.contractBundle.AddPostconditionOnThrow(contractCondition);
            return Copy(contractBundle: elevatedPostconditionsOnThrow);
        }

        public IResultContractContext<TResult> EnsureOnThrow<TException>(Func<bool> condition, Func<Exception, TException> createException = null) where TException : Exception
        {
            var contractCondition = new CustomExceptionContractCondition<TException>(condition, createException);
            var elevatedPostconditionsOnThrow = this.contractBundle.AddPostconditionOnThrow(contractCondition);
            return Copy(contractBundle: elevatedPostconditionsOnThrow);
        }

        public IResultContractContext<TResult> Handles<TException>(Action<TException> action = null) where TException : Exception
        {
            var elevatedErrorContext = this.bundle.Errors.RegisterExceptionHandler<TException>(action);
            var elevatedSettings = this.bundle.Copy(errorContext: elevatedErrorContext);
            return Copy(bundle: elevatedSettings);
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

        private IResultContractContext<TResult> Copy(IContextBundle bundle = null,
                                                     IContractBundle contractBundle = null)
        {
            return new ContractContext<TResult>(bundle ?? this.bundle,
                                                contractBundle ?? this.contractBundle,
                                                this.originalContext);
        }
    }
}
