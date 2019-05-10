using System;
using System.Collections.Generic;
using System.Threading;
using AutoFixture.Idioms;
using AutoFixture.NUnit3;
using EMG.Utilities.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Moq;
using NUnit.Framework;
using Topshelf;

namespace Tests.Hosting
{
    [TestFixture]
    public class WindowsServiceTests
    {
        [Test, AutoMoqData]
        public void Constructor_is_guarded(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(WindowsService).GetConstructors());
        }

        [Test, AutoMoqData]
        public void Given_ServiceProvider_is_available([Frozen] IServiceProvider services, WindowsService sut)
        {
            Assert.That(sut.Services, Is.SameAs(services));
        }

        [Test, AutoMoqData]
        public void Given_Configuration_is_available([Frozen] IConfiguration configuration, WindowsService sut)
        {
            Assert.That(sut.Configuration, Is.SameAs(configuration));
        }

        [Test, AutoMoqData]
        public void OnStart_starts_given_hosted_service([Frozen] IServiceProvider services, WindowsService sut, IHostedService hostedService, HostControl control)
        {
            Mock.Get(services).Setup(p => p.GetService(typeof(IEnumerable<IHostedService>))).Returns(new[] { hostedService });

            sut.OnStart(control);

            Mock.Get(hostedService).Verify(p => p.StartAsync(It.IsAny<CancellationToken>()));
        }

        [Test, AutoMoqData]
        public void OnStop_stops_started_hosted_service([Frozen] IServiceProvider services, WindowsService sut, IHostedService hostedService, HostControl control)
        {
            Mock.Get(services).Setup(p => p.GetService(typeof(IEnumerable<IHostedService>))).Returns(new[] { hostedService });

            sut.OnStart(control);

            sut.OnStop(control);

            Mock.Get(hostedService).Verify(p => p.StopAsync(It.IsAny<CancellationToken>()));
        }
    }
}
