using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using AutoFixture.Idioms;
using AutoFixture.NUnit3;
using EMG.Utilities.ServiceModel.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Tests.ServiceModel.DependencyInjection
{
    [TestFixture]
    public class DependencyInjectionServiceBehaviorTests
    {
        [Test, CustomAutoData]
        public void Constructor_is_guarded(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(DependencyInjectionServiceBehavior).GetConstructors());
        }

        [Test]
        [CustomInlineAutoData]
        [CustomInlineAutoData(null)]
        [CustomInlineAutoData(null, null)]
        public void Validate_does_not_throw(ServiceDescription description, ServiceHost host, DependencyInjectionServiceBehavior sut)
        {
            Assert.DoesNotThrow(() => sut.Validate(description, host));
        }

        [Test,CustomAutoData]
        public void AddBindingParameters_does_not_throw(DependencyInjectionServiceBehavior sut)
        {
            Assert.DoesNotThrow(() => sut.AddBindingParameters(null, null, null, null));
        }

        [Test, CustomAutoData]
        public void ApplyDispatchBehavior([Frozen] IServiceProvider serviceProvider, DependencyInjectionServiceBehavior sut, ServiceHost host, ILogger<DependencyInjectionInstanceProvider> providerLogger, string endpoint)
        {
            Mock.Get(serviceProvider).Setup(p => p.GetService(typeof(ILogger<DependencyInjectionInstanceProvider>))).Returns(providerLogger);

            host.AddServiceEndpoint(typeof(ITestService), new NetNamedPipeBinding(), new Uri($"net.pipe://localhost/{endpoint}"));

            sut.ApplyDispatchBehavior(host.Description, host);
        }
    }
}