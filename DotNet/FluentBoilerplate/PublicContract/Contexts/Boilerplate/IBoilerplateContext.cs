﻿/*
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
using System.Collections.Immutable;

namespace FluentBoilerplate
{
    /// <summary>
    /// Represents a boilerplate context
    /// </summary>
    public interface IBoilerplateContext : 
        IConversionTrait
    {
        /// <summary>
        /// The current identity that this context is executing under
        /// </summary>
        IIdentity Identity { get; }

        /// <summary>
        /// All timings of method calls performed with this context
        /// </summary>
        IImmutableQueue<TimeSpan> CallTimings { get; }

        /// <summary>
        /// Begins a contract definition block
        /// </summary>
        /// <returns>A context that will define the contract block</returns>
        IInitialContractContext BeginContract();

        /// <summary>
        /// Gets a value, applying any existing contract
        /// </summary>
        /// <typeparam name="TResult">The result type</typeparam>
        /// <param name="action">How you will get the result</param>
        /// <returns>A boilerplate context that includes the result</returns>
        IBoilerplateContext<TResult> Get<TResult>(Func<IBoilerplateContext, TResult> action);

        /// <summary>
        /// Opens a particular type for use (e.g. service/database connection), applying any existing contract
        /// </summary>
        /// <typeparam name="TType">The requested type</typeparam>
        /// <returns>A builder for access to this type</returns>
        ITypeAccessBuilder<TType> Open<TType>();

        /// <summary>
        /// Performs an action, applying any existing contract
        /// </summary>
        /// <param name="action">How you will perform the action</param>
        /// <returns>A boilerplate context</returns>
        IBoilerplateContext Do(Action<IBoilerplateContext> action); 
    }

    /// <summary>
    /// Represents a boilerplate context that contains a result
    /// </summary>
    /// <typeparam name="TResult">The result type</typeparam>
    public interface IBoilerplateContext<TResult> :
        ICopyableTrait<IBoilerplateContext<TResult>, TResult>,
        IConversionTrait
    {
        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        TResult Result { get; }

        /// <summary>
        /// The current identity that this context is executing under
        /// </summary>
        IIdentity Identity { get; }

        /// <summary>
        /// Begins a contract definition block
        /// </summary>
        /// <returns>A context that will define the contract block</returns>
        IResultContractContext<TResult> BeginContract();

        /// <summary>
        /// Gets a value, applying any existing contract
        /// </summary>
        /// <param name="action">How you will get the result</param>
        /// <returns>A boilerplate context that includes the result</returns>
        IBoilerplateContext<TResult> Get(Func<IBoilerplateContext, TResult> action);

        /// <summary>
        /// Gets a value, applying any existing contract
        /// </summary>
        /// <param name="action">How you will get the result (current result value is included)</param>
        /// <returns>A boilerplate context that includes the result</returns>
        IBoilerplateContext<TResult> Get(Func<IBoilerplateContext, TResult, TResult> action);

        /// <summary>
        /// Opens a particular type for use (e.g. service/database connection), applying any existing contract
        /// </summary>
        /// <typeparam name="TType">The requested type</typeparam>
        /// <returns>A builder for access to this type</returns>
        ITypeAccessBuilder<TType, TResult> Open<TType>();

        /// <summary>
        /// Performs an action, applying any existing contract
        /// </summary>
        /// <param name="action">How you will perform the action</param>
        /// <returns>A boilerplate context</returns>
        IBoilerplateContext<TResult> Do(Action<IBoilerplateContext> action);

        /// <summary>
        /// Performs an action, applying any existing contract
        /// </summary>
        /// <param name="action">How you will perform the action (current result value is included)</param>
        /// <returns>A boilerplate context</returns>
        IBoilerplateContext<TResult> Do(Action<IBoilerplateContext, TResult> action);
    }    
}
