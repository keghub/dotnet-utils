using AutoFixture.Idioms;
using AutoFixture.NUnit3;
using EMG.Utilities.ServiceModel.Discovery;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Reactive.Testing;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Discovery;
using AnnouncementService = EMG.Utilities.ServiceModel.Discovery.AnnouncementService;

namespace Tests.ServiceModel.Discovery
{
    [TestFixture]
    public class AnnouncementServiceTests
    {
        [Test, CustomAutoData]
        public void Constructor_is_guarded(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(AnnouncementService).GetConstructors());
        }

        [Test, CustomAutoData]
        public void AnnounceEndpoints_returns_empty_disposable_if_not_enabled([Frozen] AnnouncementServiceOptions options, AnnouncementService sut, IReadOnlyList<ServiceEndpoint> endpoints)
        {
            options.IsAnnouncementEnabled = false;

            var result = sut.AnnounceEndpoints(endpoints);

            Assert.That(result, Is.SameAs(Disposable.Empty));

            result.Dispose();
        }

        [Test, Ignore("Needs better support for testing"), CustomAutoData]
        public void AnnounceEndpoints_does_stuff([Frozen] AnnouncementServiceOptions options, AnnouncementService sut, ServiceEndpoint testEndpoint, Uri endpointUri)
        {
            testEndpoint.Address = new EndpointAddress(endpointUri);

            options.IsAnnouncementEnabled = true;

            testEndpoint.Behaviors.Add(new AnnounceableBehavior());

            var disposable = sut.AnnounceEndpoints(new[] { testEndpoint });

            disposable.Dispose();
        }

        [Test, CustomAutoData]
        public void AnnounceEndpoints_suppress_exceptions_for_AnnounceOnline(
            [Frozen] AnnouncementServiceOptions options,
            IAnnouncementClientWrapper clientWrapper,
            [Frozen] IAnnouncementClientWrapperFactory clientFactory,
            ILogger<AnnouncementService> logger, ServiceEndpoint testEndpoint, Uri endpointUri)
        {
            options.EndpointDiscoveryMetadata = (_) => new EndpointDiscoveryMetadata();
            options.Interval = TimeSpan.FromSeconds(1);
            options.IsAnnouncementEnabled = true;

            testEndpoint.Address = new EndpointAddress(endpointUri);
            testEndpoint.Behaviors.Add(new AnnounceableBehavior());

            var scheduler = new TestScheduler();

            var sut = new AnnouncementService(new OptionsWrapper<AnnouncementServiceOptions>(options), logger, clientFactory, scheduler);

            Mock.Get(clientWrapper).Setup(i => i.AnnounceOnline(It.IsAny<EndpointDiscoveryMetadata>())).Throws<Exception>();

            Mock.Get(clientFactory).Setup(i => i.Create(It.IsAny<Uri>(), It.IsAny<Binding>())).Returns(clientWrapper);

            Assert.DoesNotThrow(() => sut.AnnounceEndpoints(new[] {testEndpoint}));

            scheduler.AdvanceBy(options.Interval.Ticks);
        }

        [Test, CustomAutoData]
        public void AnnounceEndpoints_suppress_exceptions_for_AnnounceOffline(
            [Frozen] AnnouncementServiceOptions options,
            IAnnouncementClientWrapper clientWrapper,
            [Frozen] IAnnouncementClientWrapperFactory clientFactory,
            ServiceEndpoint testEndpoint, Uri endpointUri,
            AnnouncementService sut)
        {
            options.EndpointDiscoveryMetadata = (_) => new EndpointDiscoveryMetadata();
            options.IsAnnouncementEnabled = true;

            testEndpoint.Address = new EndpointAddress(endpointUri);
            testEndpoint.Behaviors.Add(new AnnounceableBehavior());

            Mock.Get(clientWrapper).Setup(i => i.AnnounceOffline(It.IsAny<EndpointDiscoveryMetadata>())).Throws<Exception>();

            Mock.Get(clientFactory).Setup(i => i.Create(It.IsAny<Uri>(), It.IsAny<Binding>())).Returns(clientWrapper);

            Assert.DoesNotThrow(() =>
            {
                var subscription = sut.AnnounceEndpoints(new[] {testEndpoint});
                subscription.Dispose();
            });

        }
    }
}