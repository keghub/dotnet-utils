using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Discovery;

namespace EMG.Utilities.ServiceModel.Discovery
{
    public class DefaultAnnouncementClientWrapperFactory : IAnnouncementClientWrapperFactory
    {
        public IAnnouncementClientWrapper Create(Uri registryUrl, Binding binding)
        {
            var endpointAddress = new EndpointAddress(registryUrl);
            var endpoint = new AnnouncementEndpoint(binding, endpointAddress);
            var client = new AnnouncementClient(endpoint);

            var wrapper = new DefaultAnnouncementClientWrapper(client);

            return wrapper;
        }
    }
}