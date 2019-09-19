using EMG.Utilities.ServiceModel;
using EMG.Utilities.ServiceModel.Configuration;
using EMG.Utilities.ServiceModel.Discovery;
using NUnit.Framework;
// ReSharper disable InvokeAsExtensionMethod

namespace Tests.ServiceModel
{
    [TestFixture]
    public class EndpointExtensionsTests
    {
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