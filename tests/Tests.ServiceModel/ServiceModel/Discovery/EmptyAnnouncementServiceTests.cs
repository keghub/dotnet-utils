using System.Collections.Generic;
using System.Reactive.Disposables;
using System.ServiceModel.Description;
using AutoFixture.Idioms;
using EMG.Utilities.ServiceModel.Discovery;
using NUnit.Framework;

namespace Tests.ServiceModel.Discovery
{
    [TestFixture]
    public class EmptyAnnouncementServiceTests
    {
        [Test, CustomAutoData]
        public void Constructor_is_guarded(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(EmptyAnnouncementService).GetConstructors());
        }

        [Test, CustomAutoData]
        public void AnnounceEndpoints_returns_empty_disposable(EmptyAnnouncementService sut, IReadOnlyList<ServiceEndpoint> endpoints)
        {
            var result = sut.AnnounceEndpoints(endpoints);

            Assert.That(result, Is.SameAs(Disposable.Empty));

            result.Dispose();
        }
    }
}