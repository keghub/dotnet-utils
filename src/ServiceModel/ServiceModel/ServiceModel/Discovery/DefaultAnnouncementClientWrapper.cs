using System;
using System.ServiceModel;
using System.ServiceModel.Discovery;

namespace EMG.Utilities.ServiceModel.Discovery
{
    public class DefaultAnnouncementClientWrapper : IAnnouncementClientWrapper
    {
        private readonly AnnouncementClient _client;

        public DefaultAnnouncementClientWrapper(AnnouncementClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public void AnnounceOnline(EndpointDiscoveryMetadata metadata)
        {
            _client.AnnounceOnline(metadata);
        }

        public void AnnounceOffline(EndpointDiscoveryMetadata metadata)
        {
            _client.AnnounceOffline(metadata);
        }

        public void Dispose()
        {
            var communicationObject = _client as ICommunicationObject;

            if (communicationObject.State != CommunicationState.Faulted)
            {
                ((IDisposable)_client).Dispose();
            }
            else 
            {
                communicationObject.Abort();
            }
        }
    }
}