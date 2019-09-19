using System;
using EMG.Utilities.ServiceModel;
using EMG.Utilities.ServiceModel.Configuration;
using EMG.Utilities.ServiceModel.Discovery;
using EMG.Utilities.ServiceModel.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
// ReSharper disable InvokeAsExtensionMethod
// ReSharper disable RedundantTypeArgumentsOfMethod

namespace Tests.ServiceModel
{
    [TestFixture]
    public class ServiceCollectionExtensionsTests
    {
        [Test, CustomAutoData]
        public void ServiceCollection_is_returned(IServiceCollection services, WcfServiceConfigurator<TestService> configurator, ServiceLifetime lifetime)
        {
            var result = ServiceCollectionExtensions.AddWcfService<TestService>(services, configurator, lifetime);

            Assert.That(result, Is.SameAs(services));
        }

        [Test, CustomAutoData]
        public void AddWcfService_registers_host_configuration(IServiceCollection services, WcfServiceConfigurator<TestService> configurator, ServiceLifetime lifetime)
        {
            ServiceCollectionExtensions.AddWcfService<TestService>(services, configurator, lifetime);

            Mock.Get(services).Verify(p => p.Add(It.Is<ServiceDescriptor>(d => d.ServiceType == typeof(WcfServiceHostConfiguration<TestService>) && d.Lifetime == ServiceLifetime.Singleton)));
        }

        [Test, CustomAutoData]
        public void AddWcfService_registers_host_builder(IServiceCollection services, WcfServiceConfigurator<TestService> configurator, ServiceLifetime lifetime)
        {
            ServiceCollectionExtensions.AddWcfService<TestService>(services, configurator, lifetime);

            Mock.Get(services).Verify(p => p.Add(It.Is<ServiceDescriptor>(d => d.ServiceType == typeof(IServiceHostBuilder) && d.ImplementationType == typeof(ServiceHostBuilder) && d.Lifetime == ServiceLifetime.Singleton)));
        }

        [Test, CustomAutoData]
        public void AddWcfService_registers_hosted_service_configuration(IServiceCollection services, WcfServiceConfigurator<TestService> configurator, ServiceLifetime lifetime)
        {
            ServiceCollectionExtensions.AddWcfService<TestService>(services, configurator, lifetime);

            Mock.Get(services).Verify(p => p.Add(It.Is<ServiceDescriptor>(d => d.ServiceType == typeof(WcfServiceHostedServiceConfiguration<TestService>) && d.Lifetime == ServiceLifetime.Singleton)));
        }

        [Test, CustomAutoData]
        public void AddWcfService_registers_hosted_service(IServiceCollection services, WcfServiceConfigurator<TestService> configurator, ServiceLifetime lifetime)
        {
            ServiceCollectionExtensions.AddWcfService<TestService>(services, configurator, lifetime);

            Mock.Get(services).Verify(p => p.Add(It.Is<ServiceDescriptor>(d => d.ServiceType == typeof(IHostedService) && d.Lifetime == ServiceLifetime.Singleton)));
        }

        [Test, CustomAutoData]
        public void AddWcfService_registers_announcement_service(IServiceCollection services, WcfServiceConfigurator<TestService> configurator, ServiceLifetime lifetime)
        {
            ServiceCollectionExtensions.AddWcfService<TestService>(services, configurator, lifetime);

            Mock.Get(services).Verify(p => p.Add(It.Is<ServiceDescriptor>(d => d.ServiceType == typeof(IAnnouncementService) && d.Lifetime == ServiceLifetime.Singleton)));
        }

        [Test, CustomAutoData]
        public void AddWcfService_registers_service(IServiceCollection services, WcfServiceConfigurator<TestService> configurator, ServiceLifetime lifetime)
        {
            ServiceCollectionExtensions.AddWcfService<TestService>(services, configurator, lifetime);

            Mock.Get(services).Verify(p => p.Add(It.Is<ServiceDescriptor>(d => d.ServiceType == typeof(TestService) && d.ImplementationType == typeof(TestService) && d.Lifetime == lifetime)));
        }

        [Test, CustomAutoData]
        public void AddDiscovery_returns_service_collection(IServiceCollection services, Uri announcementUri, TimeSpan interval, Action<TestHttpBinding> configureBinding)
        {
            var result = ServiceCollectionExtensions.AddDiscovery(services, announcementUri, interval, configureBinding);

            Assert.That(result, Is.SameAs(services));
        }

        [Test, CustomAutoData]
        public void AddDiscovery_register_announcement_service_options(IServiceCollection services, Uri announcementUri, TimeSpan interval, Action<TestBinding> configureBinding)
        {
            var result = ServiceCollectionExtensions.AddDiscovery(services, announcementUri, interval, configureBinding);

            Mock.Get(services).Verify(p => p.Add(It.Is<ServiceDescriptor>(sd => sd.ServiceType == typeof(IConfigureOptions<AnnouncementServiceOptions>))));
        }

        [Test, CustomAutoData]
        public void AddDiscovery_configures_binding_with_delegate(IServiceCollection services, Uri announcementUri, TimeSpan interval, Action<TestBinding> configureBinding)
        {
            var result = ServiceCollectionExtensions.AddDiscovery(services, announcementUri, interval, configureBinding);

            Mock.Get(configureBinding).Verify(p => p(It.IsAny<TestBinding>()));
        }
    }
}