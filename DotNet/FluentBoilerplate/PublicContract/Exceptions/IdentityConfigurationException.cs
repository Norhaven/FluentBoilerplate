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

namespace FluentBoilerplate
{
    /// <summary>
    /// Represents a violation of a contract
    /// </summary>
    [Serializable]
    public sealed class IdentityConfigurationException:Exception
    {
        /// <summary>
        /// Creates an instance of <see cref="IdentityConfigurationException"/>
        /// </summary>
        /// <param name="message">The message</param>
        public IdentityConfigurationException(string message)
            : base(message)
        {
        }
    }
}
