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
