using System;
using AutoFixture.Idioms;
using EMG.Utilities.ServiceModel.DependencyInjection;
using EMG.Utilities.ServiceModel.Hosting;
using NUnit.Framework;

namespace Tests.ServiceModel.Hosting
{
    [TestFixture]
    public class ServiceHostBuilderTests
    {
        [Test, CustomAutoData]
        public void Constructor_is_guarded(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(ServiceHostBuilder).GetConstructors());
        }

        [Test, CustomAutoData]
        public void Build_returns_a_service_host_for_service_type(ServiceHostBuilder sut)
        {
            var host = sut.Build(typeof(TestService));

            Assert.That(host, Is.Not.Null);
            Assert.That(host.Description.ServiceType, Is.EqualTo(typeof(TestService)));
        }

        [Test, CustomAutoData]
        public void Build_adds_support_for_dependency_injection(ServiceHostBuilder sut)
        {
            var host = sut.Build(typeof(TestService));

            Assert.That(host.Description.Behaviors, Has.Exactly(1).InstanceOf<DependencyInjectionServiceBehavior>());
        }

        [Test, CustomAutoData]
        public void Build_adds_base_addresses(ServiceHostBuilder sut, Uri baseAddress)
        {
            var host = sut.Build(typeof(TestService), baseAddress);

            Assert.That(host.BaseAddresses, Contains.Item(baseAddress));
        }
    }
}