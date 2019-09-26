using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading.Tasks;
using AutoFixture.Idioms;
using AutoFixture.NUnit3;
using EMG.Utilities.ServiceModel.Discovery;
using EMG.Utilities.ServiceModel.Hosting;
using Moq;
using NUnit.Framework;

namespace Tests.ServiceModel.Hosting
{
    [TestFixture]
    public class WcfServiceHostedServiceTests
    {
        [Test, CustomAutoData]
        public void Constructor_is_guarded(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(WcfServiceHostedService).GetConstructors());
        }

        [Test, CustomAutoData]
        public async Task StartAsync_starts_host([Frozen] IServiceHostedServiceConfiguration configuration, WcfServiceHostedService sut,[Frozen] ServiceHost serviceHost)
        {
            Mock.Get(configuration).Setup(p => p.CreateServiceHost()).Returns(serviceHost);

            serviceHost.AddServiceEndpoint(typeof(ITestService), new NetNamedPipeBinding(), new Uri("net.pipe://localhost/test"));

            await sut.StartAsync(default);

            Assert.That(serviceHost.State, Is.EqualTo(CommunicationState.Opened));

            await sut.StopAsync(default);
        }

        [Test, CustomAutoData]
        public async Task StartAsync_starts_announcement_service([Frozen] IAnnouncementService announcementService, WcfServiceHostedService sut, [Frozen] ServiceHost serviceHost)
        {
            Mock.Get(announcementService).Setup(p => p.AnnounceEndpoints(It.IsAny<IReadOnlyList<ServiceEndpoint>>())).Returns(Disposable.Empty);

            serviceHost.AddServiceEndpoint(typeof(ITestService), new NetNamedPipeBinding(), new Uri("net.pipe://localhost/test"));

            await sut.StartAsync(default);

            await sut.StopAsync(default);

            Mock.Get(announcementService).Verify(p => p.AnnounceEndpoints(It.Is<IReadOnlyList<ServiceEndpoint>>(ses => ses.Any(se => se.Binding is NetNamedPipeBinding))));
        }

        [Test, CustomAutoData]
        public async Task StartAsync_stops_host([Frozen] IServiceHostedServiceConfiguration configuration, WcfServiceHostedService sut, [Frozen] ServiceHost serviceHost)
        {
            Mock.Get(configuration).Setup(p => p.CreateServiceHost()).Returns(serviceHost);

            serviceHost.AddServiceEndpoint(typeof(ITestService), new NetNamedPipeBinding(), new Uri("net.pipe://localhost/test"));

            await sut.StartAsync(default);

            Assume.That(serviceHost.State, Is.EqualTo(CommunicationState.Opened));

            await sut.StopAsync(default);

            Assert.That(serviceHost.State, Is.EqualTo(CommunicationState.Closed));
        }
    }
}