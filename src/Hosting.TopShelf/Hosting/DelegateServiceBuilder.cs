using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EMG.Utilities.Hosting
{
    public delegate void ConfigurationBuilderDelegate(IConfigurationBuilder configurationBuilder, IServiceEnvironment serviceEnvironment);

    public delegate void ServiceConfiguratorDelegate(IServiceCollection services, IConfiguration configuration, IServiceEnvironment serviceEnvironment);

    public delegate void LoggingConfiguratorDelegate(ILoggingBuilder loggingBuilder, IConfiguration configuration, IServiceEnvironment serviceEnvironment);

    public class DelegateServiceBuilder : ServiceBuilder
    {
        private readonly ConfigurationBuilderDelegate _configurationBuilder;
        private readonly ServiceConfiguratorDelegate _serviceConfigurator;
        private readonly LoggingConfiguratorDelegate _loggingBuilder;

        public DelegateServiceBuilder(ConfigurationBuilderDelegate configurationBuilder, ServiceConfiguratorDelegate serviceConfigurator, LoggingConfiguratorDelegate loggingBuilder)
        {
            _configurationBuilder = configurationBuilder ?? throw new ArgumentNullException(nameof(configurationBuilder));
            _serviceConfigurator = serviceConfigurator ?? throw new ArgumentNullException(nameof(serviceConfigurator));
            _loggingBuilder = loggingBuilder ?? throw new ArgumentNullException(nameof(loggingBuilder));
        }

        protected override void ConfigureAppConfiguration(IConfigurationBuilder configurationBuilder, IServiceEnvironment serviceEnvironment) => _configurationBuilder(configurationBuilder, serviceEnvironment);

        protected override void ConfigureLogging(ILoggingBuilder logging, IConfiguration configuration, IServiceEnvironment serviceEnvironment) => _loggingBuilder(logging, configuration, serviceEnvironment);

        protected override void ConfigureServices(IServiceCollection services, IConfiguration configuration, IServiceEnvironment serviceEnvironment) => _serviceConfigurator(services, configuration, serviceEnvironment);
    }

}