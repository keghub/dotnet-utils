using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using EMG.Utilities.ServiceModel;
using EMG.Utilities.ServiceModel.Configuration;
using NUnit.Framework;
// ReSharper disable InvokeAsExtensionMethod

namespace Tests.ServiceModel
{
    [TestFixture]
    public class ServiceHostConfiguratorExtensionsTests
    {
        [Test, CustomAutoData]
        public void EnableMetadata_adds_service_host_configuration(IServiceHostConfigurator configurator, int port, Action<ServiceMetadataBehavior> serviceMetadataBehaviorConfigurator)
        {
            Assume.That(configurator.ServiceHostConfigurations, Is.Empty);

            ServiceHostConfiguratorExtensions.EnableMetadata(configurator, port, serviceMetadataBehaviorConfigurator);

            Assert.That(configurator.ServiceHostConfigurations, Has.One.InstanceOf<Action<ServiceHost>>());
        }

        [Test, CustomAutoData]
        public void AddExecutionLogging_adds_service_host_configuration(IServiceHostConfigurator configurator, int port, Action<ServiceMetadataBehavior> serviceMetadataBehaviorConfigurator)
        {
            Assume.That(configurator.ServiceHostConfigurations, Is.Empty);

            ServiceHostConfiguratorExtensions.AddExecutionLogging(configurator);

            Assert.That(configurator.ServiceHostConfigurations, Has.One.InstanceOf<Action<ServiceHost>>());
        }

    }
}