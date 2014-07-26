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
    /// Indicates that the type or member can be logged
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple=false, Inherited=true)]
    public sealed class LogAttribute:Attribute
    {
        /// <summary>
        /// A custom name for this type or member (overrides the type or member name in the log)
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Indicates the visibility levels that this type or member will be logged under. Defaults to All.
        /// </summary>
        public LogVisibility Visibility { get; set; }
        /// <summary>
        /// Indicates whether the type or member should be ignored when logging
        /// </summary>
        public bool Ignore { get; set; }
    }
}
