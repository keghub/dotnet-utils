using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Discovery;
using EMG.Utilities.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EMG.Utilities.ServiceModel.Discovery
{
    public class AnnouncementServiceOptions
    {
        public bool IsAnnouncementEnabled { get; set; }

        public Uri RegistryUri { get; set; }

        public TimeSpan Interval { get; set; }

        public Binding Binding { get; set; }
    }

    public class AnnouncementService : IAnnouncementService
    {
        private readonly ILogger<AnnouncementService> _logger;
        private readonly AnnouncementServiceOptions _options;

        public AnnouncementService(IOptions<AnnouncementServiceOptions> options, ILogger<AnnouncementService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public IDisposable AnnounceEndpoints(IReadOnlyList<ServiceEndpoint> endpoints)
        {
            if (!_options.IsAnnouncementEnabled)
            {
                return Disposable.Empty;
            }

            var endpointsToAnnounce = endpoints.Where(e => e.HasBehavior<AnnounceableEndpointBehavior>()).ToArray();

            var observable = Observable.Interval(_options.Interval).Finally(() => UnannounceService(endpointsToAnnounce));

            var sequence = from item in observable
                           from endpoint in endpointsToAnnounce
                           select endpoint;

            return sequence.Subscribe(endpoint =>
            {
                using (var client = CreateClient())
                {
                    var metadata = EndpointDiscoveryMetadata.FromServiceEndpoint(endpoint);

                    if (metadata != null)
                    {
                        client.AnnounceOnline(metadata);
                        _logger.LogTrace($"[{endpoint.Contract.ContractType}] Announced {endpoint.Address.Uri} to {_options.RegistryUri}");
                    }
                }
            });
        }

        private void UnannounceService(IReadOnlyList<ServiceEndpoint> endpoints)
        {
            if (endpoints.Any())
            {
                using (var client = CreateClient())
                {
                    foreach (var endpoint in endpoints)
                    {
                        var metadata = EndpointDiscoveryMetadata.FromServiceEndpoint(endpoint);

                        if (metadata != null)
                        {
                            client.AnnounceOffline(metadata);
                            _logger.LogTrace($"Unannounced {endpoint.Address.Uri} to {_options.RegistryUri}");
                        }

                    }
                }
            }
        }

        private AnnouncementClient CreateClient()
        {
            var endpointAddress = new EndpointAddress(_options.RegistryUri);
            var endpoint = new AnnouncementEndpoint(_options.Binding, endpointAddress);
            return new AnnouncementClient(endpoint);
        }
    }
}