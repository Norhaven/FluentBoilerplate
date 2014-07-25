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
using FluentBoilerplate.Traits;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace FluentBoilerplate
{
    public interface IContext : 
        IConversionTrait<IContext>
    {
        IIdentity Identity { get; }
        IInitialContractContext BeginContract();

        IContext<TResult> Get<TResult>(Func<IContext, TResult> action);
        IContext<TResult> Open<TType, TResult>(Func<IContext, TType, TResult> action);
        IContext Do(Action<IContext> action); 
    }

    public interface IContext<TResult> :
        ICopyableTrait<IContext<TResult>>,
        IConversionTrait<IContext<TResult>>
    {
        TResult Result { get; }
        IIdentity Identity { get; }
        IResultContractContext<TResult> BeginContract();

        IContext<TResult> Get(Func<IContext, TResult> action);
        IContext<TResult> Get(Func<IContext, TResult, TResult> action);
        IContext<TResult> Open<TType>(Func<IContext<TResult>, TType, TResult> action);
        IContext<TResult> Do(Action<IContext> action);
        IContext<TResult> Do(Action<IContext, TResult> action);
    }    
}
