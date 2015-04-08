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
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBoilerplate.Providers.Database
{
    /// <summary>
    /// Represents a way of interpreting data as a custom type.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    public interface IDataInterpreter<TResult>
    {
        /// <summary>
        /// Interprets the specified data reader.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        /// <returns>An instance of the interpreted result.</returns>
        IEnumerable<TResult> Interpret(IDataReader dataReader);
    }
}
