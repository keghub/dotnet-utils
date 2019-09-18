using System.ServiceModel;
using System.ServiceModel.Description;
using AutoFixture.Idioms;
using EMG.Utilities.ServiceModel.DependencyInjection;
using NUnit.Framework;

namespace Tests.ServiceModel.DependencyInjection
{
    [TestFixture]
    public class DependencyInjectionServiceBehaviorTests
    {
        [Test, CustomAutoData]
        public void Constructor_is_guarded(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(DependencyInjectionServiceBehavior).GetConstructors());
        }

        [Test]
        [CustomInlineAutoData]
        [CustomInlineAutoData(null)]
        [CustomInlineAutoData(null, null)]
        public void Validate_does_not_throw(ServiceDescription description, ServiceHost host, DependencyInjectionServiceBehavior sut)
        {
            Assert.DoesNotThrow(() => sut.Validate(description, host));
        }

        [Test,CustomAutoData]
        public void AddBindingParameters_does_not_throw(DependencyInjectionServiceBehavior sut)
        {
            Assert.DoesNotThrow(() => sut.AddBindingParameters(null, null, null, null));
        }
    }
}