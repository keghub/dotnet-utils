using System;
using EMG.Extensions.Configuration.Model;
using EMG.Utilities.ServiceModel.Configuration;
using Microsoft.Extensions.Configuration;

namespace EMG.Utilities
{
    public static class ECSMetadataExtensions
    {
        public static HttpEndpointAddress UseECS(this HttpEndpointAddress endpointAddress, IConfiguration configuration, Action<ECSContainerMetadataOptions> configureOptions = null)
        {
            if (Environment.GetEnvironmentVariable(EMG.Extensions.Configuration.ECSMetadataExtensions.ECSContainerMetadataFileKey) == null)
            {
                return endpointAddress;
            }

            var containerMetadata = configuration.Get<ECSContainerMetadata>();

            if (containerMetadata == null)
            {
                return endpointAddress;
            }

            var options = new ECSContainerMetadataOptions();
            configureOptions?.Invoke(options);

            var portMapping = options.PortMappingSelector(containerMetadata.PortMappings);

            if (portMapping == null)
            {
                return endpointAddress;
            }

            return EndpointAddress.ForHttp(containerMetadata.HostPrivateIPv4Address, endpointAddress.Path, portMapping.HostPort, endpointAddress.IsSecure);
        }

        public static NetTcpEndpointAddress UseECS(this NetTcpEndpointAddress endpointAddress, IConfiguration configuration, Action<ECSContainerMetadataOptions> configureOptions = null)
        {
            if (Environment.GetEnvironmentVariable(EMG.Extensions.Configuration.ECSMetadataExtensions.ECSContainerMetadataFileKey) == null)
            {
                return endpointAddress;
            }

            var containerMetadata = configuration.Get<ECSContainerMetadata>();

            if (containerMetadata == null)
            {
                return endpointAddress;
            }

            var options = new ECSContainerMetadataOptions();
            configureOptions?.Invoke(options);

            var portMapping = options.PortMappingSelector(containerMetadata.PortMappings);

            if (portMapping == null)
            {
                return endpointAddress;
            }

            return EndpointAddress.ForNetTcp(portMapping.HostPort, containerMetadata.HostPrivateIPv4Address, endpointAddress.Path);

        }
    }
}