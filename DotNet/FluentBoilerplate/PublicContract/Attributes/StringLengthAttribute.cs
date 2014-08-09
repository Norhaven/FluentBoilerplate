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

namespace FluentBoilerplate
{
    /// <summary>
    /// Indicates that the string must have a length within the expected bounds.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false)]
    public sealed class StringLengthAttribute:Attribute, IValidationAttribute
    {
        /// <summary>
        /// The inclusive minimum length of the string. Defaults to zero.
        /// </summary>
        public uint Minimum { get; set; }
        /// <summary>
        /// The inclusive maximum length of the string. Defaults to zero, which indicates that there is no maximum length.
        /// </summary>
        public uint Maximum { get; set; }                
    }
}
