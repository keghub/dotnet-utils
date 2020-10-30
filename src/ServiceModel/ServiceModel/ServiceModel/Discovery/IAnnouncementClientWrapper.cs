using System;
using System.ServiceModel.Discovery;

namespace EMG.Utilities.ServiceModel.Discovery
{
    public interface IAnnouncementClientWrapper : IDisposable
    {
        public void AnnounceOnline(EndpointDiscoveryMetadata metadata);

        public void AnnounceOffline(EndpointDiscoveryMetadata metadata);
    }
}