using System;
using AutoFixture.Idioms;
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
        public void Valid_URI_can_be_cast_to_EndpointAddress(NetTcpEndpointAddress testAddress)
        {
            var testUri = testAddress.ToUri();

            NetTcpEndpointAddress address = testUri;

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
        public void ParseFromUri_throws_if_uri_is_null(NetTcpEndpointAddress _)
        {
            Assert.Throws<ArgumentNullException>(() => NetTcpEndpointAddress.ParseFromUri(null));
        }

        [Test, CustomAutoData]
        public void ToString_returns_formed_Uri(NetTcpEndpointAddress testAddress)
        {
            Assert.That(testAddress.ToString(), Is.EqualTo($"Address: {testAddress.ToUri()}"));
        }

        [Test, CustomAutoData]
        public void ParseFromUri_returns_EndpointAddress(NamedPipeEndpointAddress testAddress)
        {
            var testUri = testAddress.ToUri();

            var address = NamedPipeEndpointAddress.ParseFromUri(testUri);

            Assert.That(address.Path, Is.EqualTo(testAddress.Path).IgnoreCase);
        }

        [Test, CustomAutoData]
        public void Valid_URI_can_be_cast_to_EndpointAddress(NamedPipeEndpointAddress testAddress)
        {
            var testUri = testAddress.ToUri();

            NamedPipeEndpointAddress address = testUri;

            Assert.That(address.Path, Is.EqualTo(testAddress.Path).IgnoreCase);
        }

        [Test, CustomAutoData]
        public void ParseFromUri_throws_if_invalid_scheme(NamedPipeEndpointAddress testAddress, string invalidScheme)
        {
            var testUri = new UriBuilder(invalidScheme, "localhost", -1, testAddress.Path).Uri;

            Assert.Throws<ArgumentException>(() => NamedPipeEndpointAddress.ParseFromUri(testUri));
        }

        [Test, CustomAutoData]
        public void ParseFromUri_throws_if_uri_is_null(NamedPipeEndpointAddress _)
        {
            Assert.Throws<ArgumentNullException>(() => NamedPipeEndpointAddress.ParseFromUri(null));
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
        public void ToString_returns_formed_Uri(NamedPipeEndpointAddress testAddress)
        {
            Assert.That(testAddress.ToString(), Is.EqualTo($"Address: {testAddress.ToUri()}"));
        }

        [Test, CustomAutoData]
        public void ParseFromUri_returns_EndpointAddress(HttpEndpointAddress testAddress)
        {
            var testUri = testAddress.ToUri();

            var address = HttpEndpointAddress.ParseFromUri(testUri);

            Assert.That(address.Host, Is.EqualTo(testAddress.Host).IgnoreCase);
            Assert.That(address.Port, Is.EqualTo(testAddress.Port));
            Assert.That(address.Path, Is.EqualTo(testAddress.Path).IgnoreCase);
            Assert.That(address.IsSecure, Is.EqualTo(testAddress.IsSecure));
        }

        [Test, CustomAutoData]
        public void Valid_URI_can_be_cast_to_EndpointAddress(HttpEndpointAddress testAddress)
        {
            var testUri = testAddress.ToUri();

            HttpEndpointAddress address = testUri;

            Assert.That(address.Host, Is.EqualTo(testAddress.Host).IgnoreCase);
            Assert.That(address.Port, Is.EqualTo(testAddress.Port));
            Assert.That(address.Path, Is.EqualTo(testAddress.Path).IgnoreCase);
            Assert.That(address.IsSecure, Is.EqualTo(testAddress.IsSecure));
        }

        [Test, CustomAutoData]
        public void ParseFromUri_throws_if_invalid_scheme(HttpEndpointAddress testAddress, string invalidScheme)
        {
            var testUri = new UriBuilder(invalidScheme, testAddress.Host, testAddress.Port, testAddress.Path).Uri;

            Assert.Throws<ArgumentException>(() => HttpEndpointAddress.ParseFromUri(testUri));
        }

        [Test, CustomAutoData]
        public void ParseFromUri_throws_if_uri_is_null(HttpEndpointAddress _)
        {
            Assert.Throws<ArgumentNullException>(() => HttpEndpointAddress.ParseFromUri(null));
        }

        [Test, CustomAutoData]
        public void ToString_returns_formed_Uri(HttpEndpointAddress testAddress)
        {
            Assert.That(testAddress.ToString(), Is.EqualTo($"Address: {testAddress.ToUri()}"));
        }

        [Test, CustomAutoData]
        public void Factory_method_ForBasicHttp_is_guarded(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(EndpointAddress).GetMethod(nameof(EndpointAddress.ForHttp)));
        }

        [Test, CustomAutoData]
        public void Factory_method_ForNetTcp_is_guarded(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(EndpointAddress).GetMethod(nameof(EndpointAddress.ForNetTcp)));
        }

        [Test, CustomAutoData]
        public void Factory_method_ForNamedPipe_is_guarded(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(EndpointAddress).GetMethod(nameof(EndpointAddress.ForNamedPipe)));
        }

        [Test]
        [CustomInlineAutoData(true)]
        [CustomInlineAutoData(false)]
        public void Factory_method_ForHttp_returns_constructed_object(bool isSecure, string host, string path, int port)
        {
            var endpointAddress = EndpointAddress.ForHttp(host, path, port, isSecure);

            Assert.That(endpointAddress.Host, Is.EqualTo(host).IgnoreCase);
            Assert.That(endpointAddress.Path, Is.EqualTo(path).IgnoreCase);
            Assert.That(endpointAddress.Port, Is.EqualTo(port));
            Assert.That(endpointAddress.IsSecure, Is.EqualTo(isSecure));
        }

        [Test, CustomAutoData]
        public void Factory_method_ForNetTcp_returns_constructed_object(string host, string path, int port)
        {
            var endpointAddress = EndpointAddress.ForNetTcp(port, host, path);

            Assert.That(endpointAddress.Host, Is.EqualTo(host).IgnoreCase);
            Assert.That(endpointAddress.Path, Is.EqualTo(path).IgnoreCase);
            Assert.That(endpointAddress.Port, Is.EqualTo(port));
        }

        [Test, CustomAutoData]
        public void Factory_method_ForNamedPipe_returns_constructed_object(string path)
        {
            var endpointAddress = EndpointAddress.ForNamedPipe(path);

            Assert.That(endpointAddress.Path, Is.EqualTo(path).IgnoreCase);
        }

        [Test, CustomAutoData]
        public void Unsupported_Uri_scheme_are_cast_to_EndpointAddress(string host, string path)
        {
            var uri = new UriBuilder(Uri.UriSchemeFtp, host, -1, path).Uri;

            EndpointAddress address = uri;

            Assert.That(address, Is.InstanceOf<GenericEndpointAddress>());
            Assert.That(address.ToUri(), Is.EqualTo(uri));
        }

    }
}