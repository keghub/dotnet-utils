using AutoFixture.Idioms;
using AutoFixture.NUnit3;
using EMG.Utilities.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Topshelf.HostConfigurators;
using Topshelf.Runtime;

namespace Tests.Hosting
{
    [TestFixture]
    public class DelegateServiceBuilderTests
    {
        [Test, AutoMoqData]
        public void Constructor_is_guarded(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(DelegateServiceBuilder).GetConstructors());
        }

        [Test, AutoMoqData]
        public void BuildService_builds_service([Frozen] ConfigurationBuilderDelegate configurationBuilder, [Frozen]  ServiceConfiguratorDelegate serviceConfigurator, [Frozen] LoggingConfiguratorDelegate loggingBuilder, DelegateServiceBuilder sut, HostSettings settings)
        {
            var service = sut.BuildService(settings);

            Assert.That(service, Is.InstanceOf<WindowsService>());

            Mock.Get(configurationBuilder).Verify(p => p(It.IsAny<IConfigurationBuilder>(), It.IsAny<ServiceEnvironment>()));

            Mock.Get(serviceConfigurator).Verify(p => p(It.IsAny<IServiceCollection>(), It.IsAny<IConfiguration>(), It.IsAny<ServiceEnvironment>()));

            Mock.Get(loggingBuilder).Verify(p => p(It.IsAny<ILoggingBuilder>(), It.IsAny<IConfiguration>(), It.IsAny<ServiceEnvironment>()));
        }
    }

    [TestFixture]
    public class HostConfiguratorExtensionsTests
    {
        [Test, AutoMoqData]
        public void HostService_configure_service(HostConfigurator configurator, ConfigurationBuilderDelegate configurationBuilder, ServiceConfiguratorDelegate serviceBuilder, LoggingConfiguratorDelegate loggingBuilder)
        {
            configurator.HostService(configurationBuilder, serviceBuilder, loggingBuilder);

            Mock.Get(configurator).Verify(p => p.UseServiceBuilder(It.IsAny<ServiceBuilderFactory>()));
        }

        [Test, AutoMoqData]
        public void HostService_doesnt_require_any_delegate(HostConfigurator configurator)
        {
            configurator.HostService();

            Mock.Get(configurator).Verify(p => p.UseServiceBuilder(It.IsAny<ServiceBuilderFactory>()));
        }
    }
}