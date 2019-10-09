using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using EMG.Utilities.ServiceModel.Configuration;
using EMG.Utilities.ServiceModel.Logging;

namespace EMG.Utilities.ServiceModel
{
    public static class ServiceHostConfiguratorExtensions
    {
        public static void AddMetadataEndpoints(this IServiceHostConfigurator configurator, Action<ServiceMetadataBehavior> serviceMetadataBehaviorConfigurator = null, string endpointAddress = "mex")
        {
            configurator.ServiceHostConfigurations.Add(host =>
            {
                var behavior = host.Description.Behaviors.Find<ServiceMetadataBehavior>();

                if (behavior == null)
                {
                    behavior = new ServiceMetadataBehavior();

                    host.Description.Behaviors.Add(behavior);
                }

                var hasSupportedEndpoint = false;

                if (TryFindEndpointByBinding<WSHttpBinding>(host, out var wsEndpoint))
                {
                    var address = new Uri($"{wsEndpoint.Address.Uri}/{endpointAddress}");

                    behavior.HttpGetEnabled = true;
                    behavior.HttpGetUrl = address;

                    host.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName, MetadataExchangeBindings.CreateMexHttpBinding(), address);

                    hasSupportedEndpoint = true;
                }

                if (TryFindEndpointByBinding<BasicHttpBinding>(host, out var httpEndpoint))
                {
                    var address = new Uri($"{httpEndpoint.Address.Uri}/{endpointAddress}");

                    behavior.HttpGetEnabled = true;
                    behavior.HttpGetUrl = address;

                    host.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName, new BasicHttpBinding(), address);

                    hasSupportedEndpoint = true;
                }

                if (TryFindEndpointByBinding<NetNamedPipeBinding>(host, out var netNamedEndpoint))
                {
                    host.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName, MetadataExchangeBindings.CreateMexNamedPipeBinding(), $"{netNamedEndpoint.Address.Uri}/mex");

                    hasSupportedEndpoint = true;
                }

                if (TryFindEndpointByBinding<NetTcpBinding>(host, out var netTcpEndpoint))
                {
                    host.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName, MetadataExchangeBindings.CreateMexTcpBinding(), $"{netTcpEndpoint.Address.Uri}/mex");

                    hasSupportedEndpoint = true;
                }

                if (!hasSupportedEndpoint)
                {
                    throw new InvalidOperationException("No supported endpoint has been found.");
                }

                serviceMetadataBehaviorConfigurator?.Invoke(behavior);
            });
        }

        public static void AddServiceBehavior(this IServiceHostConfigurator configurator, IServiceBehavior behavior)
        {
            configurator.ServiceHostConfigurations.Add(host => host.Description.Behaviors.Add(behavior));
        }

        public static void AddExecutionLogging(this IServiceHostConfigurator configurator)
        {
            configurator.AddServiceBehavior(new ExecutionLoggingBehavior(configurator.LoggerFactory));
        }

        private static bool TryFindEndpointByBinding<TBinding>(ServiceHost host, out ServiceEndpoint endpoint)
        {
            endpoint = host.Description.Endpoints.FirstOrDefault(e => e.Binding is TBinding);
            return endpoint != null;
        }

        public static IEndpoint AddBasicHttpEndpoint(this IServiceHostConfigurator configurator, Type contract, BasicHttpEndpointAddress address, Action<BasicHttpBinding> configureBinding = null) => configurator.AddEndpoint(contract, address, configureBinding);

        public static IEndpoint AddNetTcpEndpoint(this IServiceHostConfigurator configurator, Type contract, NetTcpEndpointAddress address, Action<NetTcpBinding> configureBinding = null) => configurator.AddEndpoint(contract, address, configureBinding);

        public static IEndpoint AddNamedPipeEndpoint(this IServiceHostConfigurator configurator, Type contract, NamedPipeEndpointAddress address, Action<NetNamedPipeBinding> configureBinding = null) => configurator.AddEndpoint(contract, address, configureBinding);
    }

}