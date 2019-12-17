using Moq;
using NUnit.Framework;
using System;
using System.ServiceModel.Description;
using AutoFixture.NUnit3;
using EMG.Extensions.Configuration.Model;
using EMG.Utilities;
using EMG.Utilities.ServiceModel.Configuration;
using EMG.Utilities.ServiceModel.Discovery;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ECSMetadataExtensions = EMG.Utilities.ECSMetadataExtensions;
using EndpointAddress = System.ServiceModel.EndpointAddress;

// ReSharper disable InvokeAsExtensionMethod
#pragma warning disable 618

namespace Tests
{
    public class ECSMetadataExtensionsTests
    {
        private static void AddFakeContainerMetadataFileKey()
        {
            Environment.SetEnvironmentVariable(EMG.Extensions.Configuration.ECSMetadataExtensions.ECSContainerMetadataFileKey, "some_path");
        }

        [Test, CustomAutoData]
        public void UseECS_returns_private_IPV4_address([Frozen] ECSContainerMetadata metadata, HttpEndpointAddress endpointAddress, IConfiguration configuration, Action<ECSContainerMetadataOptions> configureOptions)
        {
            AddFakeContainerMetadataFileKey();

            var newEndpoint = ECSMetadataExtensions.UseECS(endpointAddress, configuration, configureOptions);

            Assert.That(newEndpoint.Host, Is.EqualTo(metadata.HostPrivateIPv4Address));
        }

        [Test, CustomAutoData]
        public void UseECS_returns_private_IPV4_address([Frozen] ECSContainerMetadata metadata, NetTcpEndpointAddress endpointAddress, IConfiguration configuration, Action<ECSContainerMetadataOptions> configureOptions)
        {
            AddFakeContainerMetadataFileKey();

            var newEndpoint = ECSMetadataExtensions.UseECS(endpointAddress, configuration, configureOptions);

            Assert.That(newEndpoint.Host, Is.EqualTo(metadata.HostPrivateIPv4Address));
        }

        [Test, CustomAutoData]
        public void UseECS_uses_configureOptions(HttpEndpointAddress endpointAddress, IConfiguration configuration, Action<ECSContainerMetadataOptions> configureOptions)
        {
            AddFakeContainerMetadataFileKey();

            ECSMetadataExtensions.UseECS(endpointAddress, configuration, configureOptions);

            Mock.Get(configureOptions).Verify(p => p(It.IsAny<ECSContainerMetadataOptions>()), Times.Once());
        }

        [Test, CustomAutoData]
        public void UseECS_uses_configureOptions(NetTcpEndpointAddress endpointAddress, IConfiguration configuration, Action<ECSContainerMetadataOptions> configureOptions)
        {
            AddFakeContainerMetadataFileKey();

            ECSMetadataExtensions.UseECS(endpointAddress, configuration, configureOptions);

            Mock.Get(configureOptions).Verify(p => p(It.IsAny<ECSContainerMetadataOptions>()), Times.Once());
        }

        [Test, CustomAutoData]
        public void UseECS_uses_selected_port_mapping(HttpEndpointAddress endpointAddress, IConfiguration configuration, PortMapping mapping)
        {
            AddFakeContainerMetadataFileKey();

            var newEndpoint = ECSMetadataExtensions.UseECS(endpointAddress, configuration, options => options.PortMappingSelector = items => mapping);

            Assert.That(newEndpoint.Port, Is.EqualTo(mapping.HostPort));
        }

        [Test, CustomAutoData]
        public void UseECS_uses_selected_port_mapping(NetTcpEndpointAddress endpointAddress, IConfiguration configuration, PortMapping mapping)
        {
            AddFakeContainerMetadataFileKey();

            var newEndpoint = ECSMetadataExtensions.UseECS(endpointAddress, configuration, options => options.PortMappingSelector = items => mapping);

            Assert.That(newEndpoint.Port, Is.EqualTo(mapping.HostPort));
        }

        [Test, CustomAutoData]
        public void UseECS_returns_same_address_if_ECSContainerMetadataFileKey_not_available(HttpEndpointAddress endpointAddress, IConfiguration configuration)
        {
            var newEndpoint = ECSMetadataExtensions.UseECS(endpointAddress, configuration);

            Assert.That(newEndpoint, Is.SameAs(endpointAddress));
        }

        [Test, CustomAutoData]
        public void UseECS_returns_same_address_if_ECSContainerMetadataFileKey_not_available(NetTcpEndpointAddress endpointAddress, IConfiguration configuration)
        {
            var newEndpoint = ECSMetadataExtensions.UseECS(endpointAddress, configuration);

            Assert.That(newEndpoint, Is.SameAs(endpointAddress));
        }

        [Test, CustomAutoData]
        public void UseECS_returns_same_address_if_metadata_not_found_in_configuration(HttpEndpointAddress endpointAddress, ConfigurationBuilder configurationBuilder)
        {
            AddFakeContainerMetadataFileKey();

            var newEndpoint = ECSMetadataExtensions.UseECS(endpointAddress, configurationBuilder.Build());

            Assert.That(newEndpoint, Is.SameAs(endpointAddress));
        }

        [Test, CustomAutoData]
        public void UseECS_returns_same_address_if_metadata_not_found_in_configuration(NetTcpEndpointAddress endpointAddress, ConfigurationBuilder configurationBuilder)
        {
            AddFakeContainerMetadataFileKey();

            var newEndpoint = ECSMetadataExtensions.UseECS(endpointAddress, configurationBuilder.Build());

            Assert.That(newEndpoint, Is.SameAs(endpointAddress));
        }

        [Test, CustomAutoData]
        public void UseECS_returns_same_address_if_port_mapping_is_null(HttpEndpointAddress endpointAddress, IConfiguration configuration)
        {
            AddFakeContainerMetadataFileKey();

            var newEndpoint = ECSMetadataExtensions.UseECS(endpointAddress, configuration, options => options.PortMappingSelector = list => null);

            Assert.That(newEndpoint, Is.SameAs(endpointAddress));
        }

        [Test, CustomAutoData]
        public void UseECS_returns_same_address_if_port_mapping_is_null(NetTcpEndpointAddress endpointAddress, IConfiguration configuration)
        {
            AddFakeContainerMetadataFileKey();

            var newEndpoint = ECSMetadataExtensions.UseECS(endpointAddress, configuration, options => options.PortMappingSelector = list => null);

            Assert.That(newEndpoint, Is.SameAs(endpointAddress));
        }

        [TearDown]
        public void TearDown()
        {
            Environment.SetEnvironmentVariable(EMG.Extensions.Configuration.ECSMetadataExtensions.ECSContainerMetadataFileKey, null);
        }


        [Test, CustomAutoData]
        public void AddECSContainerSupport_configures_AnnouncementServiceOptions(IServiceCollection services, IConfiguration configuration)
        {
            AddFakeContainerMetadataFileKey();

            ECSMetadataExtensions.AddECSContainerSupport(services, configuration);

            Mock.Get(services).Verify(p => p.Add(It.Is<ServiceDescriptor>(sd => sd.ServiceType == typeof(IConfigureOptions<AnnouncementServiceOptions>))));
        }

        [Test, CustomAutoData]
        public void AddECSContainerSupport_does_nothing_if_no_ECS_metadata_file_key_available(IServiceCollection services, IConfiguration configuration)
        {
            Environment.SetEnvironmentVariable(EMG.Extensions.Configuration.ECSMetadataExtensions.ECSContainerMetadataFileKey, null);

            ECSMetadataExtensions.AddECSContainerSupport(services, configuration);

            Mock.Get(services).Verify(p => p.Add(It.IsAny<ServiceDescriptor>()), Times.Never);
        }

        [Test, CustomAutoData]
        public void AddECSContainerSupport_configuration_throws_if_endpoint_has_no_address(ServiceCollection services, IConfiguration configuration, ServiceEndpoint endpoint)
        {
            AddFakeContainerMetadataFileKey();

            ECSMetadataExtensions.AddECSContainerSupport(services, configuration);

            var serviceProvider = services.BuildServiceProvider();

            var options = serviceProvider.GetRequiredService<IOptions<AnnouncementServiceOptions>>();

            Assert.Throws<ArgumentNullException>(() => options.Value.EndpointDiscoveryMetadata(endpoint));
        }

        [Test, CustomAutoData]
        public void AddECSContainerSupport_configuration_returns_metadata_with_replaced_address_when_ecs_metadata_is_valid(ServiceCollection services, [Frozen] ECSContainerMetadata containerMetadata, IConfiguration configuration, ServiceEndpoint endpoint, string address)
        {
            AddFakeContainerMetadataFileKey();

            var expectedAddress = new Uri($"http://{containerMetadata.HostPrivateIPv4Address}:{containerMetadata.PortMappings[0].HostPort}/{address}");

            endpoint.Address = new EndpointAddress($"http://localhost:{containerMetadata.PortMappings[0].ContainerPort}/{address}");

            ECSMetadataExtensions.AddECSContainerSupport(services, configuration);

            var serviceProvider = services.BuildServiceProvider();

            var options = serviceProvider.GetRequiredService<IOptions<AnnouncementServiceOptions>>();

            var result = options.Value.EndpointDiscoveryMetadata(endpoint);

            Assert.That(result.Address.Uri, Is.EqualTo(expectedAddress).IgnoreCase);
        }

        [Test, CustomAutoData]
        public void AddECSContainerSupport_configuration_returns_metadata_with_same_address_when_ecs_metadata_has_no_ipv4_address(ServiceCollection services, IConfiguration configuration, ServiceEndpoint endpoint, int port, string address)
        {
            AddFakeContainerMetadataFileKey();

            configuration[nameof(ECSContainerMetadata.HostPrivateIPv4Address)] = null;

            endpoint.Address = new EndpointAddress($"http://localhost:{port}/{address}");

            ECSMetadataExtensions.AddECSContainerSupport(services, configuration);

            var serviceProvider = services.BuildServiceProvider();

            var options = serviceProvider.GetRequiredService<IOptions<AnnouncementServiceOptions>>();

            var result = options.Value.EndpointDiscoveryMetadata(endpoint);

            Assert.That(result.Address.Uri, Is.EqualTo(endpoint.Address.Uri));
        }

        [Test, CustomAutoData]
        public void AddECSContainerSupport_configuration_returns_metadata_with_same_if_no_mapping(ServiceCollection services, [Frozen] ECSContainerMetadata containerMetadata, IConfiguration configuration, ServiceEndpoint endpoint, int port, string address)
        {
            AddFakeContainerMetadataFileKey();

            var expectedAddress = new Uri($"http://{containerMetadata.HostPrivateIPv4Address}:{port}/{address}");

            endpoint.Address = new EndpointAddress($"http://localhost:{port}/{address}");

            ECSMetadataExtensions.AddECSContainerSupport(services, configuration);

            var serviceProvider = services.BuildServiceProvider();

            var options = serviceProvider.GetRequiredService<IOptions<AnnouncementServiceOptions>>();

            var result = options.Value.EndpointDiscoveryMetadata(endpoint);

            Assert.That(result.Address.Uri, Is.EqualTo(expectedAddress).IgnoreCase);
        }
    }
}