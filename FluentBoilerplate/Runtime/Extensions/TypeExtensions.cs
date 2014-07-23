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

using FluentBoilerplate.Messages;
using FluentBoilerplate.Messages.Developer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FluentBoilerplate.Runtime.Extensions
{
    public static class TypeExtensions
    {
        public static PropertyInfo[] GetPublicPropertiesWithGetter(this Type type)
        {
            return type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
        }

        public static PropertyInfo[] GetPublicPropertiesWithSetter(this Type type)
        {
            return type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty);
        }

        public static PropertyInfo[] GetPublicTwoWayProperties(this Type type)
        {
            Debug.Assert(type != null, AssertFailures.InstanceShouldNotBeNull.WithValues("type"));

            return type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty);
        }

        public static IEnumerable<T> GetAttributesOf<T>(this Type type)
        {
            return type.GetCustomAttributes(typeof(T), true).Cast<T>();
        }        

        public static bool CanBe(this Type type, Type targetType)
        {
            return targetType.IsAssignableFrom(type);
        }

        public static bool HasConversionTo(this Type type, Type targetType)
        {
            return type.HasInherentConversionTo(targetType) ||
                   type.HasImplicitConversionTo(targetType) ||
                   type.HasExplicitConversionTo(targetType);
        }

        public static bool HasImplicitConversionTo<T>(this Type type)
        {
            return type.HasImplicitConversionTo(typeof(T));
        }
        public static bool HasImplicitConversionTo(this Type type, Type targetType)
        {
            var conversion = type.GetMethod("op_Implicit", new[] { type, targetType });
            return conversion != null;
        }
        public static bool HasExplicitConversionTo(this Type type, Type targetType)
        {
            var conversion = type.GetMethod("op_Explicit", new[] { type, targetType });
            return conversion != null;
        }
        public static bool HasExplicitConversionTo<T>(this Type type)
        {
            return type.HasExplicitConversionTo(typeof(T));
        }

        public static bool HasInherentConversionTo(this Type type, Type targetType)
        {
            if (!type.IsPrimitive)
                return false;

            if (type.IsInterface && targetType.IsInterface)
            {
                return type.CanBe(targetType);
            }

            if (type.IsValueType ^ targetType.IsValueType)
                return false;

            if (type.IsValueType)
            {
                if (type == typeof(short))
                {
                    if (targetType == typeof(short)) return true;
                    if (targetType == typeof(int)) return true;
                    if (targetType == typeof(long)) return true;
                }
                else if (type == typeof(int))
                {
                    if (targetType == typeof(int)) return true;
                    if (targetType == typeof(long)) return true;
                }
                else if (type == typeof(long))
                {
                    if (targetType == typeof(long)) return true;
                }
            }

            return false;
        }     
        
    }
}
