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

namespace FluentBoilerplate.Traits
{
    /// <summary>
    /// Represents a trait for allowing type conversion
    /// </summary>
    public interface IConversionTrait
    {
        /// <summary>
        /// Selects an instance to convert
        /// </summary>
        /// <typeparam name="TFrom">The type of the instance</typeparam>
        /// <param name="instance">The instance to convert</param>
        /// <returns>An instance of <see cref="IConversionBuilder"/> that will complete the conversion</returns>
        IConversionBuilder Use<TFrom>(TFrom instance);
    }
}
