﻿/*
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

namespace FluentBoilerplate.Runtime.Contexts
{
    internal sealed class BoilerplateContractContext<TResult> :
        IBoilerplateContractContext<TResult>,
        IVerifiableContractContext
    {
        private readonly ContextBundle bundle;
        private readonly IIdentity identity;
        private readonly TResult result;
        private readonly IImmutableQueue<ContractCondition> preconditions;
        private readonly IImmutableQueue<ContractCondition> postconditionsOnReturn;
        private readonly IImmutableQueue<ContractCondition> postconditionsOnThrow;
        private readonly IImmutableQueue<Action> instanceValidations;

        public BoilerplateContractContext(ContextBundle bundle,
                                          IIdentity identity,
                                          IImmutableQueue<ContractCondition> preconditions,
                                          IImmutableQueue<ContractCondition> postconditionsOnReturn,
                                          IImmutableQueue<ContractCondition> postconditionsOnThrow,
                                          IImmutableQueue<Action> instanceValidations,
                                          TResult result)
        {
            this.bundle = bundle;
            this.identity = identity;
            this.preconditions = preconditions ?? ImmutableQueue<ContractCondition>.Empty;
            this.postconditionsOnReturn = postconditionsOnReturn ?? ImmutableQueue<ContractCondition>.Empty;
            this.postconditionsOnThrow = postconditionsOnThrow ?? ImmutableQueue<ContractCondition>.Empty;
            this.instanceValidations = instanceValidations ?? ImmutableQueue<Action>.Empty;
            this.result = result;
        }

        public IBoilerplateContractContext<TResult> Handles<TException>(string sectionName, Func<TException, TResult> action = null) where TException : Exception
        {
            var elevatedErrorContext = this.bundle.Errors.RegisterExceptionHandler<TException, TResult>(sectionName, action);
            var elevatedSettings = this.bundle.Copy(errorContext: elevatedErrorContext);
            return Copy(settings: elevatedSettings);
        }

        public IBoilerplateContractContext<TResult> RequiresRights(params IRight[] rights)
        {
            var elevatedPermissions = this.bundle.Permissions.Merge(requiredRights: rights.ToImmutableHashSet());
            var elevatedSettings = this.bundle.Copy(permissionsProvider: elevatedPermissions);
            return Copy(settings: elevatedSettings);
        }

        public IBoilerplateContractContext<TResult> MustNotHaveRights(params IRight[] rights)
        {
            var elevatedPermissions = this.bundle.Permissions.Merge(restrictedRights: rights.ToImmutableHashSet());
            var elevatedSettings = this.bundle.Copy(permissionsProvider: elevatedPermissions);
            return Copy(settings: elevatedSettings);
        }

        public IBoilerplateContractContext<TResult> RequiresRoles(params IRole[] roles)
        {
            var elevatedPermissions = this.bundle.Permissions.Merge(requiredRoles: roles.ToImmutableHashSet());
            var elevatedSettings = this.bundle.Copy(permissionsProvider: elevatedPermissions);
            return Copy(settings: elevatedSettings);
        }

        public IBoilerplateContractContext<TResult> MustNotHaveRoles(params IRole[] roles)
        {
            var elevatedPermissions = this.bundle.Permissions.Merge(restrictedRoles: roles.ToImmutableHashSet());
            var elevatedSettings = this.bundle.Copy(permissionsProvider: elevatedPermissions);
            return Copy(settings: elevatedSettings);
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
                case ContractExit.Returned: VerifyConditions(this.postconditionsOnReturn); break;
                case ContractExit.ThrewException: VerifyConditions(this.postconditionsOnThrow); break;
                default:
                    throw new ArgumentException("Unknown contract exit condition encountered");
            }
        }

        private void VerifyConditions(IImmutableQueue<ContractCondition> conditions)
        {
            var currentConditions = conditions;

            while (!currentConditions.IsEmpty)
            {
                ContractCondition condition;
                currentConditions = currentConditions.Dequeue(out condition);

                if (!condition.IsConditionMet())
                    condition.Fail();
            }
        }

        private BoilerplateContractContext<TResult> Copy(ContextBundle settings = null,
                                                              IIdentity account = null,
                                                              IImmutableQueue<ContractCondition> preconditions = null,
                                                              IImmutableQueue<ContractCondition> postconditionsOnReturn = null,
                                                              IImmutableQueue<ContractCondition> postconditionsOnThrow = null,
                                                              IImmutableQueue<Action> instanceValidations = null)
        {
            return new BoilerplateContractContext<TResult>(settings ?? this.bundle,
                                                                             account ?? this.identity,
                                                                             preconditions ?? this.preconditions,
                                                                             postconditionsOnReturn ?? this.postconditionsOnReturn,
                                                                             postconditionsOnThrow ?? this.postconditionsOnThrow,
                                                                             instanceValidations ?? this.instanceValidations,
                                                                             this.result);
        }

        IContractContext<IBoilerplateContext<TResult>, IBoilerplateContext<TResult>> FluentBoilerplate.Traits.IRightsBasedTrait<IContractContext<IBoilerplateContext<TResult>, IBoilerplateContext<TResult>>>.RequiresRights(params IRight[] rights)
        {
            throw new NotImplementedException();
        }

        IContractContext<IBoilerplateContext<TResult>, IBoilerplateContext<TResult>> FluentBoilerplate.Traits.IRightsBasedTrait<IContractContext<IBoilerplateContext<TResult>, IBoilerplateContext<TResult>>>.MustNotHaveRights(params IRight[] rights)
        {
            throw new NotImplementedException();
        }

        IContractContext<IBoilerplateContext<TResult>, IBoilerplateContext<TResult>> FluentBoilerplate.Traits.IRolesBasedTrait<IContractContext<IBoilerplateContext<TResult>, IBoilerplateContext<TResult>>>.RequiresRoles(params IRole[] roles)
        {
            throw new NotImplementedException();
        }

        IContractContext<IBoilerplateContext<TResult>, IBoilerplateContext<TResult>> FluentBoilerplate.Traits.IRolesBasedTrait<IContractContext<IBoilerplateContext<TResult>, IBoilerplateContext<TResult>>>.MustNotHaveRoles(params IRole[] roles)
        {
            throw new NotImplementedException();
        }
    }
}
