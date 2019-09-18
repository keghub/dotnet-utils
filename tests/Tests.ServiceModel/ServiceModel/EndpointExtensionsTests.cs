using EMG.Utilities.ServiceModel;
using EMG.Utilities.ServiceModel.Configuration;
using EMG.Utilities.ServiceModel.Discovery;
using Moq;
using NUnit.Framework;

namespace Tests.ServiceModel
{
    [TestFixture]
    public class EndpointExtensionsTests
    {
        [Test, CustomAutoData]
        public void AddDiscoverable_adds_announceable_behavior_to_endpoint(IEndpoint endpoint)
        {
            endpoint.Discoverable();

            Assert.That(endpoint.Behaviors, Has.One.InstanceOf<AnnounceableEndpointBehavior>());
        }
    }
}