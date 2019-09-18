using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using AutoFixture.Idioms;
using EMG.Utilities.ServiceModel.Configuration;
using Moq;
using NUnit.Framework;

namespace Tests.ServiceModel.Configuration
{
    [TestFixture]
    public class WcfServiceHostConfigurationTests
    {
        [Test, CustomAutoData]
        public void Constructor_is_guarded(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(WcfServiceHostConfiguration<>).GetConstructors());
        }

        [Test, CustomAutoData]
        public void ServiceType_reflects_the_generic_parameter(WcfServiceHostConfiguration<TestService> sut)
        {
            Assert.That(sut.ServiceType, Is.EqualTo(typeof(TestService)));
        }

        [Test, CustomAutoData]
        public void ConfigureServiceHost_executes_given_actions(WcfServiceHostConfiguration<TestService> sut, ServiceHost host, Action<ServiceHost> additionalSetup)
        {
            sut.ServiceHostConfigurations.Add(additionalSetup);

            sut.ConfigureServiceHost(host);

            Mock.Get(additionalSetup).Verify(p => p(host), Times.Once);
        }

        [Test, CustomAutoData]
        public void ConfigureServiceHost_adds_custom_endpoint_behaviors(WcfServiceHostConfiguration<TestService> sut, ServiceHost host, Uri endpointAddress, IEndpointBehavior endpointBehavior)
        {
            sut.AddEndpoint<TestHttpBinding>(typeof(ITestService), endpointAddress).Behaviors.Add(endpointBehavior);

            sut.ConfigureServiceHost(host);

            Assert.That(host.Description.Endpoints[0].Behaviors, Contains.Item(endpointBehavior));
        }

        [Test, CustomAutoData]
        public void AddEndpoint_registers_endpoint_for_given_binding(WcfServiceHostConfiguration<TestService> sut, Uri endpointAddress, Action<TestHttpBinding> bindingConfiguration, ServiceHost host)
        {
            sut.AddEndpoint<TestHttpBinding>(typeof(ITestService), endpointAddress, bindingConfiguration);

            sut.ConfigureServiceHost(host);

            Assert.That(host.Description.Endpoints, Has.Exactly(1).InstanceOf<ServiceEndpoint>());
        }

        [Test, CustomAutoData]
        public void AddEndpoint_registers_endpoint_for_given_binding(WcfServiceHostConfiguration<TestService> sut, Uri endpointAddress, ServiceHost host)
        {
            sut.AddEndpoint<TestHttpBinding>(typeof(ITestService), endpointAddress);

            sut.ConfigureServiceHost(host);

            Assert.That(host.Description.Endpoints, Has.Exactly(1).InstanceOf<ServiceEndpoint>());
        }

        [Test, CustomAutoData]
        public void AddEndpoint_uses_binding_configuration_when_given(WcfServiceHostConfiguration<TestService> sut, Uri endpointAddress, Action<TestHttpBinding> bindingConfiguration, ServiceHost host)
        {
            sut.AddEndpoint<TestHttpBinding>(typeof(ITestService), endpointAddress, bindingConfiguration);

            sut.ConfigureServiceHost(host);

            Mock.Get(bindingConfiguration).Verify(p => p(It.IsAny<TestHttpBinding>()), Times.Once);
        }

        [Test, CustomAutoData]
        public void AddEndpoint_throws_if_service_does_not_implement_contract(WcfServiceHostConfiguration<TestService> sut, Uri endpointAddress)
        {
            Assert.Throws<ArgumentException>(() => sut.AddEndpoint<TestHttpBinding>(typeof(IInvalidTestService), endpointAddress));
        }

        [Test, CustomAutoData]
        public void AddEndpoint_throws_if_binding_and_address_schema_dont_match(WcfServiceHostConfiguration<TestService> sut, Uri endpointAddress)
        {
            Assume.That(endpointAddress.Scheme, Is.Not.Empty);

            Assert.Throws<ArgumentException>(() => sut.AddEndpoint<TestBinding>(typeof(ITestService), endpointAddress));
        }
    }
}