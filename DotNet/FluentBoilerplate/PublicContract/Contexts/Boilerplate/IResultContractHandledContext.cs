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

using FluentBoilerplate.Traits;
using System;

namespace FluentBoilerplate
{
    /// <summary>
    /// Represents a result-based context for constructing a contract
    /// </summary>
    /// <typeparam name="TResult">The result type</typeparam>
    public interface IResultContractHandledContext<TResult> : IResultContractContext<TResult>
    { 
        /// <summary>
        /// This exception handler will retry the action up to the specified number of times before handling the exception.
        /// </summary>
        /// <param name="maxRetryAttempts">The maximum number of retry attempts possible.</param>
        /// <param name="millisecondsInterval">The interval in milliseconds between retry attempts.</param>
        /// <param name="backoff">The strategy for backing off on retry attempts.</param>
        /// <returns>An instance of the current contract context.</returns>
        IResultContractContext<TResult> WithRetryOf(int maxRetryAttempts, int millisecondsInterval = 0, BackoffStrategy backoff = BackoffStrategy.None);
    }
}
