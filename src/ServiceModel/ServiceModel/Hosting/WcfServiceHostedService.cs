using System;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using EMG.Utilities.ServiceModel.Discovery;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EMG.Utilities.ServiceModel.Hosting
{
    public class WcfServiceHostedService<TService> : WcfServiceHostedService where TService : class
    {
        public WcfServiceHostedService(WcfServiceHostedServiceConfiguration<TService> configuration, IAnnouncementService announcementService, ILogger<WcfServiceHostedService> logger) : base(configuration, announcementService, logger) { }
    }

    public class WcfServiceHostedService : IHostedService
    {
        private readonly IServiceHostedServiceConfiguration _configuration;
        private readonly IAnnouncementService _announcementService;
        private readonly ILogger<WcfServiceHostedService> _logger;

        public WcfServiceHostedService(IServiceHostedServiceConfiguration configuration, IAnnouncementService announcementService, ILogger<WcfServiceHostedService> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _announcementService = announcementService ?? throw new ArgumentNullException(nameof(announcementService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private ServiceHost _serviceHost;
        private IDisposable _disposable;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _serviceHost = _configuration.CreateServiceHost();

            _disposable = _announcementService.AnnounceEndpoints(_serviceHost.Description.Endpoints);

            await Task.Factory.FromAsync(_serviceHost.BeginOpen, _serviceHost.EndOpen, null).ConfigureAwait(false);

            _logger.LogDebug($"WCF service started: {_configuration.ServiceType.FullName}");
        }

        private bool _stopping = false;

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_stopping) return;

            _stopping = true;

            _disposable.Dispose();

            await Task.Factory.FromAsync(_serviceHost.BeginClose, _serviceHost.EndClose, null).ConfigureAwait(false);

            _logger.LogDebug($"WCF service stopped: {_configuration.ServiceType.FullName}");
        }
    }
}