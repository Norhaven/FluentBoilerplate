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

namespace FluentBoilerplate.Contexts
{
    /// <summary>
    /// Represents an immutable context for handling errors
    /// </summary>
    public interface IImmutableErrorContext
    {
        /// <summary>
        /// Gets whether the context has any exception handlers
        /// </summary>
        bool HasHandlers { get; }
        /// <summary>
        /// Gets the number of exception handlers that this context contains
        /// </summary>
        int HandlerCount { get; }
        /// <summary>
        /// Gets whether the context has an exception handler for the given exception type
        /// </summary>
        /// <typeparam name="TException">The exception type</typeparam>
        /// <returns>True if the context contains a handler for that type, false otherwise</returns>
        bool HasHandlerFor<TException>() where TException : Exception;        
        /// <summary>
        /// Registers the given exception handler with the error context
        /// </summary>
        /// <typeparam name="TException">The exception type</typeparam>
        /// <param name="handler">The exception handler</param>
        /// <returns>An instance of <see cref="IImmutableErrorContext"/> with the exception handler registered</returns>
        IImmutableErrorContext RegisterExceptionHandler<TException>(Action<TException> handler) where TException : Exception;
        /// <summary>
        /// Registers the given exception handler with the error context
        /// </summary>
        /// <typeparam name="TException">The exception type</typeparam>
        /// <typeparam name="TResult">The result type</typeparam>
        /// <param name="handler">The exception handler</param>
        /// <returns>An instance of <see cref="IImmutableErrorContext"/> with the exception handler registered</returns>
        IImmutableErrorContext RegisterExceptionHandler<TException, TResult>(Func<TException, TResult> handler) where TException : Exception;
        /// <summary>
        /// Marks the exception handler for retry.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="retryCount">The retry count.</param>
        /// <param name="millisecondsInterval">The interval in milliseconds before another retry may be attempted.</param>
        /// <param name="backoff">How subsequent retries should handle backing off of the retry interval.</param>
        /// <returns>An instance of <see cref="IImmutableErrorContext"/> with the exception handler marked for retry.</returns>
        IImmutableErrorContext MarkExceptionHandlerForRetry<TException>(int retryCount, int millisecondsInterval = 0, BackoffStrategy backoff = BackoffStrategy.None) where TException : Exception;
        /// <summary>
        /// Marks the exception handler for retry.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="retryCount">The retry count.</param>
        /// <param name="millisecondsInterval">The interval in milliseconds before another retry may be attempted.</param>
        /// <param name="backoff">How subsequent retries should handle backing off of the retry interval.</param>
        /// <returns>An instance of <see cref="IImmutableErrorContext"/> with the exception handler marked for retry.</returns>
        IImmutableErrorContext MarkExceptionHandlerForRetry<TException, TResult>(int retryCount, int millisecondsInterval = 0, BackoffStrategy backoff = BackoffStrategy.None) where TException : Exception;
        /// <summary>
        /// Performs an action, surrounded by the context's exception handlers
        /// </summary>
        /// <param name="action">The action</param>
        void DoInContext(Action<IImmutableErrorContext> action);
        /// <summary>
        /// Gets a value, surrounded by the context's exception handlers
        /// </summary>
        /// <typeparam name="T">The result type</typeparam>
        /// <param name="action">The action</param>
        /// <returns>The value</returns>
        T DoInContext<T>(Func<IImmutableErrorContext, T> action);

        /// <summary>
        /// Wraps an action in the context's exception handlers
        /// </summary>
        /// <param name="action">The action</param>
        /// <returns>An action that encompasses the given action in the context's exception handlers</returns>
        Action ExtendAround(Action action);
        /// <summary>
        /// Wraps an action in the context's exception handlers
        /// </summary>
        /// <typeparam name="T">The parameter type</typeparam>
        /// <param name="action">The action</param>
        /// <returns>An action that encompasses the given action in the context's exception handlers</returns>
        Action<T> ExtendAround<T>(Action<T> action);
        /// <summary>
        /// Wraps an action in the context's exception handlers
        /// </summary>
        /// <typeparam name="T1">The first parameter type</typeparam>
        /// <typeparam name="T2">The second parameter type</typeparam>
        /// <param name="action">The action</param>
        /// <returns>An action that encompasses the given action in the context's exception handlers</returns>
        Action<T1, T2> ExtendAround<T1, T2>(Action<T1, T2> action);
        /// <summary>
        /// Wraps a function in the context's exception handlers
        /// </summary>
        /// <typeparam name="T">The return type</typeparam>
        /// <param name="action">The action</param>
        /// <returns>A function that encompasses the given function in the context's exception handlers</returns>
        Func<T> ExtendAround<T>(Func<T> action);
        /// <summary>
        /// Wraps a function in the context's exception handlers
        /// </summary>
        /// <typeparam name="T">The parameter type</typeparam>
        /// <typeparam name="TResult">The return type</typeparam>
        /// <param name="action">The action</param>
        /// <returns>A function that encompasses the given function in the context's exception handlers</returns>
        Func<T, TResult> ExtendAround<T, TResult>(Func<T, TResult> action);
        /// <summary>
        /// Wraps a function in the context's exception handlers
        /// </summary>
        /// <typeparam name="T1">The first parameter type</typeparam>
        /// <typeparam name="T2">The second parameter type</typeparam>
        /// <typeparam name="TResult">The return type</typeparam>
        /// <param name="action">The action</param>
        /// <returns>A function that encompasses the given function in the context's exception handlers</returns>
        Func<T1, T2, TResult> ExtendAround<T1, T2, TResult>(Func<T1, T2, TResult> action);
        /// <summary>
        /// Copies the error context
        /// </summary>
        /// <param name="includeHandlers">Indicates whether the exception handlers in the context should be copied</param>
        /// <returns>An instance of <see cref="IImmutableErrorContext"/></returns>
        IImmutableErrorContext Copy(bool includeHandlers = true);
    }
}
