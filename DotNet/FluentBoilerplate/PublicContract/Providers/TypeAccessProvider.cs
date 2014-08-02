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
    public abstract class TypeAccessProvider:ITypeAccessProvider
    {
        private sealed class EmptyProvider : TypeAccessProvider
        {
            public EmptyProvider() : base(PermissionsProvider.Empty, Type.EmptyTypes) { }

            protected override void Use<TType>(Action<TType> action) { throw new InvalidOperationException("Should never be able to attempt to use a type with an empty type provider"); }
            protected override TResult Use<TType, TResult>(Func<TType, TResult> action) { throw new InvalidOperationException("Should never be able to attempt to use a type with an empty type provider"); }
        }

        public static ITypeAccessProvider Empty { get { return new EmptyProvider(); } }

        protected readonly IPermissionsProvider permissionsProvider;
        protected readonly IImmutableSet<Type> types;

        public TypeAccessProvider(IPermissionsProvider permissionsProvider, IEnumerable<Type> types)
        {
            this.permissionsProvider = permissionsProvider;

            if (types == null)
                this.types = new HashSet<Type>().ToImmutableHashSet();
            this.types = types.ToImmutableHashSet();
        }

        protected abstract void Use<TType>(Action<TType> action);
        protected abstract TResult Use<TType, TResult>(Func<TType, TResult> action);

        private bool VerifyPermissions(IIdentity identity)
        {
            if (this.permissionsProvider == null)
                return false;

            return this.permissionsProvider.HasPermission(identity);
        }
        public IResponse<TResult> TryAccess<TType, TResult>(IIdentity identity, Func<TType, TResult> useType)
        {
            if (!VerifyPermissions(identity))
                return Response<TResult>.Failed;

            if (!this.types.Contains(typeof(TType)))
                return Response<TResult>.Failed;

            var result = Use<TType, TResult>(useType);
            return new Response<TResult>(result);
        }

        public IResponse TryAccess<TType>(IIdentity identity, Action<TType> useType)
        {
            if (!VerifyPermissions(identity))
                return Response.Failed;

            if (!this.types.Contains(typeof(TType)))
                return Response.Failed;

            Use<TType>(useType);
            return new Response(true);
        }
    }
}
