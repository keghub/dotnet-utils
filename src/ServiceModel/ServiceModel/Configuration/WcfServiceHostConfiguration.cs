using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Microsoft.Extensions.Logging;

namespace EMG.Utilities.ServiceModel.Configuration
{
    public class WcfServiceHostConfiguration<TService> : IServiceHostConfigurator<TService>, IServiceHostConfiguration<TService>
        where TService : class
    {
        private readonly ILogger<WcfServiceHostConfiguration<TService>> _logger;
        private readonly List<IEndpoint> _endpoints = new List<IEndpoint>();

        public WcfServiceHostConfiguration(ILoggerFactory loggerFactory)
        {
            LoggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _logger = LoggerFactory.CreateLogger<WcfServiceHostConfiguration<TService>>();
        }

        public void ConfigureServiceHost(ServiceHost serviceHost)
        {
            _logger.LogDebug("Configuring {SERVICE}", typeof(TService).FullName);

            AddServiceEndpoints();

            foreach (var action in ServiceHostConfigurations)
            {
                action(serviceHost);
            }

            void AddServiceEndpoints()
            {
                foreach (var endpoint in _endpoints)
                {
                    _logger.LogDebug("Adding {BINDING} to address {ADDRESS} for contract {CONTRACT}", endpoint.Binding.GetType().Name, endpoint.Address, endpoint.Contract.FullName);

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

            if (!string.Equals(address.Scheme, binding.Scheme))
            {
                throw new ArgumentException($"Binding scheme and address scheme don't match", nameof(address));
            }

            configureBinding?.Invoke(binding);

            var endpoint = new WcfEndpoint(contract, address, binding);

            _endpoints.Add(endpoint);

            return endpoint;
        }

        private static bool IsContractValidForService(Type type) => type.IsAssignableFrom(typeof(TService));

        public IList<Action<ServiceHost>> ServiceHostConfigurations { get; } = new List<Action<ServiceHost>>();

        public ILoggerFactory LoggerFactory { get; }
    }

}