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
    /// Represents the kind of validations that may apply to an instance
    /// </summary>
    public enum ValidationKind
    {
        /// <summary>
        /// The validation kind is unknown
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// The validation kind does not matter
        /// </summary>
        Irrelevant = 1,
        /// <summary>
        /// The instance must not be null
        /// </summary>
        RequireNotNull = 1,
        /// <summary>
        /// The string length must be within the specified range
        /// </summary>
        StringLength = 2,
        /// <summary>
        /// The integer value must be within the specified range
        /// </summary>
        IntegerRange = 3,
        /// <summary>
        /// The string value must match the specified regular expression
        /// </summary>
        RegularExpressionMatch = 4
    }
}
