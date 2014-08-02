using FluentBoilerplate.Providers.WCF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace FluentBoilerplate.Runtime.Providers.WCF
{
    internal sealed class ChannelClient<TService> : ClientBase<TService>, IWcfClient where TService:class
    {
        IChannel IWcfClient.Channel { get { return base.InnerChannel; } }

        public ChannelClient(string endpointName) : base(endpointName) { }
        public ChannelClient(Binding binding, EndpointAddress endpointAddress) : base(binding, endpointAddress) { }
    }
}
