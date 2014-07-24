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

namespace FluentBoilerplate.Runtime.Contexts
{
    public class BoilerplateContext : 
        ImmutableContractAwareContext<BoilerplateContext>,
        IBoilerplateContext
    {
        private readonly IBoilerplateContractContext contractualContext;
        public IIdentity Identity { get; private set; }

        internal BoilerplateContext(ContextBundle bundle,
                                    IIdentity identity, 
                                    IBoilerplateContractContext contractualContext)
            :base(bundle, contractualContext as IVerifiableContractContext)
        {
            this.Identity = identity;
            this.contractualContext = contractualContext;
        }
        
        public IBoilerplateContractContext BeginContract()
        {
            return new BoilerplateContractContext(this.bundle);
        }

        public IBoilerplateContext<TResult> Get<TResult>(Func<IBoilerplateContext, TResult> action)
        {
            return VerifyContractIfPossible(() =>
                {
                    var safeCall = this.bundle.Errors.ExtendAround(action);
                    var downgradedSettings = DowngradeErrorHandling();
                    var downgradedContext = Copy(settings:downgradedSettings);
                    var result = action(downgradedContext);
                    
                    //TODO: Upgrade contract context
                    var elevatedContractualContext = 
                        new BoilerplateContractContext<TResult>(this.bundle,
                                                                this.Identity,
                                                                null,
                                                                null,
                                                                null,
                                                                null,
                                                                result);

                    return new BoilerplateContext<TResult>(this.bundle, 
                                                           this.Identity, 
                                                           elevatedContractualContext, 
                                                           result);
                });
        }

        public IBoilerplateContext<TResult> Open<TType, TResult>(Func<IBoilerplateContext, TType, TResult> action)
        {
            return VerifyContractIfPossible(() =>
            {
                var safeCall = this.bundle.Errors.ExtendAround(action);
                var downgradedSettings = DowngradeErrorHandling();
                var downgradedContext = Copy(settings: downgradedSettings);
                var response = this.bundle.Access.TryAccess<TType, TResult>(this.Identity, instance => action(downgradedContext, instance));

                if (!response.IsSuccess)
                    throw new OperationWasNotSuccessfulException(response.Result);
                
                //TODO: Upgrade contract context
                var elevatedContractualContext =
                    new BoilerplateContractContext<TResult>(this.bundle,
                                                            this.Identity,
                                                            null,
                                                            null,
                                                            null,
                                                            null,
                                                            response.Content);

                return new BoilerplateContext<TResult>(this.bundle,
                                                       this.Identity,
                                                       elevatedContractualContext,
                                                       response.Content);               
            });
        }
        
        public IBoilerplateContext Do(Action<IBoilerplateContext> action)
        {
            return VerifyContractIfPossible(() =>
            {
                var safeCall = this.bundle.Errors.ExtendAround(action);
                var downgradedSettings = DowngradeErrorHandling();
                var downgradedContext = Copy(settings: downgradedSettings);
                action(downgradedContext);
                
                return new BoilerplateContext(this.bundle,
                                              this.Identity,
                                              this.contractualContext);
            });
        }

        public IBoilerplateContext Open<TType>(Action<IBoilerplateContext, TType> action)
        {
            return VerifyContractIfPossible(() =>
            {
                var safeCall = this.bundle.Errors.ExtendAround(action);
                var downgradedSettings = DowngradeErrorHandling();
                var downgradedContext = Copy(settings: downgradedSettings);
                var response = this.bundle.Access.TryAccess<TType>(this.Identity, instance => safeCall(downgradedContext, instance));

                if (!response.IsSuccess)
                    throw new OperationWasNotSuccessfulException(response.Result);
                
                return new BoilerplateContext(this.bundle,
                                              this.Identity,
                                              this.contractualContext);               
            });
        }
        
        public IBoilerplateContext Copy(ContextBundle settings)
        {
            return new BoilerplateContext(settings, this.Identity, this.contractualContext);
        }

        public TTo As<TFrom, TTo>(TFrom instance)
        {
            return this.bundle.Translation.Translate<TFrom, TTo>(instance);
        }

        protected IBoilerplateContext Copy(ContextBundle bundle = null,
                                           IIdentity account = null,
                                           IBoilerplateContractContext contractualContext = null)
        {
            return new BoilerplateContext(bundle ?? this.bundle,
                                          account ?? this.Identity,
                                          contractualContext ?? this.contractualContext);
        }
    }
}
