using System;
using System.Collections.Generic;
using AutoFixture.Idioms;
using EMG.Utilities.ServiceModel.Configuration;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using ConfigurationExtensions = EMG.Utilities.ServiceModel.ConfigurationExtensions;

// ReSharper disable InvokeAsExtensionMethod

namespace Tests.ServiceModel
{
    [TestFixture]
    public class ConfigurationExtensionsTests
    {
        [Test, CustomAutoData]
        public void GetEndpointAddress_is_guarded_against_null(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(ConfigurationExtensions).GetMethod(nameof(ConfigurationExtensions.GetEndpointAddress), new[] { typeof(IConfigurationSection) }));
        }

        [Test, CustomAutoData]
        public void GetNetTcpEndpointAddress_can_return_endpoint(ConfigurationBuilder builder, NetTcpEndpointAddress testAddress)
        {
            var configuration = builder.AddInMemoryCollection(new Dictionary<string, string>()
            {
                ["Host"] = testAddress.Host, 
                ["Port"] = testAddress.Port.ToString(), 
                ["Path"] = testAddress.Path
            }).Build();

            Uri result = ConfigurationExtensions.GetNetTcpEndpointAddress(configuration);

            Assert.That(result.Scheme, Is.EqualTo(Uri.UriSchemeNetTcp));
            Assert.That(result.Host, Is.EqualTo(testAddress.Host).IgnoreCase);
            Assert.That(result.Port, Is.EqualTo(testAddress.Port));
            Assert.That(result.AbsolutePath, Is.EqualTo("/" + testAddress.Path).IgnoreCase);
        }

        [Test, CustomAutoData]
        public void GetBasicHttpEndpointAddress_can_return_endpoint(ConfigurationBuilder builder, HttpEndpointAddress testAddress)
        {
            var configuration = builder.AddInMemoryCollection(new Dictionary<string, string>()
            {
                ["Host"] = testAddress.Host,
                ["Port"] = testAddress.Port.ToString(),
                ["Path"] = testAddress.Path
            }).Build();

            Uri result = ConfigurationExtensions.GetBasicHttpEndpointAddress(configuration);

            Assert.That(result.Scheme, Is.EqualTo(Uri.UriSchemeHttp));
            Assert.That(result.Host, Is.EqualTo(testAddress.Host).IgnoreCase);
            Assert.That(result.Port, Is.EqualTo(testAddress.Port));
            Assert.That(result.AbsolutePath, Is.EqualTo("/" + testAddress.Path).IgnoreCase);
        }

        [Test, CustomAutoData]
        public void GetNamedPipeEndpointAddress_can_return_endpoint(ConfigurationBuilder builder, NamedPipeEndpointAddress testAddress)
        {
            var configuration = builder.AddInMemoryCollection(new Dictionary<string, string>()
            {
                ["Path"] = testAddress.Path
            }).Build();

            Uri result = ConfigurationExtensions.GetNamedPipeEndpointAddress(configuration);

            Assert.That(result.Scheme, Is.EqualTo(Uri.UriSchemeNetPipe));
            Assert.That(result.Host, Is.EqualTo("localhost"));
            Assert.That(result.IsDefaultPort, Is.True);
            Assert.That(result.AbsolutePath, Is.EqualTo("/" + testAddress.Path).IgnoreCase);
        }
    }
}
