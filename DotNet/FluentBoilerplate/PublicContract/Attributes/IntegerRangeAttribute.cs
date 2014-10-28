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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBoilerplate
{
    /// <summary>
    /// Indicates that the integer must have a value within the specified range.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false)]
    public sealed class IntegerRangeAttribute : Attribute, IValidationAttribute
    {
        private long? minimum;
        private long? maximum;

        /// <summary>
        /// The inclusive minimum value for the range. Defaults to zero.
        /// </summary>
        public long Minimum { get { return this.minimum.GetValueOrDefault(); } set { this.minimum = value; } }
        /// <summary>
        /// The inclusive maximum value for the range. Defaults to zero.
        /// </summary>
        public long Maximum { get { return this.maximum.GetValueOrDefault(); } set { this.maximum = value; } }
        /// <summary>
        /// Whether this instance has a minimum value specified.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has a minimum value specified, otherwise, <c>false</c>.
        /// </value>
        public bool HasMinimum { get { return this.minimum != null; } }
        /// <summary>
        /// Whether this instance has a maximum value specified.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has a maximum value specified, otherwise, <c>false</c>.
        /// </value>
        public bool HasMaximum { get { return this.maximum != null; } }
    }
}
