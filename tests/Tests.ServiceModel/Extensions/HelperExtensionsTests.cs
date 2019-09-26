using System.ServiceModel.Description;
using EMG.Utilities.Extensions;
using NUnit.Framework;

namespace Tests.Extensions
{
    [TestFixture]
    public class HelperExtensionsTests
    {
        [Test, CustomAutoData]
        public void HasBehavior_returns_false_if_behavior_is_not_found(ServiceEndpoint endpoint)
        {
            Assume.That(endpoint.Behaviors, Has.No.InstanceOf<TestEndpointBehavior>());

            var result = endpoint.HasBehavior<TestEndpointBehavior>();

            Assert.That(result, Is.False);
        }
    }
}