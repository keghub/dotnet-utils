using System;
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
            ((IDisposable)_client).Dispose();
        }
    }
}