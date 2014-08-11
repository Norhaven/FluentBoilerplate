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

using FluentBoilerplate.Providers;
using System;

namespace FluentBoilerplate
{
    /// <summary>
    /// Represents a failure to accomplish an action
    /// </summary>
    [Serializable]
    public sealed class OperationWasNotSuccessfulException : Exception
    {
        /// <summary>
        /// Gets the result code for the failed operation
        /// </summary>
        public IResultCode Result { get; private set; }

        /// <summary>
        /// Creates an instance of <see cref="OperationWasNotSuccessfulException"/>
        /// </summary>
        /// <param name="result">The result of the failed operation</param>
        public OperationWasNotSuccessfulException(IResultCode result)
            :base("Operation was not successful")
        {
            this.Result = result;
        }
    }
}
