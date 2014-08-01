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
        private readonly ICopyableTrait<IContext> context;

        public TypeAccessBuilder(IIdentity identity,
                                 IContextBundle bundle,
                                 IContractBundle contractBundle,
                                 ICopyableTrait<IContext> context)
            : base(bundle)
        {
            this.identity = identity;
            this.contractBundle = contractBundle;
            this.context = context;
        }

        public IContext AndDo(Action<IContext, T> useType)
        {   
            var safeCall = this.bundle.Errors.ExtendAround(useType);
            var downgradedSettings = DowngradeErrorHandling();
            var downgradedContext = this.context.Copy(bundle: downgradedSettings);
            var response = this.bundle.Access.TryAccess<T>(this.identity, instance => safeCall(downgradedContext, instance));

            if (!response.IsSuccess)
                throw new OperationWasNotSuccessfulException(response.Result);

            return new InitialBoilerplateContext<ContractContext>(this.bundle,
                                                                  this.identity,
                                                                  this.contractBundle); 
        }

        public IContext<TResult> AndGet<TResult>(Func<IContext, T, TResult> useType)
        {
            var safeCall = this.bundle.Errors.ExtendAround(useType);
            var downgradedSettings = DowngradeErrorHandling();
            var downgradedContext = this.context.Copy(bundle: downgradedSettings);
            var response = this.bundle.Access.TryAccess<T, TResult>(this.identity, instance => safeCall(downgradedContext, instance));

            if (!response.IsSuccess)
                throw new OperationWasNotSuccessfulException(response.Result);

            return new ResultBoilerplateContext<TResult>(this.bundle,
                                                         this.identity,
                                                         this.contractBundle,
                                                         response.Content);    
        }
    }

    internal sealed class TypeAccessBuilder<T, TResult> : ImmutableContext, ITypeAccessBuilder<T, TResult>
    {
        private readonly IIdentity identity;
        private readonly IContractBundle contractBundle;
        private readonly IMergeableTrait<TResult> context;
        private readonly TResult result;

        public TypeAccessBuilder(IIdentity identity,
                                 IContextBundle bundle,
                                 IContractBundle contractBundle,
                                 IMergeableTrait<TResult> context,
                                 TResult result)
            : base(bundle)
        {
            this.identity = identity;
            this.contractBundle = contractBundle;
            this.context = context;
            this.result = result;
        }

        public IContext<TResult> AndGet(Func<IContext, T, TResult> useType)
        {
            var safeCall = this.bundle.Errors.ExtendAround(useType);
            var downgradedSettings = DowngradeErrorHandling();
            var serviceContext = Boilerplate.New(this.identity, this.bundle.Access);
            var response = this.bundle.Access.TryAccess<T, TResult>(this.identity, instance => safeCall(serviceContext, instance));
           
            if (response.IsSuccess)
                return this.context.MergeCopy(result: response.Content);
            return new ResultBoilerplateContext<TResult>(this.bundle, this.identity, this.contractBundle, this.result);
        }
    }
}
