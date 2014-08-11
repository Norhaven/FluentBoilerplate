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
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentBoilerplate.Runtime.Extensions;
using System.Data.SqlClient;
using System.Data;
using FluentBoilerplate.Providers.Database;

namespace FluentBoilerplate.Runtime.Providers.Database
{
    /// <summary>
    /// Represents an ADO.Net connection
    /// </summary>
    internal sealed class AdoNetConnection : IAdoNetConnection
    {
        private readonly IDataSourceProvider dataSourceProvider;
        private readonly string connectionString;

        /// <summary>
        /// Creates a new instance of the <see cref="AdoNetConnection"/> class.
        /// </summary>
        /// <param name="dataSourceProvider">The data source provider.</param>
        /// <param name="connectionStringName">Name of the connection string.</param>
        /// <exception cref="System.ArgumentException">Connection string name '{0}' could not be found in configuration.WithValues(connectionString)</exception>
        public AdoNetConnection(IDataSourceProvider dataSourceProvider, string connectionStringName)
        {
            var connectionEntry = ConfigurationManager.ConnectionStrings[connectionStringName];
            if (connectionEntry == null)
                throw new ArgumentException("Connection string name '{0}' could not be found in configuration".WithValues(connectionStringName));

            this.dataSourceProvider = dataSourceProvider;
            this.connectionString = connectionEntry.ConnectionString;
        }


        public IDataParameter CreateParameter(string name, object value)
        {
            return this.dataSourceProvider.CreateParameter(name, value);
        }

        /// <summary>
        /// Executes the stored procedure.
        /// </summary>
        /// <param name="name">The stored procedure name.</param>
        /// <param name="parameters">The stored procedure parameters.</param>
        public void ExecuteStoredProcedure(string name, params IDataParameter[] parameters)
        {
            var rowsAffected = OpenCommandAsStoredProcedure(name, parameters, command => command.ExecuteNonQuery());
        }

        /// <summary>
        /// Executes the stored procedure.
        /// </summary>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="name">The stored procedure name.</param>
        /// <param name="interpreter">The interpreter for converting data to a custom type</param>
        /// <param name="parameters">The stored procedure parameters.</param>
        /// <returns>A sequence of instances of the interpreted result.</returns>
        public IEnumerable<TResult> ExecuteStoredProcedure<TResult>(string name, IDataInterpreter<TResult> interpreter, params IDataParameter[] parameters)
        {
            return OpenCommandAsStoredProcedure(name, parameters, command => interpreter.Interpret(command.ExecuteReader()).ToArray());
        }

        private TResult OpenCommandAsStoredProcedure<TResult>(string name, IDataParameter[] parameters, Func<IDbCommand, TResult> useCommand)
        {
            using (var connection = this.dataSourceProvider.CreateConnection(this.connectionString))
            using (var command = this.dataSourceProvider.CreateCommand(name, connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                if (parameters != null)
                {
                    foreach (var parameter in parameters)
                        command.Parameters.Add(parameter);
                }

                connection.Open();
                return useCommand(command);
            }
        }
    }
}
