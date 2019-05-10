using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Topshelf.Runtime;
using Topshelf.ServiceConfigurators;

namespace EMG.Utilities.Hosting
{
    public abstract class ServiceBuilder
    {
        protected virtual void ConfigureAppConfiguration(IConfigurationBuilder configurationBuilder, IServiceEnvironment serviceEnvironment) { }

        protected virtual void ConfigureServices(IServiceCollection services, IConfiguration configuration, IServiceEnvironment serviceEnvironment) { }

        protected virtual void ConfigureLogging(ILoggingBuilder logging, IConfiguration configuration, IServiceEnvironment serviceEnvironment) { }

        public IService BuildService(HostSettings settings)
        {
            var serviceEnvironment = GetServiceEnvironment();

            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddConfiguration(serviceEnvironment.Configuration);

            ConfigureAppConfiguration(configurationBuilder, serviceEnvironment);

            var configuration = configurationBuilder.Build();

            var services = new ServiceCollection();

            services.AddSingleton(serviceEnvironment);
            services.AddSingleton<IConfiguration>(configuration);

            ConfigureServices(services, configuration, serviceEnvironment);

            services.AddLogging(logging => ConfigureLogging(logging, configuration, serviceEnvironment));

            var serviceProvider = services.BuildServiceProvider();

            return new WindowsService(configuration, serviceProvider);
        }

        private IServiceEnvironment GetServiceEnvironment()
        {
            var hostConfigurationBuilder = new ConfigurationBuilder();
            hostConfigurationBuilder.AddJsonFile("hostsettings.json", true, true);
            hostConfigurationBuilder.AddEnvironmentVariables();

            var hostConfiguration = hostConfigurationBuilder.Build();

            var serviceEnvironment = hostConfiguration.GetServiceEnvironment();
            return serviceEnvironment;
        }

        public void ConfigureService(ServiceConfigurator<IService> configurator)
        {
            configurator.ConstructUsing(BuildService);

            configurator.WhenStarted((service, control) => service.OnStart(control));

            configurator.WhenStopped((service, control) => service.OnStop(control));
        }
    }

}