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
using System.Collections.Immutable;
namespace FluentBoilerplate.Providers
{
    public interface IExceptionHandlerProvider 
    {
        IImmutableSet<Type> HandledExceptionTypes { get; }
        IImmutableQueue<Type> HandledTypesInCatchOrder { get; }
        IExceptionHandler<TException> TryGetHandler<TException>() where TException : Exception;
        IExceptionHandler<TException, TResult> TryGetHandler<TException, TResult>() where TException : Exception;
        IExceptionHandlerProvider Add<TException>(string sectionName, Action<TException> action) where TException : Exception;
        IExceptionHandlerProvider Add<TException, TResult>(string sectionName, Func<TException, TResult> action) where TException : Exception;
    }
}
