using Topshelf;
using Topshelf.HostConfigurators;

namespace EMG.Utilities.Hosting
{
    public static class HostConfiguratorExtensions
    {
        private static readonly ConfigurationBuilderDelegate EmptyConfigurationBuilderDelegate = delegate { };
        private static readonly ServiceConfiguratorDelegate EmptyServiceConfiguratorDelegate = delegate { };
        private static readonly LoggingConfiguratorDelegate EmptyLoggingConfiguratorDelegate = delegate { };

        public static HostConfigurator HostService(this HostConfigurator configurator, ConfigurationBuilderDelegate configurationBuilder = null, ServiceConfiguratorDelegate serviceBuilder = null, LoggingConfiguratorDelegate loggingBuilder = null)
        {
            configurationBuilder = configurationBuilder ?? EmptyConfigurationBuilderDelegate;
            serviceBuilder = serviceBuilder ?? EmptyServiceConfiguratorDelegate;
            loggingBuilder = loggingBuilder ?? EmptyLoggingConfiguratorDelegate;

            var builder = new DelegateServiceBuilder(configurationBuilder, serviceBuilder, loggingBuilder);

            return configurator.Service<IService>(builder.ConfigureService);
        }
    }
}