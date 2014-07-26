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

namespace FluentBoilerplate.Providers
{
    /// <summary>
    /// Represents a provider that gives access to specific types
    /// </summary>
    public interface ITypeAccessProvider
    {
        /// <summary>
        /// Tries to access a type and use it
        /// </summary>
        /// <typeparam name="TType">The requested type</typeparam>
        /// <typeparam name="TResult">The result type</typeparam>
        /// <param name="identity">The current identity being used</param>
        /// <param name="useType">How you use the type, if access was successful</param>
        /// <returns>A response to your access attempt, including a result if access was successful</returns>
        IResponse<TResult> TryAccess<TType, TResult>(IIdentity identity, Func<TType, TResult> useType);

        /// <summary>
        /// Tries to access a type and use it
        /// </summary>
        /// <typeparam name="TType">The requested type</typeparam>
        /// <param name="identity">The current identity being used</param>
        /// <param name="useType">How you use the type, if access was successful</param>
        /// <returns>A response to your access attempt, including a result if access was successful</returns>
        IResponse TryAccess<TType>(IIdentity identity, Action<TType> useType);
    }
}
