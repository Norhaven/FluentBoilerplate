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

namespace FluentBoilerplate.Runtime.Contexts
{
    public class ResultBoilerplateContext<TResult> :
        ImmutableContractAwareContext<ResultBoilerplateContext<TResult>>,
        IContext<TResult>
    {
        private readonly IContractBundle contractBundle;
        
        public IIdentity Identity { get; private set; }
        public TResult Result { get; private set; }

        internal ResultBoilerplateContext(ContextBundle bundle,
                                          IIdentity identity, 
                                          IContractBundle contractBundle,
                                          TResult result)
            : base(bundle)
        {
            this.Identity = identity;
            this.contractBundle = contractBundle;
            this.Result = result;
        }

        public IResultContractContext<TResult> BeginContract()
        {
            return new ContractContext<TResult>(this.bundle, null, this);
        }

        public IContext<TResult> Get(Func<IContext, TResult> action)
        {
            return VerifyContractIfPossible(this.contractBundle,
                () =>
                {
                    var safeCall = this.bundle.Errors.ExtendAround(action);
                    var downgradedSettings = DowngradeErrorHandling();
                    var serviceContext = new InitialBoilerplateContext<ContractContext>(downgradedSettings, this.Identity, null);
                    var result = action(serviceContext);
                    return MergeCopy(result: result);
                });
        }

        public IContext<TResult> Get(Func<IContext, TResult, TResult> action)
        {
            return VerifyContractIfPossible(this.contractBundle,
                () =>
            {
                var safeCall = this.bundle.Errors.ExtendAround(action);
                var downgradedSettings = DowngradeErrorHandling();
                var serviceContext = new InitialBoilerplateContext<ContractContext>(downgradedSettings, this.Identity, null);
                var result = action(serviceContext, this.Result);
                return MergeCopy(result: result);
            });
        }

        public IContext<TResult> Open<TType>(Func<IContext<TResult>, TType, TResult> action)
        {
            return VerifyContractIfPossible(this.contractBundle,
                () =>
                {
                    var safeCall = this.bundle.Errors.ExtendAround(action);
                    var downgradedSettings = DowngradeErrorHandling();
                    var serviceContext = new ResultBoilerplateContext<TResult>(downgradedSettings, this.Identity, this.contractBundle, this.Result);
                    var response = this.bundle.Access.TryAccess<TType, TResult>(this.Identity, instance => safeCall(serviceContext, instance));

                    if (response.IsSuccess)
                        return MergeCopy(result: response.Content);
                    return this;
                });
        }
        
        public IContext<TResult> Do(Action<IContext> action)
        {
            return VerifyContractIfPossible(this.contractBundle,
                () =>
            {
                var safeCall = this.bundle.Errors.ExtendAround(action);
                var downgradedSettings = DowngradeErrorHandling();
                var serviceContext = new InitialBoilerplateContext<ContractContext>(downgradedSettings, this.Identity, null);
                safeCall(serviceContext);
                return MergeCopy();
            });
        }

        public IContext<TResult> Do(Action<IContext, TResult> action)
        {
            return VerifyContractIfPossible(this.contractBundle,
                () =>
            {
                var safeCall = this.bundle.Errors.ExtendAround(action);
                var downgradedSettings = DowngradeErrorHandling();
                var serviceContext = new InitialBoilerplateContext<ContractContext>(downgradedSettings, this.Identity, null);
                safeCall(serviceContext, this.Result);
                return MergeCopy();
            });
        }

        public IContext<TResult> OpenService<TType>(Action<IContext<TResult>, TType> action)
        {
            return VerifyContractIfPossible(this.contractBundle,
                () =>
                {
                    var safeCall = this.bundle.Errors.ExtendAround(action);
                    var downgradedSettings = DowngradeErrorHandling();
                    var serviceContext = new ResultBoilerplateContext<TResult>(downgradedSettings, this.Identity, this.contractBundle, this.Result);
                    var response = this.bundle.Access.TryAccess<TType>(this.Identity, instance => safeCall(serviceContext, instance));
                    
                    if (response.IsSuccess)
                        return MergeCopy();
                    return this;
                });
        }
        
        public IConversionBuilder Use<TFrom>(TFrom instance)
        {
            return new ConversionBuilder<TFrom>(this.bundle.Translation, instance);
        }

        public IContext<TResult> MergeCopy(ContextBundle settings = null,
                                           IIdentity account = null,
                                           IContractBundle contractBundle = null,
                                           TResult result = default(TResult))
        {
            var resultWasNotSupplied = EqualityComparer<TResult>.Default.Equals(result, default(TResult));
            var actualResult = (resultWasNotSupplied) ? this.Result : result;
            return new ResultBoilerplateContext<TResult>(settings ?? this.bundle,
                                                         account ?? this.Identity,
                                                         contractBundle ?? this.contractBundle,
                                                         actualResult);
        }

        public IContext<TResult> Copy(ContextBundle bundle = null, IContractBundle contractBundle = null)
        {
            return new ResultBoilerplateContext<TResult>(bundle ?? this.bundle,
                                                         this.Identity,
                                                         contractBundle ?? this.contractBundle,
                                                         this.Result);
        }
    }
}


