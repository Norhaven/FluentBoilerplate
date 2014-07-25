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

namespace FluentBoilerplate.Runtime.Contexts
{
    internal sealed class BoilerplateContractContext<TResult> :
        IBoilerplateContractContext<TResult>,
        IVerifiableContractContext
    {
        private readonly ContextBundle bundle;
        private readonly IIdentity identity;
        private readonly IContractBundle contractBundle;
        private readonly TResult result;
        
        public BoilerplateContractContext(ContextBundle bundle,
                                          IIdentity identity,
                                          IContractBundle contractBundle,
                                          TResult result)
        {   
            this.bundle = bundle;
            this.identity = identity;
            this.contractBundle = contractBundle;
            this.result = result;
        }

        public IBoilerplateContractContext<TResult> Handles<TException>(string sectionName, Func<TException, TResult> action = null) where TException : Exception
        {
            var elevatedErrorContext = this.bundle.Errors.RegisterExceptionHandler<TException, TResult>(sectionName, action);
            var elevatedSettings = this.bundle.Copy(errorContext: elevatedErrorContext);
            return Copy(bundle: elevatedSettings);
        }

        public IBoilerplateContractContext<TResult> RequiresRights(params IRight[] rights)
        {
            var elevatedPermissions = this.bundle.Permissions.Merge(requiredRights: rights.ToImmutableHashSet());
            var elevatedSettings = this.bundle.Copy(permissionsProvider: elevatedPermissions);
            return Copy(bundle: elevatedSettings);
        }

        public IBoilerplateContractContext<TResult> MustNotHaveRights(params IRight[] rights)
        {
            var elevatedPermissions = this.bundle.Permissions.Merge(restrictedRights: rights.ToImmutableHashSet());
            var elevatedSettings = this.bundle.Copy(permissionsProvider: elevatedPermissions);
            return Copy(bundle: elevatedSettings);
        }

        public IBoilerplateContractContext<TResult> RequiresRoles(params IRole[] roles)
        {
            var elevatedPermissions = this.bundle.Permissions.Merge(requiredRoles: roles.ToImmutableHashSet());
            var elevatedSettings = this.bundle.Copy(permissionsProvider: elevatedPermissions);
            return Copy(bundle: elevatedSettings);
        }

        public IBoilerplateContractContext<TResult> MustNotHaveRoles(params IRole[] roles)
        {
            var elevatedPermissions = this.bundle.Permissions.Merge(restrictedRoles: roles.ToImmutableHashSet());
            var elevatedSettings = this.bundle.Copy(permissionsProvider: elevatedPermissions);
            return Copy(bundle: elevatedSettings);
        }

        public IBoilerplateContext<TResult> EndContract()
        {
            return new BoilerplateContext<TResult>(this.bundle, this.identity, this, this.result);
        }

        public IContractContext<IBoilerplateContext<TResult>, IBoilerplateContext<TResult>> RequiresValidInstanceOf<TType>(params TType[] instances)
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

            var elevatedInstanceValidations = this.instanceValidations.Enqueue(validate);
            return Copy(instanceValidations: elevatedInstanceValidations);
        }

        public IContractContext<IBoilerplateContext<TResult>, IBoilerplateContext<TResult>> Require(Func<bool> condition, string message = null)
        {
            var contractCondition = new DefaultContractCondition(condition, message);
            var elevatedPreconditions = this.preconditions.Enqueue(contractCondition);
            return Copy(preconditions: elevatedPreconditions);
        }

        public IContractContext<IBoilerplateContext<TResult>, IBoilerplateContext<TResult>> Require<TException>(Func<bool> condition, Func<TException> createException = null) where TException : Exception
        {
            var contractCondition = new CustomExceptionContractCondition<TException>(condition, createException);
            var elevatedPreconditions = this.preconditions.Enqueue(contractCondition);
            return Copy(preconditions: elevatedPreconditions);
        }

        public IContractContext<IBoilerplateContext<TResult>, IBoilerplateContext<TResult>> EnsureOnReturn(Func<bool> condition, string message = null)
        {
            var contractCondition = new DefaultContractCondition(condition, message);
            var elevatedPostconditionsOnReturn = this.postconditionsOnReturn.Enqueue(contractCondition);
            return Copy(postconditionsOnReturn: elevatedPostconditionsOnReturn);
        }

        public IContractContext<IBoilerplateContext<TResult>, IBoilerplateContext<TResult>> EnsureOnReturn<TException>(Func<bool> condition, Func<TException> createException = null) where TException : Exception
        {
            var contractCondition = new CustomExceptionContractCondition<TException>(condition, createException);
            var elevatedPostconditionsOnReturn = this.postconditionsOnReturn.Enqueue(contractCondition);
            return Copy(postconditionsOnReturn: elevatedPostconditionsOnReturn);
        }

        public IContractContext<IBoilerplateContext<TResult>, IBoilerplateContext<TResult>> EnsureOnThrow(Func<bool> condition, string message = null)
        {
            var contractCondition = new DefaultContractCondition(condition, message);
            var elevatedPostconditionsOnThrow = this.postconditionsOnThrow.Enqueue(contractCondition);
            return Copy(postconditionsOnThrow: elevatedPostconditionsOnThrow);
        }

        public IContractContext<IBoilerplateContext<TResult>, IBoilerplateContext<TResult>> EnsureOnThrow<TException>(Func<bool> condition, Func<TException> createException = null) where TException : Exception
        {
            var contractCondition = new CustomExceptionContractCondition<TException>(condition, createException);
            var elevatedPostconditionsOnThrow = this.postconditionsOnThrow.Enqueue(contractCondition);
            return Copy(postconditionsOnThrow: elevatedPostconditionsOnThrow);
        }

        public IContractContext<IBoilerplateContext<TResult>, IBoilerplateContext<TResult>> Handles<TException>(string sectionName, Action<TException> action = null) where TException : Exception
        {
            var elevatedErrorContext = this.bundle.Errors.RegisterExceptionHandler<TException>(sectionName, action);
            var elevatedSettings = this.bundle.Copy(errorContext: elevatedErrorContext);
            return Copy(settings: elevatedSettings);
        }

        public void VerifyPreConditions()
        {
            VerifyConditions(this.preconditions);

            foreach (var validate in this.instanceValidations)
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

        private BoilerplateContractContext<TResult> Copy(ContextBundle bundle = null,
                                                         IIdentity account = null,
                                                         IContractBundle contractBundle = null)
        {
            return new BoilerplateContractContext<TResult>(bundle ?? this.bundle,
                                                           account ?? this.identity,
                                                           contractBundle ?? this.contractBundle,
                                                           this.result);
        }
    }
}
