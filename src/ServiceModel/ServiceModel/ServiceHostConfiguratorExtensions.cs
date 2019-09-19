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
        public static void EnableDefaultMetadata(this IServiceHostConfigurator configurator, Action<ServiceMetadataBehavior> serviceMetadataBehaviorConfigurator = null)
        {
            configurator.ServiceHostConfigurations.Add(serviceHost =>
            {
                var behavior = serviceHost.Description.Behaviors.Find<ServiceMetadataBehavior>();

                if (behavior == null)
                {
                    behavior = new ServiceMetadataBehavior();

                    serviceHost.Description.Behaviors.Add(behavior);
                }

                bool hasSupportedEndpoint = false;
                
                if (TryFindEndpointByBinding<BasicHttpBinding>(serviceHost, out var httpEndpoint))
                {
                    var address = new Uri($"{httpEndpoint.Address.Uri}/mex");

                    behavior.HttpGetEnabled = true;
                    behavior.HttpGetUrl = address;
                    
                    serviceHost.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName, new BasicHttpBinding(), address);

                    hasSupportedEndpoint = true;
                }

                if (TryFindEndpointByBinding<NetNamedPipeBinding>(serviceHost, out var netNamedEndpoint))
                {
                    serviceHost.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName, MetadataExchangeBindings.CreateMexNamedPipeBinding(), $"{netNamedEndpoint.Address.Uri}/mex");

                    hasSupportedEndpoint = true;
                }

                if (TryFindEndpointByBinding<NetTcpBinding>(serviceHost, out var netTcpEndpoint))
                {
                    serviceHost.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName, MetadataExchangeBindings.CreateMexTcpBinding(), $"{netTcpEndpoint.Address.Uri}/mex");

                    hasSupportedEndpoint = true;
                }

                if (!hasSupportedEndpoint)
                {
                    throw new InvalidOperationException("No supported endpoint has been found.");
                }

                serviceMetadataBehaviorConfigurator?.Invoke(behavior);
            });
        }

        public static void AddExecutionLogging(this IServiceHostConfigurator configurator)
        {
            configurator.ServiceHostConfigurations.Add(host => host.Description.Behaviors.Add(new ExecutionLoggingBehavior(configurator.LoggerFactory)));
        }

        private static bool TryFindEndpointByBinding<TBinding>(ServiceHost host, out ServiceEndpoint endpoint)
        {
            endpoint = host.Description.Endpoints.FirstOrDefault(e => e.Binding is TBinding);
            return endpoint != null;
        }
    }

}