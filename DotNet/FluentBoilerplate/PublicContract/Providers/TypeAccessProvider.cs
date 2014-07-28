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
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBoilerplate.PublicContract.Providers
{
    public abstract class TypeAccessProvider:ITypeAccessProvider
    {
        protected readonly IPermissionsProvider permissionsProvider;
        protected readonly IImmutableSet<Type> types;

        public TypeAccessProvider(IPermissionsProvider permissionsProvider, IEnumerable<Type> types)
        {
            this.permissionsProvider = permissionsProvider;

            if (types == null)
                this.types = new HashSet<Type>().ToImmutableHashSet();
            this.types = types.ToImmutableHashSet();
        }

        protected abstract TType CreateInstanceOf<TType>();

        public IResponse<TResult> TryAccess<TType, TResult>(IIdentity identity, Func<TType, TResult> useType)
        {
            if (this.permissionsProvider != null)
            {
                var success = this.permissionsProvider.HasPermission(identity);
                if (!success)
                    return Response<TResult>.Failed;
            }

            if (!this.types.Contains(typeof(TType)))
                return Response<TResult>.Failed;

            var instance = CreateInstanceOf<TType>();
            var result = useType(instance);
            return new Response<TResult>(result);
        }

        public IResponse TryAccess<TType>(IIdentity identity, Action<TType> useType)
        {
            if (this.permissionsProvider != null)
            {
                var success = this.permissionsProvider.HasPermission(identity);
                if (!success)
                    return Response.Failed;
            }

            if (!this.types.Contains(typeof(TType)))
                return Response.Failed;

            var instance = CreateInstanceOf<TType>();
            useType(instance);
            return new Response(true);
        }
    }
}
