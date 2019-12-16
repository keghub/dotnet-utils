using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Description;
using System.ServiceModel.Discovery;
using EMG.Extensions.Configuration.Model;
using EMG.Utilities.ServiceModel.Configuration;
using EMG.Utilities.ServiceModel.Discovery;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EMG.Utilities
{
    public static class ECSMetadataExtensions
    {
        [Obsolete("Use AddECSContainerSupport instead.")]
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

        [Obsolete("Use AddECSContainerSupport instead.")]
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

        public static IServiceCollection AddECSContainerSupport(this IServiceCollection services, IConfiguration configuration)
        {
            if (Environment.GetEnvironmentVariable(EMG.Extensions.Configuration.ECSMetadataExtensions.ECSContainerMetadataFileKey) != null)
            {
                services.Configure<AnnouncementServiceOptions>(o => o.EndpointDiscoveryMetadata = GetEndpointMetadataFromECS);
            }

            return services;

            EndpointDiscoveryMetadata GetEndpointMetadataFromECS(ServiceEndpoint endpoint)
            {
                if (endpoint.Address is null)
                {
                    throw new ArgumentNullException(nameof(endpoint),"endpoint address should not be null");
                }

                var containerMetadata = configuration.Get<ECSContainerMetadata>();

                var endpointMetadata = EndpointDiscoveryMetadata.FromServiceEndpoint(endpoint);

                if (endpointMetadata is null)
                {
                    return null;
                }

                if (containerMetadata?.HostPrivateIPv4Address is null)
                {
                    return endpointMetadata;
                }

                var uriBuilder = new UriBuilder(endpointMetadata.Address.Uri)
                {
                    Host = containerMetadata.HostPrivateIPv4Address,
                    Port = GetMappedPortOrDefault(containerMetadata.PortMappings, endpointMetadata.Address.Uri.Port)
                };

                endpointMetadata.Address = new System.ServiceModel.EndpointAddress(uriBuilder.Uri.ToString());

                return endpointMetadata;
            }

            int GetMappedPortOrDefault(IEnumerable<PortMapping> mappings, int port)
            {
                var hostPortByContainerPort = mappings.ToDictionary(k => k.ContainerPort, v => v.HostPort);

                if (hostPortByContainerPort.TryGetValue(port, out var mappedPort))
                {
                    return mappedPort;
                }

                return port;
            }
        }
    }
}