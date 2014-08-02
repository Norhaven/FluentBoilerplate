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
    public class WcfServiceClient<TService>: IWcfService where TService:class
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

        public Type ServiceType { get { return typeof(TService); } }

        public WcfServiceClient(string endpointName)
        {
            this.endpointName = endpointName;
            this.initializationKind = WcfInitializationKind.EndpointName;
        }

        public WcfServiceClient(Binding binding, EndpointAddress endpointAddress)
        {
            this.binding = binding;
            this.endpointAddress = endpointAddress;
            this.initializationKind = WcfInitializationKind.BindingAndEndpointAddress;
        }

        public IWcfClient OpenClient()
        {
            switch(this.initializationKind)
            {
                case WcfInitializationKind.EndpointName: return new ChannelClient<TService>(this.endpointName);
                case WcfInitializationKind.BindingAndEndpointAddress: return new ChannelClient<TService>(this.binding, this.endpointAddress);
                default:
                    throw new ArgumentException("Encountered unhandled kind of WCF initialization");
            }
        }
    }
}
