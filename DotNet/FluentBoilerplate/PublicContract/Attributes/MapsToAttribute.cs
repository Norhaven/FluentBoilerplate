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
    /// Indicates that a property has an explicit mapping to a property on a given type.
    /// This overrides the implicit mapping that uses the property name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class MapsToAttribute:Attribute
    {
        /// <summary>
        /// The type that this mapping points to
        /// </summary>
        public Type MappedType { get; private set; }
        /// <summary>
        /// The name of the property on the mapped type that this property explicitly maps to
        /// </summary>
        public string PropertyName { get; private set; }

        /// <summary>
        /// Creates an instance of the <see cref="MapsToAttribute"/> type.
        /// </summary>
        /// <param name="mappedType">The type that this mapping points to</param>
        /// <param name="propertyName">The name of the property on the mapped type that this property explicitly maps to</param>
        public MapsToAttribute(Type mappedType, string propertyName)
        {
            this.MappedType = mappedType;
            this.PropertyName = propertyName;
        }
    }
}
