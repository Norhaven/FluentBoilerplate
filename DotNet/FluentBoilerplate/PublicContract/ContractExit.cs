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

namespace FluentBoilerplate
{
    /// <summary>
    /// Represents possible ways that a contract-oriented method could be exited
    /// </summary>
    public enum ContractExit
    {
        /// <summary>
        /// The method returned successfully
        /// </summary>
        Returned,
        /// <summary>
        /// The method threw an exception
        /// </summary>
        ThrewException
    }
}
