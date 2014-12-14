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
    /// Represents a builder for a transaction's lock order.
    /// </summary>
    /// <typeparam name="TParent">The type of the parent.</typeparam>
    public interface IAtomicOperationOrderByBuilder<TParent>
    {
        /// <summary>
        /// Use the default lock ordering. This leaves it up to the boilerplate to determine the correct order.
        /// </summary>
        TParent Default { get; }

        /// <summary>
        /// Use the parameter order to define the lock order. This leaves it entirely up to the caller to
        /// add the atomic variables to the transaction in the exact order that they should be locked.
        /// </summary>
        TParent ParameterOrder { get; }
    }
}
