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

using FluentBoilerplate.Runtime.Providers.WCF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace FluentBoilerplate.Providers.WCF
{
    /// <summary>
    /// Represents a WCF service
    /// </summary>
    /// <typeparam name="TService">The service contract type</typeparam>
    public sealed class WcfService<TService>: IWcfService where TService:class
    {   
        private enum WcfInitializationKind
        {
            EndpointName,
            BindingAndEndpointAddress
        }

        private readonly WcfInitializationKind initializationKind;
        private readonly string endpointName;
        private readonly Binding binding;
        private readonly EndpointAddress endpointAddress;

        /// <summary>
        /// Gets the service type
        /// </summary>
        public Type ServiceType { get { return typeof(TService); } }
        
        /// <summary>
        /// Creates a new instance of the <see cref="WcfService{TService}"/> class.
        /// </summary>
        /// <param name="endpointName">The name of the endpoint</param>
        public WcfService(string endpointName)
        {
            this.endpointName = endpointName;
            this.initializationKind = WcfInitializationKind.EndpointName;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="WcfService{TService}"/> class.
        /// </summary>
        /// <param name="binding">The binding for the service</param>
        /// <param name="endpointAddress">The endpoint address</param>
        public WcfService(Binding binding, EndpointAddress endpointAddress)
        {
            this.binding = binding;
            this.endpointAddress = endpointAddress;
            this.initializationKind = WcfInitializationKind.BindingAndEndpointAddress;
        }

        /// <summary>
        /// Opens a client connection to this service
        /// </summary>
        /// <returns>An instance of the opened client connection</returns>
        public IWcfConnection OpenClient()
        {
            var client = CreateClient();
            client.Open();
            return client;
        }

        private ChannelClient<TService> CreateClient()
        {
            switch (this.initializationKind)
            {
                case WcfInitializationKind.EndpointName: return new ChannelClient<TService>(this.endpointName);
                case WcfInitializationKind.BindingAndEndpointAddress: return new ChannelClient<TService>(this.binding, this.endpointAddress);
                default:
                    throw new ArgumentException("Encountered unhandled kind of WCF initialization");
            }
        }
    }
}
