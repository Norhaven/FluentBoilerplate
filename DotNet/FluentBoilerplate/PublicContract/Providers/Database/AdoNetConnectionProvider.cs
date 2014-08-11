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

using FluentBoilerplate.Exceptions;
using FluentBoilerplate.Runtime.Providers.Database;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBoilerplate.Providers.Database
{
    /// <summary>
    /// Represents a type provider for ADO.Net connections
    /// </summary>
    public sealed class AdoNetConnectionProvider:ITypeProvider
    {
        private readonly IImmutableSet<Type> providableTypes = new HashSet<Type> { typeof(IAdoNetConnection) }.ToImmutableHashSet();
        private readonly IDictionary<Type, IAdoNetConnection> types;
        private readonly DataSource dataSource;

        /// <summary>
        /// Gets the providable types.
        /// </summary>
        public IImmutableSet<Type> ProvidableTypes { get { return this.providableTypes; } }

        /// <summary>
        /// Creates a new instance of the <see cref="AdoNetConnectionProvider"/> class.
        /// </summary>
        /// <param name="connectionStringName">Name of the connection string.</param>
        /// <param name="dataSource">The kind of data source that will be connected to</param>
        public AdoNetConnectionProvider(string connectionStringName, DataSource dataSource)
        {
            this.dataSource = dataSource;
            this.types = new Dictionary<Type, IAdoNetConnection>
            {
                { typeof(IAdoNetConnection), CreateAdoNetConnection(connectionStringName, this.dataSource) }
            }.ToImmutableDictionary();
        }

        /// <summary>
        /// Attempts to use the specified type.
        /// </summary>
        /// <typeparam name="TType">The requested type</typeparam>
        /// <param name="useType">How the requested type will be used</param>
        public void Use<TType>(Action<TType> useType)
        {
            VerifyTypeIsProvidable<TType>();

            var connection = (TType)this.types[typeof(TType)];

            useType(connection);
        }

        /// <summary>
        /// Attempts to use the specified type.
        /// </summary>
        /// <typeparam name="TType">The requested type.</typeparam>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="useType">How the requested type will be used.</param>
        /// <returns>An instance of the result</returns>
        public TResult Use<TType, TResult>(Func<TType, TResult> useType)
        {
            VerifyTypeIsProvidable<TType>();

            var connection = (TType)this.types[typeof(TType)];

            return useType(connection);
        }

        private void VerifyTypeIsProvidable<TType>()
        {
            var type = typeof(TType);
            if (!this.types.ContainsKey(type))
                throw new TypeNotFoundException(type);
        }

        private IAdoNetConnection CreateAdoNetConnection(string connectionStringName, DataSource dataSource)
        {
            var dataSourceProvider = new DataSourceProvider(dataSource);
            return new AdoNetConnection(dataSourceProvider, connectionStringName);
        }
    }
}
