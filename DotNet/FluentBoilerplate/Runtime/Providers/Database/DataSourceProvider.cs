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

using FluentBoilerplate.Providers.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentBoilerplate.Runtime.Extensions;

namespace FluentBoilerplate.Runtime.Providers.Database
{
    internal sealed class DataSourceProvider:IDataSourceProvider
    {
        private readonly DataSource dataSource;

        public DataSourceProvider(DataSource dataSource)
        {
            this.dataSource = dataSource;    
        }
        
        public IDataParameter CreateParameter(string name, object value)
        {
            switch(this.dataSource)
            {
                case DataSource.ODBC: return new OdbcParameter(name, value);
                case DataSource.OleDb: return new OleDbParameter(name, value);
                case DataSource.SQL: return new SqlParameter(name, value);
                default:
                    throw new DataException("The data source '{0}' is not know, cannot create a parameter".WithValues(this.dataSource));
            }
        }

        public IDbConnection CreateConnection(string connectionString)
        {
            switch(this.dataSource)
            {
                case DataSource.ODBC: return new OdbcConnection(connectionString);
                case DataSource.OleDb: return new OleDbConnection(connectionString);
                case DataSource.SQL: return new SqlConnection(connectionString);
                default:
                    throw new DataException("The data source '{0}' is not known, cannot create a connection".WithValues(this.dataSource));
            }
        }

        public IDbCommand CreateCommand(string name, IDbConnection connection)
        {
            IDbCommand command = null;

            switch(this.dataSource)
            {
                case DataSource.ODBC: command =  new OdbcCommand(name); break;
                case DataSource.OleDb: command = new OleDbCommand(name); break;                    
                case DataSource.SQL: command = new SqlCommand(name); break;
                default:
                    throw new DataException("The data source '{0}' is not known, cannot create a command".WithValues(this.dataSource));                    
            }

            command.Connection = connection;
            return command;
        }
    }
}
