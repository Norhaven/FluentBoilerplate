/*
   Copyright 2015 Chris Hannon

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
namespace FluentBoilerplate
{
    /// <summary>
    /// Represents a type conversion
    /// </summary>
    public interface IConversionBuilder<TFrom>:ITypeCheckExecution
    {
        /// <summary>
        /// Converts the instance to the requested type
        /// </summary>
        /// <typeparam name="TType">The target type</typeparam>
        /// <returns>An instance of <typeparamref name="TType"/></returns>
        TType As<TType>();

        /// <summary>
        /// Converts the instance to an atomic version of the provided instance.
        /// </summary>
        /// <returns>An atomic version of the provided instance.</returns>
        Atomic<TFrom> AsAtomic();
    }
}
