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

namespace FluentBoilerplate.Runtime.Providers.Database
{
    /// <summary>
    /// Represents a provider of database interactivity for a data source.
    /// </summary>
    internal interface IDataSourceProvider
    {
        /// <summary>
        /// Creates the parameter.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="value">The parameter value.</param>
        /// <returns>An instance of <see cref="IDataParameter"/></returns>
        IDataParameter CreateParameter(string name, object value);

        /// <summary>
        /// Creates the data source connection.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>An instance of the data source connection.</returns>
        IDbConnection CreateConnection(string connectionString);

        /// <summary>
        /// Creates the data source command.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="connection">The connection.</param>
        /// <returns>An instance of the data source command.</returns>
        IDbCommand CreateCommand(string commandText, IDbConnection connection);
    }
}
