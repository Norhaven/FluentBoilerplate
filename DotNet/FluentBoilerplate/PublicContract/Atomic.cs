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

using FluentBoilerplate.Exceptions;
using FluentBoilerplate.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FluentBoilerplate
{
    /// <summary>
    /// Represents an atomic storage location.
    /// </summary>
    /// <typeparam name="T">The type of data that this storage location contains.</typeparam>
    public sealed class Atomic<T> : ILockTransactionMember
    {
        //The reason that we're only providing built in conversions away from Atomic<T> is that
        //we get into a difficult spot with assignment. Assigning to a newly declared atomic will
        //create a new Atomic<T> to store it in, but the same thing happens when assigning to
        //a precreated storage location. Every atomic location has a specific instance used for
        //locking as well as a unique ID to help determine lock order. Allowing the developer
        //to nuke their lock and ID while thinking that they're assigning to the
        //same atomic storage location should not ever be allowed.

        public static implicit operator T(Atomic<T> instance)
        {
            return instance.Value;
        }

        public static Atomic<T> New(T instance)
        {
            return new Atomic<T>(instance);
        }

        private readonly Guid atomicId = Guid.NewGuid();
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

        Guid ILockTransactionMember.Id
        {
            get { return this.atomicId; }
        }

        internal Atomic(T instance)
        {
            this.instance = instance;
        }

        void ILockTransactionMember.AcquireLock()
        {
            var acquired = Monitor.TryEnter(this.instanceLock);

            if (!acquired)
                throw new InvalidLockTransactionStateException("Lock transaction could not acquire a lock");
        }

        void ILockTransactionMember.ReleaseLock()
        {
            if (!Monitor.IsEntered(this.instanceLock))
                throw new InvalidLockTransactionStateException("Lock transaction attempted to release a lock that wasn't acquired");
            Monitor.Exit(this.instanceLock);
        }

        public override bool Equals(object obj)
        {
            if (Object.ReferenceEquals(this, obj))
                return true;

            if (obj is ILockTransactionMember)
            {
                var member = (ILockTransactionMember)obj;
                return this.atomicId == member.Id;
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ this.atomicId.GetHashCode();
        }
    }
}
