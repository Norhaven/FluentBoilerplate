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

using FluentBoilerplate;
using FluentBoilerplate.Runtime.Contexts;
using FluentBoilerplate.Runtime.Extensions;
using FluentBoilerplate.Messages;
using FluentBoilerplate.Messages.Developer;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentBoilerplate.Runtime.Providers;
using FluentBoilerplate.Messages.User;
using FluentBoilerplate.Contexts;
using FluentBoilerplate.Traits;

namespace FluentBoilerplate.Runtime.Contexts
{
    public class InitialBoilerplateContext<TContract> : 
        ImmutableContractAwareContext<InitialBoilerplateContext<TContract>>,
        IContext
        where TContract: IInitialContractContext, 
                         IBundledContractContext,                          
                         ICopyableContractTrait<TContract>,
                         new()
    {
        private readonly TContract contractContext;
        public IIdentity Identity { get; private set; }

        internal InitialBoilerplateContext(ContextBundle bundle,
                                           IIdentity identity,
                                           TContract contractContext)
            :base(bundle, contractContext as IVerifiableContractContext)
        {
            this.Identity = identity;
            this.contractContext = contractContext;
        }

        public IInitialContractContext BeginContract()
        {
            return new ContractContext(this.bundle, null);
        }

        public IContext<TResult> Get<TResult>(Func<IContext, TResult> action)
        {
            return VerifyContractIfPossible(() =>
                {
                    var safeCall = this.bundle.Errors.ExtendAround(action);
                    var downgradedSettings = DowngradeErrorHandling();
                    var downgradedContext = Copy(bundle: downgradedSettings);
                    var result = action(downgradedContext);
                                        
                    //TODO: Upgrade contract context
                    var elevatedContractualContext = 
                        new ContractContext<IContext<TResult>, TResult>(this.bundle,
                                                                this.Identity,
                                                                this.contractContext.Bundle,
                                                                null, 
                                                                result);

                    return new BoilerplateContext<TResult>(this.bundle,
                                                           this.Identity,
                                                           null,
                                                           result);;
                });
        }

        public IContext<TResult> Open<TType, TResult>(Func<IContext, TType, TResult> action)
        {
            return VerifyContractIfPossible(() =>
            {
                var safeCall = this.bundle.Errors.ExtendAround(action);
                var downgradedSettings = DowngradeErrorHandling();
                var downgradedContext = Copy(bundle: downgradedSettings);
                var response = this.bundle.Access.TryAccess<TType, TResult>(this.Identity, instance => action(downgradedContext, instance));

                if (!response.IsSuccess)
                    throw new OperationWasNotSuccessfulException(response.Result);
                
                var elevatedContractContext =
                    new ContractContext<IContext<TResult>, TResult>(this.bundle,
                                                            this.Identity,
                                                            this.contractContext.Bundle,
                                                            null,
                                                            response.Content);

                return new BoilerplateContext<TResult>(this.bundle,
                                                       this.Identity,
                                                       elevatedContractContext,
                                                       response.Content);               
            });
        }
        
        public IContext Do(Action<IContext> action)
        {
            return VerifyContractIfPossible(() =>
            {
                var safeCall = this.bundle.Errors.ExtendAround(action);
                var downgradedSettings = DowngradeErrorHandling();
                var downgradedContext = Copy(bundle: downgradedSettings);
                action(downgradedContext);
                
                return new InitialBoilerplateContext<TContract>(this.bundle,
                                                                this.Identity,
                                                                this.contractContext);
            });
        }

        public IContext Open<TType>(Action<IContext, TType> action)
        {
            return VerifyContractIfPossible(() =>
            {
                var safeCall = this.bundle.Errors.ExtendAround(action);
                var downgradedSettings = DowngradeErrorHandling();
                var downgradedContext = Copy(bundle: downgradedSettings);
                var response = this.bundle.Access.TryAccess<TType>(this.Identity, instance => safeCall(downgradedContext, instance));

                if (!response.IsSuccess)
                    throw new OperationWasNotSuccessfulException(response.Result);
                
                return new InitialBoilerplateContext<TContract>(this.bundle,
                                                                this.Identity,
                                                                this.contractContext);               
            });
        }
        
        public IContext Copy(ContextBundle bundle)
        {
            return new InitialBoilerplateContext<TContract>(bundle, 
                                                            this.Identity, 
                                                            this.contractContext);
        }

        public TTo As<TFrom, TTo>(TFrom instance)
        {
            return this.bundle.Translation.Translate<TFrom, TTo>(instance);
        }

        protected IContext Copy(ContextBundle bundle = null,
                                IIdentity account = null,
                                TContract contractContext = default(TContract))
        {
            var isDefault = EqualityComparer<TContract>.Default.Equals(contractContext, default(TContract));
            var actualContractContext = (isDefault) ? this.contractContext : contractContext;

            return new InitialBoilerplateContext<TContract>(bundle ?? this.bundle,
                                                            account ?? this.Identity,
                                                            actualContractContext);
        }

        public IContext Copy(ContextBundle bundle = null, 
                             IContractBundle contractBundle = null)
        {
            var contract = new TContract();
            var fullContract = contract.Copy(bundle ?? this.bundle,
                                             contractBundle ?? this.contractContext.Bundle);

            return new InitialBoilerplateContext<TContract>(bundle ?? this.bundle,
                                                            this.Identity,
                                                            fullContract);
        }
    }
}
