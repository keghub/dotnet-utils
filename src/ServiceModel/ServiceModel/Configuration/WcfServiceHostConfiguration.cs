using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using EMG.Utilities.ServiceModel.Logging;
using Microsoft.Extensions.Logging;

namespace EMG.Utilities.ServiceModel.Configuration
{
    public class WcfServiceHostConfiguration<TService> : IServiceHostConfigurator<TService>, IServiceHostConfiguration
        where TService : class
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<WcfServiceHostConfiguration<TService>> _logger;
        private readonly List<IEndpoint> _endpoints = new List<IEndpoint>();

        public WcfServiceHostConfiguration(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _logger = _loggerFactory.CreateLogger<WcfServiceHostConfiguration<TService>>();
        }

        public void ConfigureServiceHost(ServiceHost serviceHost)
        {
            AddServiceEndpoints();

            foreach (var action in ServiceHostConfigurations)
            {
                action(serviceHost);
            }

            void AddServiceEndpoints()
            {
                foreach (var endpoint in _endpoints)
                {
                    _logger.LogDebug($"Adding {endpoint.Binding.GetType().Name} to address {endpoint.Address} for contract {endpoint.Contract.FullName}");

                    var serviceEndpoint = serviceHost.AddServiceEndpoint(endpoint.Contract, endpoint.Binding, endpoint.Address);

                    foreach (var behavior in endpoint.Behaviors)
                    {
                        serviceEndpoint.Behaviors.Add(behavior);
                    }
                }
            }
        }

        public Type ServiceType => typeof(TService);

        public IEndpoint AddEndpoint<TBinding>(Type contract, Uri address, Action<TBinding> configureBinding = null)
            where TBinding : Binding, new()
        {
            if (!IsContractValidForService(contract))
            {
                throw new ArgumentException($"{typeof(TService).FullName} does not implement {contract.FullName}", nameof(contract));
            }

            var binding = new TBinding();

            configureBinding?.Invoke(binding);

            var endpoint = new WcfEndpoint(contract, address, binding);

            _endpoints.Add(endpoint);

            return endpoint;
        }

        private static bool IsContractValidForService(Type type) => type.IsAssignableFrom(typeof(TService));

        public IList<Action<ServiceHost>> ServiceHostConfigurations { get; } = new List<Action<ServiceHost>>();

        public void AddExecutionLogging()
        {
            ServiceHostConfigurations.Add(serviceHost =>
            {
                serviceHost.Description.Behaviors.Add(new ExecutionLoggingBehavior(_loggerFactory));
            });
        }
    }

}