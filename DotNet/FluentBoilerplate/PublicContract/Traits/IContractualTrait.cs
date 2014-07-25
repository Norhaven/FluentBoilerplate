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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBoilerplate.Traits
{
    public interface IContractualTrait<TContext>:IPermissionsBasedTrait<TContext>
    {
        TContext RequiresValidInstanceOf<TType>(params TType[] instances);
        TContext Require(Func<bool> condition, string message = null);
        TContext Require<TException>(Func<bool> condition, Func<TException> createException = null) where TException : Exception;
        TContext EnsureOnReturn(Func<bool> condition, string message = null);
        TContext EnsureOnReturn<TException>(Func<bool> condition, Func<TException> createException = null) where TException : Exception;
        TContext EnsureOnThrow(Func<bool> condition, string message = null);
        TContext EnsureOnThrow<TException>(Func<bool> condition, Func<TException> createException = null) where TException : Exception;   
    }

    public interface IContractualTrait<TContext, out TResult> : IPermissionsBasedTrait<TContext>
    {
        TResult Result { get; }
        TContext RequiresValidInstanceOf<TType>(params TType[] instances);
        TContext Require(Func<bool> condition, string message = null);
        TContext Require<TException>(Func<bool> condition, Func<TException> createException = null) where TException : Exception;
        TContext EnsureOnReturn(Func<bool> condition, string message = null);
        TContext EnsureOnReturn<TException>(Func<bool> condition, Func<TException> createException = null) where TException : Exception;
        TContext EnsureOnThrow(Func<bool> condition, string message = null);
        TContext EnsureOnThrow<TException>(Func<bool> condition, Func<TException> createException = null) where TException : Exception; 
    }
}
