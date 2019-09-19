using System;
using System.ServiceModel.Channels;
using EMG.Utilities.ServiceModel.Configuration;
using EMG.Utilities.ServiceModel.Discovery;
using EMG.Utilities.ServiceModel.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EMG.Utilities.ServiceModel
{
    public delegate void WcfServiceConfigurator<TService>(IServiceHostConfigurator<TService> configurator)
        where TService : class;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWcfService<TService>(this IServiceCollection services, WcfServiceConfigurator<TService> configurator, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton) where TService : class
        {
            services.AddSingleton(sp =>
            {
                var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

                var configuration = new WcfServiceHostConfiguration<TService>(loggerFactory);

                configurator?.Invoke(configuration);

                return configuration;
            });

            services.TryAddSingleton<IServiceHostBuilder, ServiceHostBuilder>();

            services.AddSingleton<WcfServiceHostedServiceConfiguration<TService>>();

            services.AddSingleton<IHostedService>(sp =>
            {
                var options = sp.GetRequiredService<WcfServiceHostedServiceConfiguration<TService>>();

                var logger = sp.GetRequiredService<ILogger<WcfServiceHostedService>>();

                var announceService = sp.GetRequiredService<IAnnouncementService>();

                return new WcfServiceHostedService(options, announceService, logger);
            });

            services.TryAddSingleton<IAnnouncementService, EmptyAnnouncementService>();

            services.Add(new ServiceDescriptor(typeof(TService), typeof(TService), serviceLifetime));

            return services;
        }

        public static IServiceCollection AddDiscovery<TBinding>(this IServiceCollection services, Uri announcementUri, TimeSpan interval, Action<TBinding> configureBinding = null)
            where TBinding : Binding, new()
        {
            var binding = new TBinding();
            configureBinding?.Invoke(binding);

            services.Configure<AnnouncementServiceOptions>(options =>
            {
                options.IsAnnouncementEnabled = true;
                options.AnnouncementUri = announcementUri;
                options.Interval = interval;
                options.Binding = binding;
            });

            services.AddSingleton<IAnnouncementService, AnnouncementService>();

            return services;
        }

    }
}