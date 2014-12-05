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
    /// Represents an atomic storage location.
    /// </summary>
    /// <typeparam name="T">The type of data that this storage location contains.</typeparam>
    public sealed class Atomic<T>
    {
        public static implicit operator Atomic<T>(T instance)
        {
            return new Atomic<T>(instance);
        }

        public static implicit operator T(Atomic<T> instance)
        {
            return instance.Value;
        }
        
        private T instance;
        private readonly object instanceLock = new object();

        /// <summary>
        /// Gets or sets the value atomically.
        /// </summary>
        public T Value
        {
            get { lock (this.instanceLock) return instance; }
            set { lock (this.instanceLock) this.instance = value; }
        }

        internal Atomic(T instance)
        {
            this.instance = instance;
        }
    }
}
