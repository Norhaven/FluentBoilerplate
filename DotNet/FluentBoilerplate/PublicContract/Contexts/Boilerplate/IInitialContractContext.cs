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

namespace FluentBoilerplate
{
    /// <summary>
    /// Represents an initial context for constructing a contract
    /// </summary>
    public interface IInitialContractContext : IContractualTrait<IInitialContractContext>
    {
        /// <summary>
        /// Indicates that <see cref="TException"/> will be handled if thrown during a context action
        /// </summary>
        /// <typeparam name="TException">The exception type</typeparam>
        /// <typeparam name="TResult">The result type</typeparam>
        /// <param name="action">How the exception will be handled (if omitted, falls back to default handler)</param>
        /// <returns>A context geared towards result-based contract definitions</returns>
        IResultContractHandledContext<TResult> Handles<TException, TResult>(Func<TException, TResult> action = null) where TException : Exception;

        /// <summary>
        /// Indicates that <see cref="TException"/> will be handled if thrown during a context action
        /// </summary>
        /// <typeparam name="TException">The exception type</typeparam>
        /// <param name="action">How the exception will be handled (if omitted, falls back to default handler)</param>
        /// <returns>A context geared towards void-returning contract definitions</returns>
        IVoidReturnContractHandledContext Handles<TException>(Action<TException> action = null) where TException : Exception;

        /// <summary>
        /// Ends the contract definition for the context
        /// </summary>
        /// <returns>The context that this contract applies to</returns>
        IBoilerplateContext EndContract();
    }
}
