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

namespace FluentBoilerplate.Runtime.Contexts
{
    /// <summary>
    /// Represents a context that can verify a contract
    /// </summary>
    internal interface IVerifiableContractContext
    {
        /// <summary>
        /// Verify all preconditions contained by the context
        /// </summary>
        void VerifyPreConditions();
        /// <summary>
        /// Verify all postconditions contained by the context, for the given way of exiting the contract
        /// </summary>
        /// <param name="exit">The way of exiting the contract</param>
        void VerifyPostConditions(ContractExit exit);
    }
}
