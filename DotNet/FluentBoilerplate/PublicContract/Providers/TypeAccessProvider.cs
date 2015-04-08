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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentBoilerplate.Runtime.Extensions;
using FluentBoilerplate.Runtime.Providers;
using FluentBoilerplate.Providers.WCF;

namespace FluentBoilerplate.Providers
{
    /// <summary>
    /// Represents a simple implementation of a usable type access provider
    /// </summary>
    public sealed class TypeAccessProvider:TypeAccessProviderBase
    {
        private sealed class EmptyProvider : TypeAccessProviderBase
        {
            public EmptyProvider() : base(PermissionsProvider.Default, Type.EmptyTypes) { }

            protected override void Use<TType>(Action<TType> action) { throw new InvalidOperationException("Should never be able to attempt to use a type with an empty type provider"); }
            protected override TResult Use<TType, TResult>(Func<TType, TResult> action) { throw new InvalidOperationException("Should never be able to attempt to use a type with an empty type provider"); }
            public override ITypeAccessProvider AddProvider(ITypeProvider provider) { throw new InvalidOperationException("Should never be able to add a provider with an empty type provider"); }
        }

        /// <summary>
        /// Gets an empty type access provider
        /// </summary>
        public static ITypeAccessProvider Empty { get { return new EmptyProvider(); } }

        /// <summary>
        /// Gets a type access provider for WCF connections.
        /// Uses the default <see cref="IPermissionsProvider"/> to determine permissions
        /// and will attempt to inspect the &lt;system.serviceModel&gt;&lt;client&gt; configuration section
        /// to determine which endpoints are providable.
        /// </summary>
        public static ITypeAccessProvider WcfAccessProvider { get { return new TypeAccessProvider(PermissionsProvider.Default, new WcfConnectionProvider()); } }

        private readonly ITypeProvider[] typeProviders;

        /// <summary>
        /// Creates a new instance of the <see cref="TypeAccessProvider"/> class.
        /// </summary>
        /// <param name="typeProviders">The type providers that may be accessed</param>
        public TypeAccessProvider(params ITypeProvider[] typeProviders)
            :this(PermissionsProvider.Default, typeProviders)
        {

        }

        /// <summary>
        /// Creates a new instance of the <see cref="TypeAccessProvider"/> class.
        /// Uses the given <see cref="IPermissionsProvider"/> to determine permissions.
        /// </summary>
        /// <param name="permissionsProvider">The permissions provider</param>
        /// <param name="typeProviders">The type providers that may be accessed</param>
        public TypeAccessProvider(IPermissionsProvider permissionsProvider, params ITypeProvider[] typeProviders)
            :base(permissionsProvider, typeProviders.AggregateProvidableTypes())
        {
            this.typeProviders = typeProviders;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="TypeAccessProvider"/> class.
        /// Uses the default permissions provider and, optionally, an extended provider for greater permissions capabilities.
        /// </summary>
        /// <param name="extendedProvider">The extended permissions provider that should be used</param>
        /// <param name="typeProviders">The type providers that may be accessed</param>
        public TypeAccessProvider(ExtendedPermissionsProviders extendedProvider = ExtendedPermissionsProviders.None, params ITypeProvider[] typeProviders)
            : base(GetExtendedPermissionsProvider(extendedProvider), typeProviders.AggregateProvidableTypes())
        {
            this.typeProviders = typeProviders;
        }
        
        private static IPermissionsProvider GetExtendedPermissionsProvider(ExtendedPermissionsProviders provider)
        {
            switch(provider)
            {
                //TODO: Include after they're solid
                //case ExtendedPermissionsProviders.ActiveDirectoryApplicationDirectory: return PermissionsProvider.ActiveDirectoryApplicationDirectory;
                //case ExtendedPermissionsProviders.ActiveDirectoryDomain: return PermissionsProvider.ActiveDirectoryDomain;
                case ExtendedPermissionsProviders.ActiveDirectoryMachine: return PermissionsProvider.IncludingActiveDirectoryMachine;
                default:
                    return PermissionsProvider.Default;
            }
        }

        private ITypeProvider LocateProviderFor<TType>()
        {
            foreach (var provider in this.typeProviders)
            {
                if (provider.ProvidableTypes.Contains(typeof(TType)))
                {
                    return provider;
                }
            }

            return null;
        }

        /// <summary>
        /// Uses the accessible type
        /// </summary>
        /// <typeparam name="TType">The requested type</typeparam>
        /// <param name="action">How the type will be used</param>
        protected override void Use<TType>(Action<TType> action)
        {
            var provider = LocateProviderFor<TType>();
            provider.Use(action);
        }

        /// <summary>
        /// Uses the accessible type
        /// </summary>
        /// <typeparam name="TType">The requested type</typeparam>
        /// <typeparam name="TResult">The result type</typeparam>
        /// <param name="action">How the type will be used</param>
        /// <returns>The result</returns>
        protected override TResult Use<TType, TResult>(Func<TType, TResult> action)
        {
            var provider = LocateProviderFor<TType>();
            return provider.Use(action);
        }

        public override ITypeAccessProvider AddProvider(ITypeProvider provider)
        {
            var elevatedProviders = this.typeProviders.Add(provider);
            return new TypeAccessProvider(this.permissionsProvider, elevatedProviders);
        }
    }
}
