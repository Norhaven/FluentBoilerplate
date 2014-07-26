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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FluentBoilerplate.Runtime.Extensions
{
    internal static class PropertyInfoExtensions
    {
        public static IEnumerable<T> GetAttributesOf<T>(this PropertyInfo property)
        {
            return property.GetCustomAttributes(typeof(T), true).Cast<T>();
        }

        public static IEnumerable<Tuple<PropertyInfo, MapsToAttribute>> GetTranslatableProperties<TTo>(this IEnumerable<PropertyInfo> properties)
        {
            var translatable = (from p in properties
                                let attributes = p.GetCustomAttributes<MapsToAttribute>()
                                where attributes.Any()
                                select new { Property = p, Attributes = attributes });

            var translatableToType = (from p in translatable
                                      let relatedAttributes = (from a in p.Attributes
                                                               where a.MappedType == typeof(TTo)
                                                               select a).ToArray()
                                      where relatedAttributes.Any()
                                      select new { Property = p.Property, Attributes = relatedAttributes });

            foreach (var info in translatableToType)
            {
                var attribute = info.Attributes.First();
                yield return new Tuple<PropertyInfo, MapsToAttribute>(info.Property, attribute);
            }
        }
    }
}
