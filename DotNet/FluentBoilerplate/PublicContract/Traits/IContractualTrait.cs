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

namespace FluentBoilerplate.Traits
{
    /// <summary>
    /// Represents a trait for constructing a contract
    /// </summary>
    /// <typeparam name="TContext">The context type that this trait applies to</typeparam>
    public interface IContractualTrait<TContext>:IPermissionsBasedTrait<TContext>
    {
        /// <summary>
        /// Indicates that the given instances should be validated against any validation attributes they contain
        /// </summary>
        /// <typeparam name="TType">The validated type</typeparam>
        /// <param name="instances">Instances that require validation</param>
        /// <returns>The context that this trait applies to</returns>
        TContext RequireValidInstanceOf<TType>(params TType[] instances);

        /// <summary>
        /// Indicates that the given condition should be validated prior to a context action
        /// </summary>
        /// <param name="condition">The required condition</param>
        /// <param name="message">A message that will be included with the <see cref="ContractViolationException"/> that's thrown on failure</param>
        /// <returns>The context that this trait applies to</returns>
        /// <exception cref="ContractViolationException">Thrown if the condition fails</exception>
        TContext Require(Func<bool> condition, string message = null);

        /// <summary>
        /// Indicates that the given condition should be validated prior to a context action
        /// </summary>
        /// <typeparam name="TException">The type of exception that will be thrown if the condition fails</typeparam>
        /// <param name="condition">The required condition</param>
        /// <param name="createException">Creates the exception that will be thrown if the condition fails</param>
        /// <returns>The context that this trait applies to</returns>
        /// <exception cref="TException">Thrown if the condition fails</exception>
        TContext Require<TException>(Func<bool> condition, Func<TException> createException = null) where TException : Exception;

        /// <summary>
        /// Indicates that the given condition should be validated when returning from a context action
        /// </summary>
        /// <param name="condition">The required condition</param>
        /// <param name="message">A message that will be included with the <see cref="ContractViolationException"/> that's thrown on failure</param>
        /// <returns>The context that this trait applies to</returns>
        /// <exception cref="ContractViolationException">Thrown if the condition fails</exception>
        TContext EnsureOnReturn(Func<bool> condition, string message = null);

        /// <summary>
        /// Indicates that the given condition should be validated when returning from a context action
        /// </summary>
        /// <typeparam name="TException">The type of exception that will be thrown if the condition fails</typeparam>
        /// <param name="condition">The required condition</param>
        /// <param name="createException">Creates the exception that will be thrown if the condition fails</param>
        /// <returns>The context that this trait applies to</returns>
        /// <exception cref="TException">Thrown if the condition fails</exception>
        TContext EnsureOnReturn<TException>(Func<bool> condition, Func<TException> createException = null) where TException : Exception;

        /// <summary>
        /// Indicates that the given condition should be validated when a context action throws an exception
        /// </summary>
        /// <param name="condition">The required condition</param>
        /// <param name="message">A message that will be included with the <see cref="ContractViolationException"/> that's thrown on failure</param>
        /// <returns>The context that this trait applies to</returns>
        /// <exception cref="ContractViolationException">Thrown if the condition fails</exception>
        TContext EnsureOnThrow(Func<bool> condition, string message = null);

        /// <summary>
        /// Indicates that the given condition should be validated when a context action throws an exception
        /// </summary>
        /// <typeparam name="TException">The type of exception that will be thrown if the condition fails</typeparam>
        /// <param name="condition">The required condition</param>
        /// <param name="createException">Creates the exception that will be thrown if the condition fails</param>
        /// <returns>The context that this trait applies to</returns>
        /// <exception cref="TException">Thrown if the condition fails</exception>
        TContext EnsureOnThrow<TException>(Func<bool> condition, Func<Exception, TException> createException = null) where TException : Exception;

        /// <summary>
        /// Indicates that the context action will be timed when running under the specified visibility
        /// </summary>
        /// <param name="visibility">The timing visibility</param>
        /// <returns>The context that this trait applies to</returns>
        TContext IsTimedUnder(Visibility visibility);

        /// <summary>
        /// Gets the kinds of restrictions that this contract could impose.
        /// </summary>
        IRestrictionBuilder<TContext> Restrict { get; }
    }

    /// <summary>
    /// Represents a trait for constructing a contract
    /// </summary>
    /// <typeparam name="TContext">The context type that this trait applies to</typeparam>
    /// <typeparam name="TResult">The result type</typeparam>
    public interface IContractualTrait<TContext, out TResult> : IContractualTrait<TContext>, IPermissionsBasedTrait<TContext>
    {
        /// <summary>
        /// Gets the result
        /// </summary>
        TResult Result { get; }
    }
}
