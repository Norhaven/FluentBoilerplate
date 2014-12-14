﻿/*
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

namespace FluentBoilerplate.Runtime.Extensions
{
    internal static class ArrayExtensions
    {
        public static T[] Add<T>(this T[] array, T item)
        {
            if (array == null)
                return new T[] { item };

            var newLength = array.Length + 1;
            var newArray = new T[newLength];
            Array.Copy(array, newArray, array.Length);
            newArray[newLength - 1] = item;
            return newArray;
        }

        public static bool HasContents<T>(this T[] array)
        {
            if (array == null)
                return false;

            if (array.Length == 0)
                return false;

            return true;
        }

        public static T[] DefaultIfNull<T>(this T[] array)
        {
            if (array == null)
                return new T[0];
            return array;
        }
    }
}
