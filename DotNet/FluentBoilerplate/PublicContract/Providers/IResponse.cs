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

namespace FluentBoilerplate.Providers
{
    /// <summary>
    /// Represents a response
    /// </summary>
    public interface IResponse
    {
        /// <summary>
        /// Gets the result
        /// </summary>
        IResultCode Result { get; }
        /// <summary>
        /// Gets a value indicating whether the operation was successful
        /// </summary>
        /// <value>
        /// <c>true</c> if the operation was successful, <c>false</c> otherwise
        /// </value>
        bool IsSuccess { get; }
    }

    /// <summary>
    /// Represents a response that contains an instance
    /// </summary>
    /// <typeparam name="T">The instance type</typeparam>
    public interface IResponse<out T>:IResponse
    {
        /// <summary>
        /// Gets the response content
        /// </summary>
        T Content { get; }        
    }
}
