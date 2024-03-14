using Microsoft.Extensions.Logging;
using System;
using System.ServiceModel;
using System.ServiceModel.Discovery;

namespace EMG.Utilities.ServiceModel.Discovery
{
    public class DefaultAnnouncementClientWrapper : IAnnouncementClientWrapper
    {
        private readonly AnnouncementClient _client;
        private readonly ILogger<DefaultAnnouncementClientWrapper> _logger;

        public DefaultAnnouncementClientWrapper(AnnouncementClient client, ILogger<DefaultAnnouncementClientWrapper> logger)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

            try
            {
                if (communicationObject.State != CommunicationState.Faulted)
                {
                    ((IDisposable)_client).Dispose();
                }
                else
                {
                    communicationObject.Abort();
                }
            }
            catch (Exception ex)
            {
                communicationObject.Abort();
               
                _logger.LogError(ex, "An error occurred while disposing the announcement client");
            }
        }
    }
}