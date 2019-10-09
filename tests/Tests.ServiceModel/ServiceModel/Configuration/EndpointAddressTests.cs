using System;
using EMG.Utilities.ServiceModel.Configuration;
using NUnit.Framework;

namespace Tests.ServiceModel.Configuration
{
    [TestFixture]
    public class EndpointAddressTests
    {
        [Test, CustomAutoData]
        public void Uri_can_be_converted_into_EndpointAddress(Uri testUri)
        {
            EndpointAddress address = testUri;

            Assert.That(address, Is.Not.Null);
            
            Assert.That(address.ToUri(), Is.EqualTo(testUri));
        }

        [Test, CustomAutoData]
        public void ParseFromUri_returns_EndpointAddress(NetTcpEndpointAddress testAddress)
        {
            var testUri = testAddress.ToUri();

            var address = NetTcpEndpointAddress.ParseFromUri(testUri);

            Assert.That(address.Host, Is.EqualTo(testAddress.Host).IgnoreCase);
            Assert.That(address.Port, Is.EqualTo(testAddress.Port));
            Assert.That(address.Path, Is.EqualTo(testAddress.Path).IgnoreCase);
        }

        [Test, CustomAutoData]
        public void ParseFromUri_throws_if_invalid_scheme(NetTcpEndpointAddress testAddress, string invalidScheme)
        {
            var testUri = new UriBuilder(invalidScheme, testAddress.Host, testAddress.Port, testAddress.Path).Uri;

            Assert.Throws<ArgumentException>(() => NetTcpEndpointAddress.ParseFromUri(testUri));
        }

        [Test, CustomAutoData]
        public void ParseFromUri_returns_EndpointAddress(NamedPipeEndpointAddress testAddress)
        {
            var testUri = testAddress.ToUri();

            var address = NamedPipeEndpointAddress.ParseFromUri(testUri);

            Assert.That(address.Path, Is.EqualTo(testAddress.Path).IgnoreCase);
        }

        [Test, CustomAutoData]
        public void ParseFromUri_throws_if_invalid_scheme(NamedPipeEndpointAddress testAddress, string invalidScheme)
        {
            var testUri = new UriBuilder(invalidScheme, "localhost", -1, testAddress.Path).Uri;

            Assert.Throws<ArgumentException>(() => NamedPipeEndpointAddress.ParseFromUri(testUri));
        }

        [Test, CustomAutoData]
        public void ParseFromUri_throws_if_invalid_host(NamedPipeEndpointAddress testAddress, string invalidHost)
        {
            var testUri = new UriBuilder(Uri.UriSchemeNetPipe, invalidHost, -1, testAddress.Path).Uri;

            Assert.Throws<ArgumentException>(() => NamedPipeEndpointAddress.ParseFromUri(testUri));
        }

        [Test, CustomAutoData, Ignore("Impossible to create a URI with scheme net.pipe and custom port")]
        public void ParseFromUri_throws_if_invalid_port(NamedPipeEndpointAddress testAddress, int invalidPort)
        {
            var testUri = new UriBuilder(Uri.UriSchemeNetPipe, "localhost", invalidPort, testAddress.Path).Uri;

            Assert.Throws<ArgumentException>(() => NamedPipeEndpointAddress.ParseFromUri(testUri));
        }

        [Test, CustomAutoData]
        public void ParseFromUri_returns_EndpointAddress(BasicHttpEndpointAddress testAddress)
        {
            var testUri = testAddress.ToUri();

            var address = BasicHttpEndpointAddress.ParseFromUri(testUri);

            Assert.That(address.Host, Is.EqualTo(testAddress.Host).IgnoreCase);
            Assert.That(address.Port, Is.EqualTo(testAddress.Port));
            Assert.That(address.Path, Is.EqualTo(testAddress.Path).IgnoreCase);
        }

        [Test, CustomAutoData]
        public void ParseFromUri_throws_if_invalid_scheme(BasicHttpEndpointAddress testAddress, string invalidScheme)
        {
            var testUri = new UriBuilder(invalidScheme, testAddress.Host, testAddress.Port, testAddress.Path).Uri;

            Assert.Throws<ArgumentException>(() => BasicHttpEndpointAddress.ParseFromUri(testUri));
        }
    }
}