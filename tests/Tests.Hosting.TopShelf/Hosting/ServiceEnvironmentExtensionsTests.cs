using System.Collections.Generic;
using AutoFixture.NUnit3;
using EMG.Utilities.Hosting;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace Tests.Hosting
{
    [TestFixture]
    public class ServiceEnvironmentExtensionsTests
    {
        [Test, AutoMoqData]
        public void GetServiceEnvironment_can_build_valid_ServiceEnvironment(ConfigurationBuilder configurationBuilder, string environmentName)
        {
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Environment"] = environmentName
            });

            var configuration = configurationBuilder.Build();

            var serviceEnvironment = configuration.GetServiceEnvironment();

            Assert.That(serviceEnvironment, Is.Not.Null);
            Assert.That(serviceEnvironment.EnvironmentName, Is.EqualTo(environmentName));
            Assert.That(serviceEnvironment.Configuration, Is.SameAs(configuration));
        }

        [Test, AutoMoqData]
        public void IsEnvironment_returns_true_if_environment_is_correct([Frozen] string environmentName, ServiceEnvironment sut)
        {
            Assume.That(sut.EnvironmentName, Is.EqualTo(environmentName));

            Assert.That(sut.IsEnvironment(environmentName), Is.True);
        }

        [Test]
        public void IsProduction_returns_true_if_environment_is_correct()
        {
            var environment = new ServiceEnvironment { EnvironmentName = "Production" };

            Assert.That(environment.IsProduction(), Is.True);
        }

        [Test]
        public void IsDevelopment_returns_true_if_environment_is_correct()
        {
            var environment = new ServiceEnvironment { EnvironmentName = "Development" };

            Assert.That(environment.IsDevelopment(), Is.True);
        }

        [Test]
        public void IsStaging_returns_true_if_environment_is_correct()
        {
            var environment = new ServiceEnvironment { EnvironmentName = "Staging" };

            Assert.That(environment.IsStaging(), Is.True);
        }
    }
}