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

namespace FluentBoilerplate.Providers
{
    /// <summary>
    /// Represents a provider that validates types
    /// </summary>
    public interface IValidationProvider
    {
        /// <summary>
        /// Perform validation on an instance of <typeparamref name="TType"/>
        /// </summary>
        /// <typeparam name="TType">The validated type</typeparam>
        /// <param name="instance">The validated instance</param>
        /// <returns>An instance of <see cref="IValidationResult"/></returns>
        IValidationResult Validate<TType>(TType instance);
    }
}
