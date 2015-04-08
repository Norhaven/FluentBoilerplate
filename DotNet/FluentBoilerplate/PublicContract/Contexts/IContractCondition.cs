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
    /// Represents a condition that's included in a contract
    /// </summary>
    public interface IContractCondition
    {
        /// <summary>
        /// Marks the contract as failed
        /// </summary>
        void Fail(Exception thrownException = null);

        /// <summary>
        /// Determines whether the condition has been met
        /// </summary>
        /// <returns>True if it was met, false otherwise</returns>
        bool IsConditionMet();
    }
}
