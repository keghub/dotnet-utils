using System.ServiceModel;
using AutoFixture.Idioms;
using AutoFixture.NUnit3;
using EMG.Utilities.ServiceModel.Configuration;
using EMG.Utilities.ServiceModel.Hosting;
using Moq;
using NUnit.Framework;

namespace Tests.ServiceModel.Hosting
{
    [TestFixture]
    public class WcfServiceHostedServiceConfigurationTests
    {
        [Test, CustomAutoData]
        public void Constructor_is_guarded(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(WcfServiceHostedServiceConfiguration<TestService>).GetConstructors());
        }

        [Test, CustomAutoData]
        public void ServiceType_returns_configuration_serviceType([Frozen] IServiceHostConfiguration<TestService> configuration, WcfServiceHostedServiceConfiguration<TestService> sut)
        {
            Mock.Get(configuration).SetupGet(p => p.ServiceType).Returns(typeof(TestService));

            Assert.That(sut.ServiceType, Is.EqualTo(configuration.ServiceType));
            Assert.That(sut.ServiceType, Is.EqualTo(typeof(TestService)));
        }

        [Test, CustomAutoData]
        public void CreateServiceHost_uses_builder([Frozen] IServiceHostBuilder builder, WcfServiceHostedServiceConfiguration<TestService> sut)
        {
            var host = sut.CreateServiceHost();

            Mock.Get(builder).Verify(p => p.Build(typeof(TestService)));
        }

        [Test, CustomAutoData]
        public void CreateServiceHost_uses_configuration([Frozen] IServiceHostConfiguration<TestService> configuration, WcfServiceHostedServiceConfiguration<TestService> sut)
        {
            var host = sut.CreateServiceHost();

            Mock.Get(configuration).Verify(p => p.ConfigureServiceHost(It.IsAny<ServiceHost>()));
        }
    }
}