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

using FluentBoilerplate.Contexts;
using FluentBoilerplate.Runtime.Contexts;
using FluentBoilerplate.Traits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBoilerplate.Runtime
{
    internal sealed class TypeAccessBuilder<T>:ImmutableContext, ITypeAccessBuilder<T>
    {
        private readonly IIdentity identity;
        private readonly IContractBundle contractBundle;
        private readonly IBoilerplateContext context;

        public TypeAccessBuilder(IIdentity identity,
                                 IContextBundle bundle,
                                 IContractBundle contractBundle,
                                 IBoilerplateContext context)
            : base(bundle)
        {
            this.identity = identity;
            this.contractBundle = contractBundle;
            this.context = context;
        }

        public IBoilerplateContext AndDo(Action<IBoilerplateContext, T> useType)
        {
            var downgradedContext = DowngradeToInitial();
            var timingContext = new TimingContext(this.bundle.TimingVisibility);
            var access = this.bundle.Access;
            var response = access.TryAccess<T>(this.identity, instance =>
                {
                    this.bundle.Errors.DoInContext(_ =>
                        {
                            timingContext.OpenAs(this.bundle.Visibility, () => useType(downgradedContext, instance));
                        });
                });

            if (!response.IsSuccess)
                throw new OperationWasNotSuccessfulException(response.Result);

            var timings = timingContext.EnqueueElapsed(this.context.CallTimings);
            
            return new InitialBoilerplateContext<ContractContext>(this.bundle,
                                                                  this.identity,
                                                                  this.contractBundle,
                                                                  timings); 
        }

        public IBoilerplateContext<TResult> AndGet<TResult>(Func<IBoilerplateContext, T, TResult> useType)
        {
            var downgradedContext = DowngradeToInitial();
            var timingContext = new TimingContext(this.bundle.TimingVisibility);

            var access = this.bundle.Access;
            var response = access.TryAccess<T, TResult>(this.identity, instance =>
                {
                    return this.bundle.Errors.DoInContext<TResult>(_ =>
                        {
                            TResult result = default(TResult);
                            timingContext.OpenAs(this.bundle.Visibility, () => result = useType(downgradedContext, instance));
                            return result;
                        });
                });

            if (!response.IsSuccess)
                throw new OperationWasNotSuccessfulException(response.Result);

            var timings = timingContext.EnqueueElapsed(this.context.CallTimings);

            return new ResultBoilerplateContext<TResult>(this.bundle,
                                                         this.identity,
                                                         this.contractBundle,
                                                         response.Content,
                                                         timings);    
        }

        private InitialBoilerplateContext<ContractContext> DowngradeToInitial()
        {
            var downgradedBundle = DowngradeErrorHandling();
            return new InitialBoilerplateContext<ContractContext>(downgradedBundle, this.identity);
        }
    }

    internal sealed class TypeAccessBuilder<T, TResult> : ImmutableContext, ITypeAccessBuilder<T, TResult>
    {
        private readonly IIdentity identity;
        private readonly IContractBundle contractBundle;
        private readonly IBoilerplateContext<TResult> context;
        private readonly TResult result;

        public TypeAccessBuilder(IIdentity identity,
                                 IContextBundle bundle,
                                 IContractBundle contractBundle,
                                 IBoilerplateContext<TResult> context,
                                 TResult result)
            : base(bundle)
        {
            this.identity = identity;
            this.contractBundle = contractBundle;
            this.context = context;
            this.result = result;
        }

        public IBoilerplateContext<TResult> AndGet(Func<IBoilerplateContext, T, TResult> useType)
        {   
            var downgradedContext = DowngradeToInitial();
            var timingContext = new TimingContext(this.bundle.TimingVisibility);

            var access = this.bundle.Access;
            var response = access.TryAccess<T, TResult>(this.identity, instance =>
                {
                    return this.bundle.Errors.DoInContext<TResult>(_ =>
                        {
                            return useType(downgradedContext, instance);
                        });
                });
            
            if (response.IsSuccess)
                return this.context.Copy(result: response.Content);
            return this.context;
        }

        private InitialBoilerplateContext<ContractContext> DowngradeToInitial()
        {
            var downgradedBundle = DowngradeErrorHandling();
            return new InitialBoilerplateContext<ContractContext>(downgradedBundle, this.identity);
        }
    }
}
