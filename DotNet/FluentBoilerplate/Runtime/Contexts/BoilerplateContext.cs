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
        private readonly IBoilerplateContractualContext contractualContext;
        public IIdentity Identity { get; private set; }

        internal BoilerplateContext(ContextSettings settings,
                                      IIdentity identity, 
                                      IBoilerplateContractualContext contractualContext)
            :base(settings, contractualContext as IVerifiableContractContext)
        {
            this.Identity = identity;
            this.contractualContext = contractualContext;
        }
        
        public IBoilerplateContractualContext BeginContract()
        {
            return new BoilerplateContractContext(this.settings);
        }

        public IBoilerplateContext<TResult> Get<TResult>(Func<IBoilerplateContext, TResult> action)
        {
            return VerifyContractIfPossible(() =>
                {
                    var safeCall = this.settings.ErrorContext.ExtendAround(action);
                    var downgradedSettings = DowngradeErrorHandling();
                    var downgradedContext = Copy(settings:downgradedSettings);
                    var result = action(downgradedContext);
                    
                    //TODO: Upgrade contract context
                    var elevatedContractualContext = 
                        new BoilerplateContractContext<TResult>(this.settings,
                                                                                  this.Identity,
                                                                                  null,
                                                                                  null,
                                                                                  null,
                                                                                  null,
                                                                                  result);

                    return new BoilerplateContext<TResult>(this.settings, 
                                                                          this.Identity, 
                                                                          elevatedContractualContext, 
                                                                          result);
                });
        }

        public IBoilerplateContext<TResult> OpenService<TService, TResult>(Func<IBoilerplateContext, TService, TResult> action)
        {
            return VerifyContractIfPossible(() =>
            {
                var safeCall = this.settings.ErrorContext.ExtendAround(action);
                var downgradedSettings = DowngradeErrorHandling();
                var downgradedContext = Copy(settings: downgradedSettings);
                var response = this.settings.ServiceProvider.TryAccess<TService, TResult>(service => action(downgradedContext, service));

                if (!response.IsSuccess)
                    throw new OperationWasNotSuccessfulException(response.Result);
                
                //TODO: Upgrade contract context
                var elevatedContractualContext =
                    new BoilerplateContractContext<TResult>(this.settings,
                                                                                this.Identity,
                                                                                null,
                                                                                null,
                                                                                null,
                                                                                null,
                                                                                response.Content);

                return new BoilerplateContext<TResult>(this.settings,
                                                                        this.Identity,
                                                                        elevatedContractualContext,
                                                                        response.Content);               
            });
        }

        public IBoilerplateContext<TResult> OpenDataAccess<TEntity, TResult>(Func<IBoilerplateContext, TEntity, TResult> action)
        {
            return VerifyContractIfPossible(() =>
            {
                var safeCall = this.settings.ErrorContext.ExtendAround(action);
                var downgradedSettings = DowngradeErrorHandling();
                var downgradedContext = Copy(settings: downgradedSettings);
                var response = this.settings.DataProvider.TryAccess<TEntity, TResult>(service => action(downgradedContext, service));

                if (!response.IsSuccess)
                    throw new OperationWasNotSuccessfulException(response.Result);

                //TODO: Upgrade contract context
                var elevatedContractualContext =
                    new BoilerplateContractContext<TResult>(this.settings,
                                                                                this.Identity,
                                                                                null,
                                                                                null,
                                                                                null,
                                                                                null,
                                                                                response.Content);

                return new BoilerplateContext<TResult>(this.settings,
                                                                        this.Identity,
                                                                        elevatedContractualContext,
                                                                        response.Content);               
            });
        }

        public IBoilerplateContext Do(Action<IBoilerplateContext> action)
        {
            return VerifyContractIfPossible(() =>
            {
                var safeCall = this.settings.ErrorContext.ExtendAround(action);
                var downgradedSettings = DowngradeErrorHandling();
                var downgradedContext = Copy(settings: downgradedSettings);
                action(downgradedContext);
                
                return new BoilerplateContext(this.settings,
                                                             this.Identity,
                                                             this.contractualContext);
            });
        }

        public IBoilerplateContext OpenService<TService>(Action<IBoilerplateContext, TService> action)
        {
            return VerifyContractIfPossible(() =>
            {
                var safeCall = this.settings.ErrorContext.ExtendAround(action);
                var downgradedSettings = DowngradeErrorHandling();
                var downgradedContext = Copy(settings: downgradedSettings);
                var response = this.settings.ServiceProvider.TryAccess<TService>(service => safeCall(downgradedContext, service));

                if (!response.IsSuccess)
                    throw new OperationWasNotSuccessfulException(response.Result);
                
                return new BoilerplateContext(this.settings,
                                                                this.Identity,
                                                                this.contractualContext);               
            });
        }

        public IBoilerplateContext OpenDataAccess<TEntity>(Action<IBoilerplateContext, TEntity> action)
        {
            return VerifyContractIfPossible(() =>
            {
                var safeCall = this.settings.ErrorContext.ExtendAround(action);
                var downgradedSettings = DowngradeErrorHandling();
                var downgradedContext = Copy(settings: downgradedSettings);
                var response = this.settings.DataProvider.TryAccess<TEntity>(service => safeCall(downgradedContext, service));

                if (!response.IsSuccess)
                    throw new OperationWasNotSuccessfulException(response.Result);

                return new BoilerplateContext(this.settings,
                                                             this.Identity,
                                                             this.contractualContext);
            });
        }

        public IBoilerplateContext Copy(ContextSettings settings)
        {
            return new BoilerplateContext(settings, this.Identity, this.contractualContext);
        }

        public TTo As<TFrom, TTo>(TFrom instance)
        {
            return this.settings.TranslationProvider.Translate<TFrom, TTo>(instance);
        }

        protected IBoilerplateContext Copy(ContextSettings settings = null,
                                                                           IIdentity account = null,
                                                                           IBoilerplateContractualContext contractualContext = null)
        {
            return new BoilerplateContext(settings ?? this.settings,
                                                         account ?? this.Identity,
                                                         contractualContext ?? this.contractualContext);
        }
    }
}
