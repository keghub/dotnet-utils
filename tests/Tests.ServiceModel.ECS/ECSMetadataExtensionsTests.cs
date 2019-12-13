using Moq;
using NUnit.Framework;
using System;
using AutoFixture.NUnit3;
using EMG.Extensions.Configuration.Model;
using EMG.Utilities;
using EMG.Utilities.ServiceModel.Configuration;
using Microsoft.Extensions.Configuration;
using ECSMetadataExtensions = EMG.Utilities.ECSMetadataExtensions;

// ReSharper disable InvokeAsExtensionMethod
#pragma warning disable 618

namespace Tests
{
    public class ECSMetadataExtensionsTests
    {
        [Test, CustomAutoData]
        public void UseECS_returns_private_IPV4_address([Frozen] ECSContainerMetadata metadata, HttpEndpointAddress endpointAddress, IConfiguration configuration, Action<ECSContainerMetadataOptions> configureOptions, string filePath)
        {
            Environment.SetEnvironmentVariable(EMG.Extensions.Configuration.ECSMetadataExtensions.ECSContainerMetadataFileKey, filePath);

            var newEndpoint = ECSMetadataExtensions.UseECS(endpointAddress, configuration, configureOptions);

            Assert.That(newEndpoint.Host, Is.EqualTo(metadata.HostPrivateIPv4Address));
        }

        [Test, CustomAutoData]
        public void UseECS_returns_private_IPV4_address([Frozen] ECSContainerMetadata metadata, NetTcpEndpointAddress endpointAddress, IConfiguration configuration, Action<ECSContainerMetadataOptions> configureOptions, string filePath)
        {
            Environment.SetEnvironmentVariable(EMG.Extensions.Configuration.ECSMetadataExtensions.ECSContainerMetadataFileKey, filePath);

            var newEndpoint = ECSMetadataExtensions.UseECS(endpointAddress, configuration, configureOptions);

            Assert.That(newEndpoint.Host, Is.EqualTo(metadata.HostPrivateIPv4Address));
        }

        [Test, CustomAutoData]
        public void UseECS_uses_configureOptions(HttpEndpointAddress endpointAddress, IConfiguration configuration, Action<ECSContainerMetadataOptions> configureOptions, string filePath)
        {
            Environment.SetEnvironmentVariable(EMG.Extensions.Configuration.ECSMetadataExtensions.ECSContainerMetadataFileKey, filePath);

            ECSMetadataExtensions.UseECS(endpointAddress, configuration, configureOptions);

            Mock.Get(configureOptions).Verify(p => p(It.IsAny<ECSContainerMetadataOptions>()), Times.Once());
        }

        [Test, CustomAutoData]
        public void UseECS_uses_configureOptions(NetTcpEndpointAddress endpointAddress, IConfiguration configuration, Action<ECSContainerMetadataOptions> configureOptions, string filePath)
        {
            Environment.SetEnvironmentVariable(EMG.Extensions.Configuration.ECSMetadataExtensions.ECSContainerMetadataFileKey, filePath);

            ECSMetadataExtensions.UseECS(endpointAddress, configuration, configureOptions);

            Mock.Get(configureOptions).Verify(p => p(It.IsAny<ECSContainerMetadataOptions>()), Times.Once());
        }

        [Test, CustomAutoData]
        public void UseECS_uses_selected_port_mapping(HttpEndpointAddress endpointAddress, IConfiguration configuration, PortMapping mapping, string filePath)
        {
            Environment.SetEnvironmentVariable(EMG.Extensions.Configuration.ECSMetadataExtensions.ECSContainerMetadataFileKey, filePath);

            var newEndpoint = ECSMetadataExtensions.UseECS(endpointAddress, configuration, options => options.PortMappingSelector = items => mapping);

            Assert.That(newEndpoint.Port, Is.EqualTo(mapping.HostPort));
        }

        [Test, CustomAutoData]
        public void UseECS_uses_selected_port_mapping(NetTcpEndpointAddress endpointAddress, IConfiguration configuration, PortMapping mapping, string filePath)
        {
            Environment.SetEnvironmentVariable(EMG.Extensions.Configuration.ECSMetadataExtensions.ECSContainerMetadataFileKey, filePath);

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
        public void UseECS_returns_same_address_if_metadata_not_found_in_configuration(HttpEndpointAddress endpointAddress, ConfigurationBuilder configurationBuilder, string filePath)
        {
            Environment.SetEnvironmentVariable(EMG.Extensions.Configuration.ECSMetadataExtensions.ECSContainerMetadataFileKey, filePath);

            var newEndpoint = ECSMetadataExtensions.UseECS(endpointAddress, configurationBuilder.Build());

            Assert.That(newEndpoint, Is.SameAs(endpointAddress));
        }

        [Test, CustomAutoData]
        public void UseECS_returns_same_address_if_metadata_not_found_in_configuration(NetTcpEndpointAddress endpointAddress, ConfigurationBuilder configurationBuilder, string filePath)
        {
            Environment.SetEnvironmentVariable(EMG.Extensions.Configuration.ECSMetadataExtensions.ECSContainerMetadataFileKey, filePath);

            var newEndpoint = ECSMetadataExtensions.UseECS(endpointAddress, configurationBuilder.Build());

            Assert.That(newEndpoint, Is.SameAs(endpointAddress));
        }

        [Test, CustomAutoData]
        public void UseECS_returns_same_address_if_port_mapping_is_null(HttpEndpointAddress endpointAddress, IConfiguration configuration, string filePath)
        {
            Environment.SetEnvironmentVariable(EMG.Extensions.Configuration.ECSMetadataExtensions.ECSContainerMetadataFileKey, filePath);

            var newEndpoint = ECSMetadataExtensions.UseECS(endpointAddress, configuration, options => options.PortMappingSelector = list => null);

            Assert.That(newEndpoint, Is.SameAs(endpointAddress));
        }

        [Test, CustomAutoData]
        public void UseECS_returns_same_address_if_port_mapping_is_null(NetTcpEndpointAddress endpointAddress, IConfiguration configuration, string filePath)
        {
            Environment.SetEnvironmentVariable(EMG.Extensions.Configuration.ECSMetadataExtensions.ECSContainerMetadataFileKey, filePath);

            var newEndpoint = ECSMetadataExtensions.UseECS(endpointAddress, configuration, options => options.PortMappingSelector = list => null);

            Assert.That(newEndpoint, Is.SameAs(endpointAddress));
        }

        [TearDown]
        public void TearDown()
        {
            Environment.SetEnvironmentVariable(EMG.Extensions.Configuration.ECSMetadataExtensions.ECSContainerMetadataFileKey, null);
        }
    }
}