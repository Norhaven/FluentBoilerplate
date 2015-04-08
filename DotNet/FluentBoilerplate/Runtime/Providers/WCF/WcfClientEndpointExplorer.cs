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
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Configuration;
using FluentBoilerplate.Providers.WCF;

namespace FluentBoilerplate.Runtime.Providers.WCF
{
    internal sealed class WcfClientEndpointExplorer
    {
        private ClientSection client;
        private IWcfService[] knownServices = new IWcfService[0];

        public bool HasClient { get { return this.client != null; } }
        public IEnumerable<IWcfService> KnownServices { get { return this.knownServices; } }

        public void Load()
        {
            var clientSection = ConfigurationManager.GetSection(@"system.serviceModel\client") as ClientSection;

            if (clientSection == null)
                return;

            this.client = clientSection;
            this.knownServices = WcfClientEndpointExplorer.GetKnownServices(clientSection).ToArray();
        }

        private static IEnumerable<IWcfService> GetKnownServices(ClientSection client)
        {
            if (client == null)
                yield break;

            foreach (var endpoint in client.Endpoints.Cast<ChannelEndpointElement>())
            {
                var contractType = Type.GetType(endpoint.Contract);
                var serviceType = typeof(WcfService<>).MakeGenericType(contractType);
                var service = (IWcfService)Activator.CreateInstance(serviceType, endpoint.Name);

                yield return service;
            }
        }
    }
}
