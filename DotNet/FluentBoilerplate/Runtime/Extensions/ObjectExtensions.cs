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
using FluentBoilerplate.Messages.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBoilerplate.Runtime.Extensions
{
    using System.Diagnostics.Contracts;

    public static class ObjectExtensions
    {       
        public static bool CanBe<T>(this object instance)
        {
            Contract.Requires(instance != null, CommonErrors.ParameterMustNotBeNull.WithValues("instance"));
            return instance.CanBe(typeof(T));
        }

        public static bool CanBe(this object instance, Type type)
        {
            return type.IsAssignableFrom(instance.GetType());
        }

        public static bool TryConvert<TTo>(this object from, Action<TTo> action)
        {
            if (!from.CanBe<TTo>())
                return false;

            var to = (TTo)from;
            action(to);
            return true;
        }

        public static bool TryConvert<TTo, TResult>(this object from, Func<TTo, TResult> action, out TResult result)
        {
            result = default(TResult);

            if (!from.CanBe<TTo>())
                return false;

            var to = (TTo)from;
            result = action(to);
            return true;
        }
    }
}
