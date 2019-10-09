using System;
using EMG.Utilities.ServiceModel.Configuration;
using Microsoft.Extensions.Configuration;
using EndpointAddress = EMG.Utilities.ServiceModel.Configuration.EndpointAddress;

namespace EMG.Utilities.ServiceModel
{
    public static class ConfigurationExtensions
    {
        public static TAddress GetEndpointAddress<TAddress>(this IConfiguration configuration) where TAddress : EndpointAddress, new()
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            return configuration.Get<TAddress>();
        }

        public static NetTcpEndpointAddress GetNetTcpEndpointAddress(this IConfiguration configuration) => GetEndpointAddress<NetTcpEndpointAddress>(configuration);

        public static NamedPipeEndpointAddress GetNamedPipeEndpointAddress(this IConfiguration configuration) => GetEndpointAddress<NamedPipeEndpointAddress>(configuration);

        public static HttpEndpointAddress GetBasicHttpEndpointAddress(this IConfiguration configuration) => GetEndpointAddress<HttpEndpointAddress>(configuration);
    }

}