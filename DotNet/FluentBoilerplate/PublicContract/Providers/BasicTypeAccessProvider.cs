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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentBoilerplate.Runtime.Extensions;

namespace FluentBoilerplate.Providers
{
    public sealed class BasicTypeAccessProvider:TypeAccessProvider
    {
        private readonly ITypeProvider[] typeProviders;

        public BasicTypeAccessProvider(IPermissionsProvider permissionsProvider, IEnumerable<ITypeProvider> typeProviders)
            :base(permissionsProvider, typeProviders.AggregateProvidableTypes())
        {
            this.typeProviders = typeProviders.ToArray();
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

        protected override void Use<TType>(Action<TType> action)
        {
            var provider = LocateProviderFor<TType>();
            provider.Use(action);
        }

        protected override TResult Use<TType, TResult>(Func<TType, TResult> action)
        {
            var provider = LocateProviderFor<TType>();
            return provider.Use(action);
        }
    }
}
