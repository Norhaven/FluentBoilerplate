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
        private readonly IBoilerplateContractContext<TResult> contractualContext;
        
        public IIdentity Identity { get; private set; }
        public TResult Result { get; private set; }

        internal BoilerplateContext(ContextBundle settings,
                                      IIdentity identity, 
                                      IBoilerplateContractContext<TResult> contractualContext,
                                      TResult result)
            : base(settings, contractualContext as IVerifiableContractContext)
        {
            this.Identity = identity;
            this.contractualContext = contractualContext;
            this.Result = result;
        }
        
        public IBoilerplateContractContext<TResult> BeginContract()
        {
            return new BoilerplateContractContext<TResult>(this.bundle, this.Identity, null, this.Result);
        }

        public IBoilerplateContext<TResult> Get(Func<IBoilerplateContext<TResult>, TResult> action)
        {
            return VerifyContractIfPossible(() =>
                {
                    var safeCall = this.bundle.Errors.ExtendAround(action);
                    var downgradedSettings = DowngradeErrorHandling();
                    var serviceContext = new BoilerplateContext<TResult>(downgradedSettings, this.Identity, this.contractualContext, this.Result);
                    var result = action(serviceContext);
                    return Copy(result: result);
                });
        }

        public IBoilerplateContext<TResult> Open<TType>(Func<IBoilerplateContext<TResult>, TType, TResult> action)
        {
            return VerifyContractIfPossible(() =>
                {
                    var safeCall = this.bundle.Errors.ExtendAround(action);
                    var downgradedSettings = DowngradeErrorHandling();
                    var serviceContext = new BoilerplateContext<TResult>(downgradedSettings, this.Identity, this.contractualContext, this.Result);
                    var response = this.bundle.Access.TryAccess<TType, TResult>(this.Identity, instance => safeCall(serviceContext, instance));

                    if (response.IsSuccess)
                        return Copy(result: response.Content);
                    return this;
                });
        }
        
        public IBoilerplateContext<TResult> Do(Action<IBoilerplateContext<TResult>> action)
        {
            return VerifyContractIfPossible(() =>
            {
                var safeCall = this.bundle.Errors.ExtendAround(action);
                var downgradedSettings = DowngradeErrorHandling();
                var serviceContext = new BoilerplateContext<TResult>(downgradedSettings, this.Identity, this.contractualContext, this.Result);
                safeCall(serviceContext);
                return Copy();
            });
        }

        public IBoilerplateContext<TResult> OpenService<TType>(Action<IBoilerplateContext<TResult>, TType> action)
        {
            return VerifyContractIfPossible(() =>
                {
                    var safeCall = this.bundle.Errors.ExtendAround(action);
                    var downgradedSettings = DowngradeErrorHandling();
                    var serviceContext = new BoilerplateContext<TResult>(downgradedSettings, this.Identity, this.contractualContext, this.Result);
                    var response = this.bundle.Access.TryAccess<TType>(this.Identity, instance => safeCall(serviceContext, instance));
                    
                    if (response.IsSuccess)
                        return Copy();
                    return this;
                });
        }

        public IBoilerplateContext<TResult> Copy(ContextBundle settings)
        {
            return Copy(settings: settings);
        }

        public TTo As<TFrom, TTo>(TFrom instance)
        {
            return this.bundle.Translation.Translate<TFrom, TTo>(instance);
        }        

        public IBoilerplateContext<TResult> Copy(ContextBundle settings = null,
                                      IIdentity account = null,
                                      IBoilerplateContractContext<TResult> contractualContext = null,
                                      TResult result = default(TResult))
        {   
            var resultWasNotSupplied = EqualityComparer<TResult>.Default.Equals(result, default(TResult));
            var actualResult = (resultWasNotSupplied) ? this.Result : result;
            return new BoilerplateContext<TResult>(settings ?? this.bundle,
                                                                  account ?? this.Identity,
                                                                  contractualContext ?? this.contractualContext,
                                                                  actualResult);
        }
    }
}


