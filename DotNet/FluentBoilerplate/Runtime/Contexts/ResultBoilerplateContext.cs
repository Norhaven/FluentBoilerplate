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
    internal class ResultBoilerplateContext<TResult> :
        ImmutableContractAwareContext<ResultBoilerplateContext<TResult>>,
        IBoilerplateContext<TResult>,
        IMergeableTrait<TResult>
    {
        private readonly IContractBundle contractBundle;
        private readonly IImmutableQueue<TimeSpan> callTimings;
        
        public IIdentity Identity { get; private set; }
        public TResult Result { get; private set; }

        internal ResultBoilerplateContext(IContextBundle bundle,
                                          IIdentity identity, 
                                          IContractBundle contractBundle,
                                          TResult result,
                                          IImmutableQueue<TimeSpan> callTimings)
            : base(bundle)
        {
            this.Identity = identity;
            this.contractBundle = contractBundle;
            this.Result = result;
            this.callTimings = callTimings;
        }

        public IResultContractContext<TResult> BeginContract()
        {
            return new ContractContext<TResult>(this.bundle, null, this);
        }

        public IBoilerplateContext<TResult> Get(Func<IBoilerplateContext, TResult> action)
        {
            return VerifyContractIfPossible(this.contractBundle, this.Identity,
                () =>
                {
                    var downgradedContext = DowngradeToInitial();

                    var timingContext = new TimingContext(this.bundle.TimingVisibility);

                    TResult result = default(TResult);
                    this.bundle.Errors.DoInContext(_ =>
                        {
                            timingContext.OpenAs(this.bundle.Visibility, () => result = action(downgradedContext));
                        });

                    var timings = timingContext.EnqueueElapsed(this.callTimings);

                    return MergeCopy(result: result, callTimings: timings);
                });
        }

        public IBoilerplateContext<TResult> Get(Func<IBoilerplateContext, TResult, TResult> action)
        {
            return VerifyContractIfPossible(this.contractBundle, this.Identity,
                () =>
            {
                var downgradedContext = DowngradeToInitial();

                TResult result = default(TResult);
                var timings = SafeTimedCall(() => result = action(downgradedContext, this.Result));

                return MergeCopy(result: result, callTimings: timings);
            });
        }

        public ITypeAccessBuilder<TType, TResult> Open<TType>()
        {
            return VerifyContractIfPossible(this.contractBundle, this.Identity,
                () => new TypeAccessBuilder<TType, TResult>(this.Identity, this.bundle, this.contractBundle, this, this.Result));
        }
        
        public IBoilerplateContext<TResult> Do(Action<IBoilerplateContext> action)
        {
            return VerifyContractIfPossible(this.contractBundle, this.Identity,
                () =>
            {
                var downgradedContext = DowngradeToInitial();

                var timings = SafeTimedCall(() => action(downgradedContext));

                return MergeCopy(callTimings: timings);
            });
        }

        public IBoilerplateContext<TResult> Do(Action<IBoilerplateContext, TResult> action)
        {
            return VerifyContractIfPossible(this.contractBundle, this.Identity,
                () =>
            {
                var downgradedContext = DowngradeToInitial();

                var timings = SafeTimedCall(() => action(downgradedContext, this.Result));
                
                return MergeCopy(callTimings: timings);
            });
        }
                
        public IConversionBuilder Use<TFrom>(TFrom instance)
        {
            return new ConversionBuilder<TFrom>(this.bundle.Translation, instance);
        }

        public IBoilerplateContext<TResult> MergeCopy(IContextBundle settings = null,
                                                      IIdentity account = null,
                                                      IContractBundle contractBundle = null,
                                                      TResult result = default(TResult),
                                                      IImmutableQueue<TimeSpan> callTimings = null)
        {
            var resultWasProvided = !EqualityComparer<TResult>.Default.Equals(result, default(TResult));
            var actualResult = (resultWasProvided) ? result : this.Result;

            return new ResultBoilerplateContext<TResult>(settings ?? this.bundle,
                                                         account ?? this.Identity,
                                                         contractBundle ?? this.contractBundle,
                                                         actualResult,
                                                         callTimings);
        }

        public IBoilerplateContext<TResult> Copy(IContextBundle bundle = null, 
                                                 IContractBundle contractBundle = null,
                                                 TResult result = default(TResult))
        {
            var resultWasProvided = !EqualityComparer<TResult>.Default.Equals(result, default(TResult));
            var actualResult = (resultWasProvided) ? result : this.Result;
 
            return new ResultBoilerplateContext<TResult>(bundle ?? this.bundle,
                                                         this.Identity,
                                                         contractBundle ?? this.contractBundle,
                                                         actualResult,
                                                         this.callTimings);
        }

        private InitialBoilerplateContext<ContractContext> DowngradeToInitial()
        {
            var downgradedBundle = DowngradeErrorHandling();
            return new InitialBoilerplateContext<ContractContext>(downgradedBundle, this.Identity);
        }

        private ResultBoilerplateContext<TResult> DowngradeCurrentContext()
        {
            var downgradedBundle = DowngradeErrorHandling();
            return new ResultBoilerplateContext<TResult>(downgradedBundle, 
                                                         this.Identity, 
                                                         this.contractBundle, 
                                                         this.Result,
                                                         this.callTimings);
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


