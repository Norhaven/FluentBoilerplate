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
    public class BoilerplateContext<TResult> :
        ImmutableContractAwareContext<BoilerplateContext<TResult>>,
        IBoilerplateContext<TResult>
    {
        private readonly IBoilerplateContractualContext<TResult> contractualContext;
        
        public IIdentity Account { get; private set; }
        public TResult Result { get; private set; }

        internal BoilerplateContext(ContextSettings settings,
                                      IIdentity account, 
                                      IBoilerplateContractualContext<TResult> contractualContext,
                                      TResult result)
            : base(settings, contractualContext as IVerifiableContractContext)
        {
            this.Account = account;
            this.contractualContext = contractualContext;
            this.Result = result;
        }
        
        public IBoilerplateContractualContext<TResult> BeginContract()
        {
            return new BoilerplateContractContext<TResult>(this.settings, this.Account, null, null, null, null, this.Result);
        }

        public IBoilerplateContext<TResult> Get(Func<IBoilerplateContext<TResult>, TResult> action)
        {
            return VerifyContractIfPossible(() =>
                {
                    var safeCall = this.settings.ErrorContext.ExtendAround(action);
                    var downgradedSettings = DowngradeErrorHandling();
                    var serviceContext = new BoilerplateContext<TResult>(downgradedSettings, this.Account, this.contractualContext, this.Result);
                    var result = action(serviceContext);
                    return Copy(result: result);
                });
        }

        public IBoilerplateContext<TResult> OpenService<TService>(Func<IBoilerplateContext<TResult>, TService, TResult> action)
        {
            return VerifyContractIfPossible(() =>
                {
                    var safeCall = this.settings.ErrorContext.ExtendAround(action);
                    var downgradedSettings = DowngradeErrorHandling();
                    var serviceContext = new BoilerplateContext<TResult>(downgradedSettings, this.Account, this.contractualContext, this.Result);
                    var response = this.settings.ServiceProvider.TryAccess<TService, TResult>(service => safeCall(serviceContext, service));

                    if (response.IsSuccess)
                        return Copy(result: response.Content);
                    return this;
                });
        }

        public IBoilerplateContext<TResult> OpenDataAccess<TEntity>(Func<IBoilerplateContext<TResult>, TEntity, TResult> action)
        {
            return VerifyContractIfPossible(() =>
                {
                    var safeCall = this.settings.ErrorContext.ExtendAround(action);
                    var downgradedSettings = DowngradeErrorHandling();
                    var serviceContext = new BoilerplateContext<TResult>(downgradedSettings, this.Account, this.contractualContext, this.Result);
                    var response = this.settings.DataProvider.TryAccess<TEntity, TResult>(service => safeCall(serviceContext, service));

                    if (response.IsSuccess)
                        return Copy(result: response.Content);
                    return this;
                });
        }

        public IBoilerplateContext<TResult> Do(Action<IBoilerplateContext<TResult>> action)
        {
            return VerifyContractIfPossible(() =>
            {
                var safeCall = this.settings.ErrorContext.ExtendAround(action);
                var downgradedSettings = DowngradeErrorHandling();
                var serviceContext = new BoilerplateContext<TResult>(downgradedSettings, this.Account, this.contractualContext, this.Result);
                safeCall(serviceContext);
                return Copy();
            });
        }

        public IBoilerplateContext<TResult> OpenService<TService>(Action<IBoilerplateContext<TResult>, TService> action)
        {
            return VerifyContractIfPossible(() =>
                {
                    var safeCall = this.settings.ErrorContext.ExtendAround(action);
                    var downgradedSettings = DowngradeErrorHandling();
                    var serviceContext = new BoilerplateContext<TResult>(downgradedSettings, this.Account, this.contractualContext, this.Result);
                    var response = this.settings.DataProvider.TryAccess<TService>(service => safeCall(serviceContext, service));
                    
                    if (response.IsSuccess)
                        return Copy();
                    return this;
                });
        }

        public IBoilerplateContext<TResult> OpenDataAccess<TEntity>(Action<IBoilerplateContext<TResult>, TEntity> action)
        {
            return VerifyContractIfPossible(() =>
                {
                    var safeCall = this.settings.ErrorContext.ExtendAround(action);
                    var downgradedSettings = DowngradeErrorHandling();
                    var serviceContext = new BoilerplateContext<TResult>(downgradedSettings, this.Account, this.contractualContext, this.Result);
                    var response = this.settings.DataProvider.TryAccess<TEntity>(service => safeCall(serviceContext, service));
                    if (response.IsSuccess)
                        return Copy();
                    return this;
                });
        }

        public IBoilerplateContext<TResult> Copy(ContextSettings settings)
        {
            return Copy(settings: settings);
        }

        public TTo As<TFrom, TTo>(TFrom instance)
        {
            return this.settings.TranslationProvider.Translate<TFrom, TTo>(instance);
        }        

        public IBoilerplateContext<TResult> Copy(ContextSettings settings = null,
                                      IIdentity account = null,
                                      IBoilerplateContractualContext<TResult> contractualContext = null,
                                      TResult result = default(TResult))
        {   
            var resultWasNotSupplied = EqualityComparer<TResult>.Default.Equals(result, default(TResult));
            var actualResult = (resultWasNotSupplied) ? this.Result : result;
            return new BoilerplateContext<TResult>(settings ?? this.settings,
                                                                  account ?? this.Account,
                                                                  contractualContext ?? this.contractualContext,
                                                                  actualResult);
        }
    }
}


