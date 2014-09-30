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
        IBoilerplateContext,
        IElevatableContext
        where TContract: IInitialContractContext, 
                         IBundledContractContext
    {
        private readonly IContractBundle contractBundle;
        private readonly IImmutableQueue<TimeSpan> callTimings;

        public IIdentity Identity { get; private set; }
        public IImmutableQueue<TimeSpan> CallTimings { get { return this.callTimings; } }

        internal InitialBoilerplateContext(IContextBundle bundle,
                                           IIdentity identity,
                                           IContractBundle contractBundle = null,
                                           IImmutableQueue<TimeSpan> callTimings = null)
            :base(bundle)
        {
            this.Identity = identity;
            this.contractBundle = contractBundle ?? new ContractBundle();
            this.callTimings = callTimings.DefaultIfNull();
        }

        public IInitialContractContext BeginContract()
        {
            return new ContractContext(this.bundle, new ContractBundle(), this);
        }

        public IBoilerplateContext<TResult> Get<TResult>(Func<IBoilerplateContext, TResult> action)
        {
            return VerifyContractIfPossible(this.contractBundle, this.Identity,
                () =>
                {
                    var downgradedContext = DowngradeCurrentContext();

                    TResult result = default(TResult);
                    var timings = SafeTimedCall(() => result = action(downgradedContext));

                    return new ResultBoilerplateContext<TResult>(this.bundle,
                                                                 this.Identity,
                                                                 this.contractBundle,
                                                                 result,
                                                                 timings);
                });
        }
        
        public ITypeAccessBuilder<TType> Open<TType>()
        {
            return VerifyContractIfPossible(this.contractBundle, this.Identity,
                () => new TypeAccessBuilder<TType>(this.Identity, this.bundle, this.contractBundle, this));            
        }
        
        public IBoilerplateContext Do(Action<IBoilerplateContext> action)
        {
            return VerifyContractIfPossible(this.contractBundle, this.Identity,
                () =>
            {
                var downgradedContext = DowngradeCurrentContext();

                var timings = SafeTimedCall(() => action(downgradedContext));

                return new InitialBoilerplateContext<TContract>(this.bundle,
                                                                this.Identity,
                                                                this.contractBundle,
                                                                timings);
            });
        }
          
        public IConversionBuilder Use<TFrom>(TFrom instance)
        {
            return new ConversionBuilder<TFrom>(this.bundle.Translation, instance);
        }
        
        public IBoilerplateContext Copy(IContextBundle bundle = null, 
                                        IContractBundle contractBundle = null)
        {
            return new InitialBoilerplateContext<TContract>(bundle ?? this.bundle,
                                                            this.Identity,
                                                            contractBundle ?? this.contractBundle);
        }

        public IBoilerplateContext<TResult> Elevate<TResult>(IContextBundle bundle = null, 
                                                             IContractBundle contractBundle = null, 
                                                             TResult result = default(TResult))
        {
            return new ResultBoilerplateContext<TResult>(bundle ?? this.bundle,
                                                         this.Identity,
                                                         contractBundle ?? this.contractBundle,
                                                         result,
                                                         this.callTimings);
        }

        private InitialBoilerplateContext<ContractContext> DowngradeCurrentContext()
        {
            var downgradedBundle = DowngradeErrorHandling();
            return new InitialBoilerplateContext<ContractContext>(downgradedBundle, this.Identity);
        }

        private IImmutableQueue<TimeSpan> SafeTimedCall(Action action)
        {
            var timingContext = new TimingContext(this.bundle.TimingVisibility);

            this.bundle.Errors.DoInContext(_ =>
            {
                timingContext.OpenAs(this.bundle.Visibility, action);
            });

            return timingContext.EnqueueElapsed(this.callTimings);
        }
    }
}
