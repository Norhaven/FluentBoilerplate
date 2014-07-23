﻿/*
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
    public interface IBoilerplateContext : 
        IActionableTrait<IBoilerplateContext>,
        ICopyableTrait<IBoilerplateContext>,
        IConversionTrait<IBoilerplateContext>
    {
        IIdentity Identity { get; }
        IBoilerplateContractualContext BeginContract();

        IBoilerplateContext<TResult> Get<TResult>(Func<IBoilerplateContext, TResult> action);
        IBoilerplateContext<TResult> OpenService<TService, TResult>(Func<IBoilerplateContext, TService, TResult> action);
        IBoilerplateContext<TResult> OpenDataAccess<TEntity, TResult>(Func<IBoilerplateContext, TEntity, TResult> action);
    }
}
