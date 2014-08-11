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
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBoilerplate.Providers.Database
{
    /// <summary>
    /// Represents an ADO.Net connection
    /// </summary>
    public interface IAdoNetConnection
    {
        /// <summary>
        /// Creates a data parameter.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="value">The parameter value.</param>
        /// <returns>An instance of <see cref="IDataParameter"/></returns>
        IDataParameter CreateParameter(string name, object value);

        /// <summary>
        /// Executes the stored procedure.
        /// </summary>
        /// <param name="name">The stored procedure name.</param>
        /// <param name="parameters">The stored procedure parameters.</param>
        void ExecuteStoredProcedure(string name, params IDataParameter[] parameters);

        /// <summary>
        /// Executes the stored procedure.
        /// </summary>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="name">The stored procedure name.</param>
        /// <param name="interpreter">The interpreter for converting data to a custom type.</param>
        /// <param name="parameters">The stored procedure parameters.</param>
        /// <returns>A sequence of instances of the interpreted result.</returns>
        IEnumerable<TResult> ExecuteStoredProcedure<TResult>(string name, IDataInterpreter<TResult> interpreter, params IDataParameter[] parameters);
    }
}
