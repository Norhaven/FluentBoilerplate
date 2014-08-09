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
#if DEBUG
    public
#else
    internal 
#endif 
        class InitialBoilerplateContext<TContract> : 
        ImmutableContractAwareContext<InitialBoilerplateContext<TContract>>,
        IContext,
        IElevatableContext
        where TContract: IInitialContractContext, 
                         IBundledContractContext
    {
        private readonly IContractBundle contractBundle;
        public IIdentity Identity { get; private set; }

        internal InitialBoilerplateContext(IContextBundle bundle,
                                           IIdentity identity,
                                           IContractBundle contractBundle = null)
            :base(bundle)
        {
            this.Identity = identity;
            this.contractBundle = contractBundle ?? new ContractBundle();
        }

        public IInitialContractContext BeginContract()
        {
            return new ContractContext(this.bundle, new ContractBundle(), this);
        }

        public IContext<TResult> Get<TResult>(Func<IContext, TResult> action)
        {
            return VerifyContractIfPossible(this.contractBundle, this.Identity,
                () =>
                {
                    var safeCall = this.bundle.Errors.ExtendAround(action);
                    var downgradedSettings = DowngradeErrorHandling();
                    var downgradedContext = Copy(bundle: downgradedSettings);
                    var result = safeCall(downgradedContext);
                                        
                    return new ResultBoilerplateContext<TResult>(this.bundle,
                                                           this.Identity,
                                                           this.contractBundle,
                                                           result);
                });
        }

        public ITypeAccessBuilder<TType> Open<TType>()
        {
            return VerifyContractIfPossible(this.contractBundle, this.Identity,
                () => new TypeAccessBuilder<TType>(this.Identity, this.bundle, this.contractBundle, this));            
        }
        
        public IContext Do(Action<IContext> action)
        {
            return VerifyContractIfPossible(this.contractBundle, this.Identity,
                () =>
            {
                var safeCall = this.bundle.Errors.ExtendAround(action);
                var downgradedSettings = DowngradeErrorHandling();
                var downgradedContext = Copy(bundle: downgradedSettings);
                safeCall(downgradedContext);
                
                return new InitialBoilerplateContext<TContract>(this.bundle,
                                                                this.Identity,
                                                                this.contractBundle);
            });
        }

        public IContext Open<TType>(Action<IContext, TType> action)
        {
            return VerifyContractIfPossible(this.contractBundle, this.Identity,
                () =>
            {
                var safeCall = this.bundle.Errors.ExtendAround(action);
                var downgradedSettings = DowngradeErrorHandling();
                var downgradedContext = Copy(bundle: downgradedSettings);
                var response = this.bundle.Access.TryAccess<TType>(this.Identity, instance => safeCall(downgradedContext, instance));

                if (!response.IsSuccess)
                    throw new OperationWasNotSuccessfulException(response.Result);
                
                return new InitialBoilerplateContext<TContract>(this.bundle,
                                                                this.Identity,
                                                                this.contractBundle);               
            });
        }
        
        public IContext Copy(IContextBundle bundle)
        {
            return new InitialBoilerplateContext<TContract>(bundle, 
                                                            this.Identity,
                                                            this.contractBundle);
        }

        public IConversionBuilder Use<TFrom>(TFrom instance)
        {
            return new ConversionBuilder<TFrom>(this.bundle.Translation, instance);
        }
        
        public IContext Copy(IContextBundle bundle = null, 
                             IContractBundle contractBundle = null)
        {
            return new InitialBoilerplateContext<TContract>(bundle ?? this.bundle,
                                                            this.Identity,
                                                            contractBundle ?? this.contractBundle);
        }

        public IContext<TResult> Elevate<TResult>(IContextBundle bundle = null, IContractBundle contractBundle = null, TResult result = default(TResult))
        {
            return new ResultBoilerplateContext<TResult>(bundle ?? this.bundle,
                                                         this.Identity,
                                                         contractBundle ?? this.contractBundle,
                                                         default(TResult));
        }
    }
}
