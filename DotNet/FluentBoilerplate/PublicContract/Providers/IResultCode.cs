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
    /// Represents a result code
    /// </summary>
    public interface IResultCode
    {
        /// <summary>
        /// Gets a value indicating whether the operation was successful
        /// </summary>
        /// <value>
        /// <c>true</c> if the operation was successful, <c>false</c> otherwise
        /// </value>
        bool IsSuccess { get; }

        /// <summary>
        /// Gets the result code
        /// </summary>
        int Code { get; }

        /// <summary>
        /// Gets the description
        /// </summary>
        string Description { get; }
    }
}
