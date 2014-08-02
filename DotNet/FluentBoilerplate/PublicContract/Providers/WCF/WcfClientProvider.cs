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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using FluentBoilerplate.Exceptions;
using System.ServiceModel.Channels;
using System.Collections.Immutable;

namespace FluentBoilerplate.Providers.WCF
{
    public sealed class WcfClientProvider:ITypeProvider
    {
        private readonly IImmutableDictionary<Type, IWcfService> services;
        private readonly IImmutableSet<Type> providableTypes;

        public IImmutableSet<Type> ProvidableTypes { get { return this.providableTypes; } }

        public WcfClientProvider(IEnumerable<IWcfService> services)
        {
            this.services = services.ToImmutableDictionary(service => service.ServiceType);
            this.providableTypes = this.services.Keys.ToImmutableHashSet();
        }
        
        private void UseChannel<TChannel>(TChannel channel, Action<TChannel> useChannel)
        {
            try
            {
                useChannel(channel);
            }
            finally
            {
                var actualChannel = (IChannel)channel;
                ((IChannel)channel).Close();
            }
        }

        private void VerifyTypeIsProvidable<TType>()
        {
            var type = typeof(TType);
            if (!this.services.ContainsKey(type))
                throw new ServiceProviderNotFoundException(type);
        }

        public void Use<TType>(Action<TType> useType)
        {
            VerifyTypeIsProvidable<TType>();

            var serviceProvider = this.services[typeof(TType)];
            using (var serviceChannel = serviceProvider.OpenClient())
            {
                var typedChannel = (TType)serviceChannel.Channel;
                UseChannel(typedChannel, useType);
            }
        }
        
        public TResult Use<TType, TResult>(Func<TType, TResult> useType)
        {
            VerifyTypeIsProvidable<TType>();

            var serviceProvider = this.services[typeof(TType)];
            using (var serviceChannel = serviceProvider.OpenClient())
            {
                var typedChannel = (TType)serviceChannel.Channel;
                TResult result = default(TResult);
                UseChannel(typedChannel, service => result = useType(service));
                return result;
            }
        }
    }
}
