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

using FluentBoilerplate.Messages;
using FluentBoilerplate.Messages.Developer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBoilerplate.Runtime.Extensions
{
    internal static class ActionExtensions
    {
        public static Func<T> AsFunc<T>(this Action action)
        {
            return () =>
                {
                    action();
                    return default(T);
                };
        }

        public static Func<Func<T, TResult>, TResult> SubstituteFuncForAction<T, TResult>(this Action<Action<T>> action)
        {
            return function =>
                {
                    TResult result = default(TResult);
                    action(parameter => { result = function(parameter); });
                    return result;
                };
        }

        public static Func<Func<T1, T2, TResult>, TResult> SubstituteFuncForAction<T1, T2, TResult>(this Action<Action<T1, T2>> action)
        {
            return function =>
                {
                    TResult result = default(TResult);
                    action((p1, p2) => { result = function(p1, p2); });
                    return result;
                };
        }

        public static Action<object> Generalize<T>(this Action<T> action)
        {
            return value =>
                {
                    Debug.Assert(value is T, AssertFailures.ParameterIsNotExpectedType.WithValues("value", typeof(T)));

                    action((T)value);
                };
        }
    }
}
