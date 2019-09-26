using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.ServiceModel;
using System.ServiceModel.Description;
using AutoFixture.Idioms;
using AutoFixture.NUnit3;
using EMG.Utilities.ServiceModel.Discovery;
using NUnit.Framework;

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
    }
}