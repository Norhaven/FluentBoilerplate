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

using System;
using System.Collections.Immutable;

namespace FluentBoilerplate.Contexts
{
    /// <summary>
    /// Represents all pre and post conditions that a contract may include
    /// </summary>
    public interface IContractBundle
    {
        /// <summary>
        /// Gets the ordered preconditions for this contract that apply prior to performing an action
        /// </summary>
        IImmutableQueue<IContractCondition> Preconditions { get; }

        /// <summary>
        /// Gets the ordered postconditions for this contract that apply when returning from an action
        /// </summary>
        IImmutableQueue<IContractCondition> PostconditionsOnReturn { get; }

        /// <summary>
        /// Gets the ordered postconditions for this contract that apply when an action throws an exception
        /// </summary>
        IImmutableQueue<IContractCondition> PostconditionsOnThrow { get; }

        /// <summary>
        /// Gets the ordered instance validations for this contract that apply prior to performing an action
        /// </summary>
        IImmutableQueue<Action> InstanceValidations { get; }

        /// <summary>
        /// Adds a precondition to the contract
        /// </summary>
        /// <param name="condition">The condition</param>
        /// <returns>An instance of <see cref="IContractBundle"/> with the condition included</returns>
        IContractBundle AddPrecondition(IContractCondition condition);

        /// <summary>
        /// Adds a postcondition (when returning) to the contract
        /// </summary>
        /// <param name="condition">The condition</param>
        /// <returns>An instance of <see cref="IContractBundle"/> with the condition included</returns>
        IContractBundle AddPostconditionOnReturn(IContractCondition condition);

        /// <summary>
        /// Adds a postconditon (when exceptions are thrown) to the contract
        /// </summary>
        /// <param name="condition">The condition</param>
        /// <returns>An instance of <see cref="IContractBundle"/> with the condition included</returns>
        IContractBundle AddPostconditionOnThrow(IContractCondition condition);

        /// <summary>
        /// Adds an instance validation precondition to the contract
        /// </summary>
        /// <param name="validate">The validation method</param>
        /// <returns>An instance of <see cref="IContractBundle"/> with the condition included</returns>
        IContractBundle AddInstanceValidation(Action validate);
    }
}
