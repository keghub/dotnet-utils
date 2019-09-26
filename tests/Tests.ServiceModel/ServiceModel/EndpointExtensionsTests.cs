using System;
using System.ServiceModel.Description;
using AutoFixture.Idioms;
using EMG.Utilities.ServiceModel;
using EMG.Utilities.ServiceModel.Configuration;
using EMG.Utilities.ServiceModel.Discovery;
using Moq;
using NUnit.Framework;
// ReSharper disable InvokeAsExtensionMethod

namespace Tests.ServiceModel
{
    [TestFixture]
    public class EndpointExtensionsTests
    {
        [Test, CustomAutoData]
        public void AddBehavior_is_guarded_against_nulls(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(EndpointExtensions).GetMethod(nameof(EndpointExtensions.AddBehavior), new[] { typeof(IEndpoint), typeof(IEndpointBehavior) }));
        }

        [Test, CustomAutoData]
        public void AddBehavior_adds_behavior_to_endpoint(IEndpoint endpoint, TestEndpointBehavior testBehavior)
        {
            EndpointExtensions.AddBehavior(endpoint, testBehavior);

            Assert.That(endpoint.Behaviors, Has.One.InstanceOf<TestEndpointBehavior>());
        }

        [Test, CustomAutoData]
        public void AddBehavior_returns_same_endpoint(IEndpoint endpoint, TestEndpointBehavior testBehavior)
        {
            var result = EndpointExtensions.AddBehavior(endpoint, testBehavior);

            Assert.That(result, Is.SameAs(endpoint));
        }

        [Test, CustomAutoData]
        public void AddBehavior_creates_new_instance_of_behavior(IEndpoint endpoint)
        {
            var result = EndpointExtensions.AddBehavior<TestEndpointBehavior>(endpoint);

            Assert.That(endpoint.Behaviors, Has.One.InstanceOf<TestEndpointBehavior>());
        }

        [Test, CustomAutoData]
        public void AddBehavior_returns_same_endpoint(IEndpoint endpoint)
        {
            var result = EndpointExtensions.AddBehavior<TestEndpointBehavior>(endpoint);

            Assert.That(result, Is.SameAs(endpoint));
        }

        [Test, CustomAutoData]
        public void AddBehavior_uses_configuration_delegate(IEndpoint endpoint, Action<TestEndpointBehavior> configurationAction)
        {
            EndpointExtensions.AddBehavior<TestEndpointBehavior>(endpoint, configurationAction);

            Mock.Get(configurationAction).Verify(p => p(It.IsAny<TestEndpointBehavior>()), Times.Once);
        }

        [Test, CustomAutoData]
        public void AddDiscoverable_adds_announceable_behavior_to_endpoint(IEndpoint endpoint)
        {
            EndpointExtensions.Discoverable(endpoint);

            Assert.That(endpoint.Behaviors, Has.One.InstanceOf<AnnounceableEndpointBehavior>());
        }

        [Test, CustomAutoData]
        public void AddDiscoverable_returns_same_endpoint(IEndpoint endpoint)
        {
            var result = EndpointExtensions.Discoverable(endpoint);

            Assert.That(result, Is.SameAs(endpoint));
        }
    }
}