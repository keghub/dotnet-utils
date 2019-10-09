using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using EMG.Utilities.ServiceModel;
using EMG.Utilities.ServiceModel.Configuration;
using EMG.Utilities.ServiceModel.Logging;
using Moq;
using NUnit.Framework;
// ReSharper disable InvokeAsExtensionMethod

namespace Tests.ServiceModel
{
    [TestFixture]
    public class ServiceHostConfiguratorExtensionsTests
    {
        [Test, CustomAutoData]
        public void AddMetadataEndpoints_adds_service_host_configuration(WcfServiceHostConfiguration<TestService> configurator, Action<ServiceMetadataBehavior> serviceMetadataBehaviorConfigurator)
        {
            Assume.That(configurator.ServiceHostConfigurations, Is.Empty);

            ServiceHostConfiguratorExtensions.AddMetadataEndpoints(configurator, serviceMetadataBehaviorConfigurator);

            Assert.That(configurator.ServiceHostConfigurations, Has.One.InstanceOf<Action<ServiceHost>>());
        }

        [Test, CustomAutoData]
        public void AddMetadataEndpoints_adds_ServiceMetadataBehavior_to_host_with_NamedPipe_binding(WcfServiceHostConfiguration<TestService> configurator, ServiceHost host)
        {
            configurator.AddEndpoint<NetNamedPipeBinding>(typeof(ITestService), new Uri("net.pipe://localhost/test"));

            configurator.ConfigureServiceHost(host);

            ServiceHostConfiguratorExtensions.AddMetadataEndpoints(configurator);

            var configuration = configurator.ServiceHostConfigurations.First();

            configuration(host);

            Assert.That(host.Description.Behaviors.Find<ServiceMetadataBehavior>(), Is.Not.Null);
        }

        [Test, CustomAutoData]
        public void AddMetadataEndpoints_adds_metadata_endpoint_to_host_with_NamedPipe_binding(WcfServiceHostConfiguration<TestService> configurator, ServiceHost host)
        {
            configurator.AddEndpoint<NetNamedPipeBinding>(typeof(ITestService), new Uri("net.pipe://localhost/test"));

            configurator.ConfigureServiceHost(host);

            ServiceHostConfiguratorExtensions.AddMetadataEndpoints(configurator);

            var configuration = configurator.ServiceHostConfigurations.First();

            configuration(host);

            Assert.That(host.Description.Endpoints.Any(endpoint => endpoint.Address.Uri.AbsolutePath.EndsWith("mex")));
        }

        [Test, CustomAutoData]
        public void AddMetadataEndpoints_adds_ServiceMetadataBehavior_to_host_with_NetTcp_binding(WcfServiceHostConfiguration<TestService> configurator, ServiceHost host)
        {
            configurator.AddEndpoint<NetTcpBinding>(typeof(ITestService), new Uri("net.tcp://localhost/test"));

            configurator.ConfigureServiceHost(host);

            ServiceHostConfiguratorExtensions.AddMetadataEndpoints(configurator);

            var configuration = configurator.ServiceHostConfigurations.First();

            configuration(host);

            Assert.That(host.Description.Behaviors.Find<ServiceMetadataBehavior>(), Is.Not.Null);
        }

        [Test, CustomAutoData]
        public void AddMetadataEndpoints_adds_metadata_endpoint_to_host_with_NetTcp_binding(WcfServiceHostConfiguration<TestService> configurator, ServiceHost host)
        {
            configurator.AddEndpoint<NetTcpBinding>(typeof(ITestService), new Uri("net.tcp://localhost/test"));

            configurator.ConfigureServiceHost(host);

            ServiceHostConfiguratorExtensions.AddMetadataEndpoints(configurator);

            var configuration = configurator.ServiceHostConfigurations.First();

            configuration(host);

            Assert.That(host.Description.Endpoints.Any(endpoint => endpoint.Address.Uri.AbsolutePath.EndsWith("mex")));
        }

        [Test, CustomAutoData]
        public void AddMetadataEndpoints_adds_ServiceMetadataBehavior_to_host_with_BasicHttp_binding(WcfServiceHostConfiguration<TestService> configurator, ServiceHost host)
        {
            configurator.AddEndpoint<BasicHttpBinding>(typeof(ITestService), new Uri("http://localhost/test"));

            configurator.ConfigureServiceHost(host);

            ServiceHostConfiguratorExtensions.AddMetadataEndpoints(configurator);

            var configuration = configurator.ServiceHostConfigurations.First();

            configuration(host);

            Assert.That(host.Description.Behaviors.Find<ServiceMetadataBehavior>(), Is.Not.Null);
        }

        [Test, CustomAutoData]
        public void AddMetadataEndpoints_adds_metadata_endpoint_to_host_with_BasicHttp_binding(WcfServiceHostConfiguration<TestService> configurator, ServiceHost host)
        {
            configurator.AddEndpoint<BasicHttpBinding>(typeof(ITestService), new Uri("http://localhost/test"));

            configurator.ConfigureServiceHost(host);

            ServiceHostConfiguratorExtensions.AddMetadataEndpoints(configurator);

            var configuration = configurator.ServiceHostConfigurations.First();

            configuration(host);

            Assert.That(host.Description.Endpoints.Any(endpoint => endpoint.Address.Uri.AbsolutePath.EndsWith("mex")));
        }

        [Test, CustomAutoData]
        public void AddMetadataEndpoints_throws_if_not_supported_endpoint_is_added(WcfServiceHostConfiguration<TestService> configurator, ServiceHost host)
        {
            configurator.ConfigureServiceHost(host);

            ServiceHostConfiguratorExtensions.AddMetadataEndpoints(configurator);

            var configuration = configurator.ServiceHostConfigurations.First();

            Assert.Throws<InvalidOperationException>(() => configuration(host));
        }

        [Test, CustomAutoData]
        public void AddExecutionLogging_adds_service_host_configuration(WcfServiceHostConfiguration<TestService> configurator, Action<ServiceMetadataBehavior> serviceMetadataBehaviorConfigurator)
        {
            Assume.That(configurator.ServiceHostConfigurations, Is.Empty);

            ServiceHostConfiguratorExtensions.AddExecutionLogging(configurator);

            Assert.That(configurator.ServiceHostConfigurations, Has.One.InstanceOf<Action<ServiceHost>>());
        }

        [Test, CustomAutoData]
        public void AddExecutionLogging_adds_behavior_to_host(WcfServiceHostConfiguration<TestService> configurator, ServiceHost host)
        {
            ServiceHostConfiguratorExtensions.AddExecutionLogging(configurator);

            var configuration = configurator.ServiceHostConfigurations.First();

            configuration(host);

            Assert.That(host.Description.Behaviors.Find<ExecutionLoggingBehavior>(), Is.Not.Null);
        }

        [Test, CustomAutoData]
        public void AddBasicHttpEndpoint_forwards_to_configurator(IServiceHostConfigurator configurator, Action<BasicHttpBinding> testDelegate, BasicHttpEndpointAddress address)
        {
            configurator.AddBasicHttpEndpoint(typeof(ITestService), address, testDelegate);

            Mock.Get(configurator).Verify(p => p.AddEndpoint<BasicHttpBinding>(typeof(ITestService), address, testDelegate));
        }

        [Test, CustomAutoData]
        public void AddNetTcpEndpoint_forwards_to_configurator(IServiceHostConfigurator configurator, Action<NetTcpBinding> testDelegate, NetTcpEndpointAddress address)
        {
            configurator.AddNetTcpEndpoint(typeof(ITestService), address, testDelegate);

            Mock.Get(configurator).Verify(p => p.AddEndpoint<NetTcpBinding>(typeof(ITestService), address, testDelegate));
        }

        [Test, CustomAutoData]
        public void AddNamedPipeEndpoint_forwards_to_configurator(IServiceHostConfigurator configurator, Action<NetNamedPipeBinding> testDelegate, NamedPipeEndpointAddress address)
        {
            configurator.AddNamedPipeEndpoint(typeof(ITestService), address, testDelegate);

            Mock.Get(configurator).Verify(p => p.AddEndpoint<NetNamedPipeBinding>(typeof(ITestService), address, testDelegate));
        }
    }
}