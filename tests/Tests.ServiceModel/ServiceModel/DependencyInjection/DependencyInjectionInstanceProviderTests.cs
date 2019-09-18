using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using AutoFixture.Idioms;
using AutoFixture.NUnit3;
using EMG.Utilities.ServiceModel.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace Tests.ServiceModel.DependencyInjection
{
    [TestFixture]
    public class DependencyInjectionInstanceProviderTests
    {
        [Test, CustomAutoData]
        public void Constructor_is_guarded(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(DependencyInjectionInstanceProvider).GetConstructors());
        }

        [Test, CustomAutoData]
        public void GetInstance_returns_service_instance_from_provider([Frozen] IServiceProvider serviceProvider, [Frozen] Type serviceType, TestService serviceInstance, DependencyInjectionInstanceProvider sut, InstanceContext instanceContext, Message message)
        {
            Mock.Get(serviceProvider).Setup(p => p.GetService(serviceType)).Returns(serviceInstance);

            var result = sut.GetInstance(instanceContext, message);

            Assert.That(result, Is.SameAs(serviceInstance));
        }

        [Test, CustomAutoData]
        public void GetInstance_returns_service_instance_from_provider([Frozen] IServiceProvider serviceProvider, [Frozen] Type serviceType, TestService serviceInstance, DependencyInjectionInstanceProvider sut, InstanceContext instanceContext)
        {
            Mock.Get(serviceProvider).Setup(p => p.GetService(serviceType)).Returns(serviceInstance);

            var result = sut.GetInstance(instanceContext);

            Assert.That(result, Is.SameAs(serviceInstance));
        }

        [Test, CustomAutoData]
        public void GetInstance_throws_if_instanceContext_is_null(DependencyInjectionInstanceProvider sut, Message message)
        {
            Assert.Throws<ArgumentNullException>(() => sut.GetInstance(null, message));
        }

        [Test, CustomAutoData]
        public void GetInstance_throws_if_instanceContext_is_null(DependencyInjectionInstanceProvider sut)
        {
            Assert.Throws<ArgumentNullException>(() => sut.GetInstance(null));
        }

        [Test]
        [CustomInlineAutoData()]
        [CustomInlineAutoData(null)]
        [CustomInlineAutoData(null, null)]
        public void ReleaseInstance_does_not_throw(InstanceContext instanceContext, Message message, DependencyInjectionInstanceProvider sut)
        {
            Assert.DoesNotThrow(() => sut.ReleaseInstance(instanceContext, message));
        }

        [Test, CustomAutoData]
        public void GetInstance_throws_if_service_cannot_be_resolved([Frozen] IServiceProvider serviceProvider, [Frozen] Type serviceType, TestService serviceInstance, DependencyInjectionInstanceProvider sut, InstanceContext instanceContext, Message message, Exception exception)
        {
            Mock.Get(serviceProvider).Setup(p => p.GetService(serviceType)).Throws(exception);

            Assert.Throws<Exception>(() => sut.GetInstance(instanceContext, message));
        }

        [Test, CustomAutoData]
        public void GetInstance_throws_if_service_cannot_be_resolved([Frozen] IServiceProvider serviceProvider, [Frozen] Type serviceType, TestService serviceInstance, DependencyInjectionInstanceProvider sut, InstanceContext instanceContext, Exception exception)
        {
            Mock.Get(serviceProvider).Setup(p => p.GetService(serviceType)).Throws(exception);

            Assert.Throws<Exception>(() => sut.GetInstance(instanceContext));
        }
    }
}