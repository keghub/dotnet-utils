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
        public static void EnableMetadata(this IServiceHostConfigurator configurator, int? port = null, Action<ServiceMetadataBehavior> serviceMetadataBehaviorConfigurator = null)
        {
            configurator.ServiceHostConfigurations.Add(serviceHost =>
            {
                var metadataPort = port ?? GetPortFromExistingBindings(serviceHost, Uri.UriSchemeHttp) ?? throw new ArgumentException("A port number is required if no HTTP endpoint is already defined");

                var uriBuilder = new UriBuilder(Uri.UriSchemeHttp, "localhost", metadataPort);

                var address = uriBuilder.Uri;

                var behavior = new ServiceMetadataBehavior
                {
                    HttpGetEnabled = true,
                    HttpGetUrl = address
                };

                serviceMetadataBehaviorConfigurator?.Invoke(behavior);

                serviceHost.Description.Behaviors.Add(behavior);

                serviceHost.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName, new BasicHttpBinding(), address);
            });

            int? GetPortFromExistingBindings(ServiceHost serviceHost, string uriScheme)
            {
                var httpBinding = serviceHost.Description.Endpoints.FirstOrDefault(s => s.Address.Uri.Scheme == uriScheme);

                return httpBinding?.Address.Uri.Port;
            }
        }

        public static void AddExecutionLogging(this IServiceHostConfigurator configurator)
        {
            configurator.ServiceHostConfigurations.Add(host => host.Description.Behaviors.Add(new ExecutionLoggingBehavior(configurator.LoggerFactory)));
        }
    }

}