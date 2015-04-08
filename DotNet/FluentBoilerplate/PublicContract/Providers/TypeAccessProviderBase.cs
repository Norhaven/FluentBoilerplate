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

using FluentBoilerplate.Providers;
using FluentBoilerplate.Runtime;
using FluentBoilerplate.Runtime.Providers;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBoilerplate.Providers
{
    /// <summary>
    /// Represents a type access provider that may be extended to implement a custom type access provider
    /// </summary>
    public abstract class TypeAccessProviderBase:ITypeAccessProvider
    {        
        protected readonly IPermissionsProvider permissionsProvider;
        protected readonly IImmutableSet<Type> types;

        /// <summary>
        /// Creates a new instance of the <see cref="TypeAccessProviderBase"/> class
        /// </summary>
        /// <param name="permissionsProvider">The permissions provider</param>
        /// <param name="types">The types that may be provided</param>
        public TypeAccessProviderBase(IPermissionsProvider permissionsProvider, IEnumerable<Type> types)
        {
            this.permissionsProvider = permissionsProvider;

            if (types == null)
                this.types = new HashSet<Type>().ToImmutableHashSet();
            this.types = types.ToImmutableHashSet();
        }

        /// <summary>
        /// Uses the accessible type
        /// </summary>
        /// <typeparam name="TType">The requested type</typeparam>
        /// <param name="action">How the type will be used</param>
        protected abstract void Use<TType>(Action<TType> action);
        
        /// <summary>
        /// Uses the accessible type
        /// </summary>
        /// <typeparam name="TType">The requested type</typeparam>
        /// <typeparam name="TResult">The result type</typeparam>
        /// <param name="action">How the type will be used</param>
        /// <returns>The result</returns>
        protected abstract TResult Use<TType, TResult>(Func<TType, TResult> action);

        private bool VerifyPermissions(IIdentity identity)
        {
            if (this.permissionsProvider == null)
                return false;

            return this.permissionsProvider.HasPermission(identity);
        }

        /// <summary>
        /// Tries to access an instance of the requested type
        /// </summary>
        /// <typeparam name="TType">The requested type</typeparam>
        /// <typeparam name="TResult">The result type</typeparam>
        /// <param name="identity">The identity</param>
        /// <param name="useType">How the type will be used if it can be accessed</param>
        /// <returns>A response containing information about the access attempt and the result</returns>
        public IResponse<TResult> TryAccess<TType, TResult>(IIdentity identity, Func<TType, TResult> useType)
        {
            if (!VerifyPermissions(identity))
                return Response<TResult>.Failed;

            if (!this.types.Contains(typeof(TType)))
                return Response<TResult>.Failed;

            var result = Use<TType, TResult>(useType);
            return new Response<TResult>(result);
        }

        /// <summary>
        /// Tries to access an instance of the requested type
        /// </summary>
        /// <typeparam name="TType">The requested type</typeparam>
        /// <param name="identity">The identity</param>
        /// <param name="useType">How the type will be used if it can be accessed</param>
        /// <returns>A response containing information about the access attempt</returns>
        public IResponse TryAccess<TType>(IIdentity identity, Action<TType> useType)
        {
            if (!VerifyPermissions(identity))
                return Response.Failed;

            if (!this.types.Contains(typeof(TType)))
                return Response.Failed;

            Use<TType>(useType);
            return new Response(true);
        }

        public abstract ITypeAccessProvider AddProvider(ITypeProvider provider);
    }
}
