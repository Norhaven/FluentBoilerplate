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

namespace FluentBoilerplate.Contexts
{
    public interface IImmutableErrorContext
    {
        bool HasHandlers { get; }
        int HandlerCount { get; }
        bool HasHandlerFor<TException>() where TException : Exception;        
        IImmutableErrorContext RegisterExceptionHandler<TException>(string sectionName, Action<TException> handler) where TException : Exception;
        IImmutableErrorContext RegisterExceptionHandler<TException, TResult>(string sectionName, Func<TException, TResult> handler) where TException : Exception;
        void DoInContext(Action<IImmutableErrorContext> action);
        T DoInContext<T>(Func<IImmutableErrorContext, T> action);
        Action ExtendAround(Action action);
        Action<T> ExtendAround<T>(Action<T> action);
        Action<T1, T2> ExtendAround<T1, T2>(Action<T1, T2> action);
        Func<T> ExtendAround<T>(Func<T> action);
        Func<T, TResult> ExtendAround<T, TResult>(Func<T, TResult> action);
        Func<T1, T2, TResult> ExtendAround<T1, T2, TResult>(Func<T1, T2, TResult> action);

        IImmutableErrorContext Copy(bool includeHandlers = true);
    }
}
