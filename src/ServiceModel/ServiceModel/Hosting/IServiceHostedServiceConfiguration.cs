using System;
using System.ServiceModel;
using EMG.Utilities.ServiceModel.Configuration;

namespace EMG.Utilities.ServiceModel.Hosting
{
    public interface IServiceHostedServiceConfiguration
    {
        Type ServiceType { get; }

        ServiceHost CreateServiceHost();
    }

    public class WcfServiceHostedServiceConfiguration<TService> : IServiceHostedServiceConfiguration where TService : class
    {
        private readonly IServiceHostBuilder _serviceHostBuilder;
        private readonly IServiceHostConfiguration _serviceHostConfiguration;

        public WcfServiceHostedServiceConfiguration(IServiceHostBuilder serviceHostBuilder, WcfServiceHostConfiguration<TService> configuration)
        {
            _serviceHostBuilder = serviceHostBuilder ?? throw new ArgumentNullException(nameof(serviceHostBuilder));
            _serviceHostConfiguration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public Type ServiceType => _serviceHostConfiguration.ServiceType;

        public ServiceHost CreateServiceHost()
        {
            var serviceHost = _serviceHostBuilder.Build(typeof(TService));

            _serviceHostConfiguration.ConfigureServiceHost(serviceHost);

            return serviceHost;
        }
    }

}