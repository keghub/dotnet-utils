using System.Threading.Tasks;
using Amazon;
using Amazon.SimpleNotificationService;
using EMG.Utilities;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class UsageTests
    {
        [Test, Ignore("Execution sample")]
        public async Task InvokeTestCommand()
        {
            var config = new AmazonSimpleNotificationServiceConfig { RegionEndpoint = RegionEndpoint.EUWest1 };
            var sns = new AmazonSimpleNotificationServiceClient(config);
            var options = new NybusBridgeOptions { TopicArn = "arn:aws:sns:eu-west-1:123457890:your-topic-arn" };
            var bridge = new SnsNybusBridge(sns, options);

            await bridge.InvokeCommand(new TestCommand { Value = "Foo Bar" });
        }

        [Test, Ignore("Execution sample")]
        public async Task RaiseTestEvent()
        {
            var config = new AmazonSimpleNotificationServiceConfig { RegionEndpoint = RegionEndpoint.EUWest1 };
            var sns = new AmazonSimpleNotificationServiceClient(config);
            var options = new NybusBridgeOptions { TopicArn = "arn:aws:sns:eu-west-1:123457890:your-topic-arn" };
            var bridge = new SnsNybusBridge(sns, options);

            await bridge.RaiseEvent(new TestEvent { Value = "Foo Bar" });
        }
    }
}