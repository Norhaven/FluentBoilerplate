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
    public static class FuncExtensions
    {
        public static Action AsAction<T>(this Func<T> func)
        {
            return () => func();
        }

        public static Action<T> AsAction<T, TResult>(this Func<T, TResult> func)
        {
            return value => func(value);
        }

        public static Func<object, T2> Generalize<T1, T2>(this Func<T1, T2> func)
        {
            return value =>
            {
                Debug.Assert(value is T1, AssertFailures.ParameterIsNotExpectedType.WithValues("value", typeof(T1)));

                return func((T1)value);
            };
        }
                
        public static void IgnoreResult<T>(this Func<T> func)
        {
            func();
        }
        public static void IgnoreResult<T, TResult>(this Func<T, TResult> func, T parameter)
        {
            func(parameter);
        }
        
        public static Func<Func<T, TResult>, Tuple<TOldResult, TResult>> SubstituteFuncForAction<T, TOldResult, TResult>(this Func<Action<T>, TOldResult> action)
        {
            return function =>
                {
                    TResult result = default(TResult);
                    var oldResult = action(parameter => { result = function(parameter); });
                    return new Tuple<TOldResult, TResult>(oldResult, result);
                };
        }

        public static Func<Func<T1, T2, TResult>, Tuple<TOldResult, TResult>> SubstituteFuncForAction<T1, T2, TOldResult, TResult>(this Func<Action<T1, T2>, TOldResult> action)
        {
            return function =>
            {
                TResult result = default(TResult);
                var oldResult = action((p1, p2) => { result = function(p1, p2); });
                return new Tuple<TOldResult, TResult>(oldResult, result);
            };
        }
        
        public static Func<T> PartiallyApply<TParam, T>(this Func<TParam, T> func, TParam parameter)
        {
            return () => func(parameter);
        }

        public static Func<TParam2, T> PartiallyApply<TParam1, TParam2, T>(this Func<TParam1, TParam2, T> func, TParam1 parameter)
        {
            return secondParameter => func(parameter, secondParameter);
        }
    }
}
