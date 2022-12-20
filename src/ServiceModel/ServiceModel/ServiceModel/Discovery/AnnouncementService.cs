using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
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

        public Func<ServiceEndpoint, EndpointDiscoveryMetadata> EndpointDiscoveryMetadata { get; set; } = System.ServiceModel.Discovery.EndpointDiscoveryMetadata.FromServiceEndpoint;
    }

    public class AnnouncementService : IAnnouncementService
    {
        private readonly ILogger<AnnouncementService> _logger;
        private readonly IAnnouncementClientWrapperFactory _clientFactory;
        private readonly IScheduler _scheduler;
        private readonly AnnouncementServiceOptions _options;

        public AnnouncementService(IOptions<AnnouncementServiceOptions> options, ILogger<AnnouncementService> logger, IAnnouncementClientWrapperFactory clientFactory, IScheduler scheduler = null)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
            _scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public IDisposable AnnounceEndpoints(IReadOnlyList<ServiceEndpoint> endpoints)
        {
            if (!_options.IsAnnouncementEnabled)
            {
                return Disposable.Empty;
            }

            var endpointsToAnnounce = endpoints.Where(e => e.HasBehavior<AnnounceableBehavior>()).ToArray();

            var observable = Observable.Interval(_options.Interval, _scheduler).Finally(() => UnannounceService(endpointsToAnnounce));

            var sequence = from item in observable
                           from endpoint in endpointsToAnnounce
                           select endpoint;

            return sequence.Subscribe(endpoint =>
            {
                using (var client = _clientFactory.Create(_options.RegistryUri, _options.Binding))
                {
                    var metadata = _options.EndpointDiscoveryMetadata(endpoint);

                    if (metadata != null)
                    {
                        try
                        {
                            client.AnnounceOnline(metadata);
                            _logger.LogTrace(
                                $"[{endpoint.Contract.ContractType}] Announced {endpoint.Address.Uri} to {_options.RegistryUri}");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex,
                                $"Failed to announce [{endpoint.Contract.ContractType}] service to {_options.RegistryUri}: {ex}");
                        }
                    }
                }
            });
        }

        private void UnannounceService(IReadOnlyList<ServiceEndpoint> endpoints)
        {
            if (endpoints.Any())
            {
                using (var client = _clientFactory.Create(_options.RegistryUri, _options.Binding))
                {
                    foreach (var endpoint in endpoints)
                    {
                        var metadata = _options.EndpointDiscoveryMetadata(endpoint);

                        if (metadata != null)
                        {
                            try
                            {
                                client.AnnounceOffline(metadata);
                                _logger.LogTrace($"Unannounced {endpoint.Address.Uri} to {_options.RegistryUri}");
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning(ex, $"Failed to announce [{endpoint.Contract.ContractType}] service to {_options.RegistryUri}: {ex}");
                            }
                        }
                    }
                }
            }
        }
    }
}