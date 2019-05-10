using System;
using EMG.Utilities.Hosting;
using Moq;
using NUnit.Framework;
using Topshelf.Runtime;
using Topshelf.ServiceConfigurators;
using HostControl = Topshelf.HostControl;

namespace Tests.Hosting
{
    [TestFixture]
    public class ServiceBuilderTests
    {
        [Test, AutoMoqData]
        public void ConfigureService_register_service(ServiceBuilder sut, ServiceConfigurator<IService> configurator)
        {
            sut.ConfigureService(configurator);

            Mock.Get(configurator).Verify(p => p.ConstructUsing(It.IsAny<ServiceFactory<IService>>()));

            Mock.Get(configurator).Verify(p => p.WhenStarted(It.IsAny<Func<IService, HostControl, bool>>()));

            Mock.Get(configurator).Verify(p => p.WhenStopped(It.IsAny<Func<IService, HostControl, bool>>()));
        }

        [Test, AutoMoqData]
        public void BuildService_builds_service(ServiceBuilder sut, HostSettings settings)
        {
            var service = sut.BuildService(settings);

            Assert.That(service, Is.InstanceOf<WindowsService>());
        }
    }
}