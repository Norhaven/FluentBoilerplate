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

namespace FluentBoilerplate.Runtime.Providers.ErrorHandling
{
    /// <summary>
    /// Represents an exception handler
    /// </summary>
    /// <typeparam name="TException">The exception type</typeparam>
    internal interface IExceptionHandler<in TException>
    {
        int RetryCount { get; set; }
        int RetryIntervalInMilliseconds { get; set; }
        RetryBackoff Backoff { get; set; }
    }

    internal interface IVoidReturnExceptionHandler<in TException> : IExceptionHandler<TException>
    {
        /// <summary>
        /// Handles the exception
        /// </summary>
        /// <param name="exception">The exception</param>
        void Handle(TException exception);        
    }

    /// <summary>
    /// Represents an exception handler that can return a result
    /// </summary>
    /// <typeparam name="TException">The exception type</typeparam>
    /// <typeparam name="TResult">The result type</typeparam>
    internal interface IResultExceptionHandler<in TException, out TResult> : IExceptionHandler<TException> 
    {
        /// <summary>
        /// Handles the exception, returning a result
        /// </summary>
        /// <param name="exception">The exception</param>
        /// <returns>The result</returns>
        TResult Handle(TException exception);
    }
}
