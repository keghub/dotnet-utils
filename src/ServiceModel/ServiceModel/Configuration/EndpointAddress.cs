using System;

namespace EMG.Utilities.ServiceModel.Configuration
{
    public abstract class EndpointAddress
    {
        public abstract Uri ToUri();

        public static implicit operator Uri(EndpointAddress address) => address.ToUri();

        public static implicit operator EndpointAddress(Uri uri) => new GenericEndpointAddress(uri);
    }

    public class GenericEndpointAddress : EndpointAddress
    {
        private readonly Uri _uri;

        public GenericEndpointAddress(Uri uri)
        {
            _uri = uri ?? throw new ArgumentNullException(nameof(uri));
        }

        public override Uri ToUri() => _uri;
    }

    public class NetTcpEndpointAddress : EndpointAddress
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public string Path { get; set; }

        public override Uri ToUri()
        {
            var builder = new UriBuilder(Uri.UriSchemeNetTcp, Host, Port, Path);
            return builder.Uri;
        }

        public static NetTcpEndpointAddress ParseFromUri(Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            if (uri.Scheme != Uri.UriSchemeNetTcp)
            {
                throw new ArgumentException("Scheme not valid", nameof(uri));
            }

            return new NetTcpEndpointAddress { Host = uri.Host, Path = uri.AbsolutePath.Substring(1), Port = uri.Port };
        }

        public static implicit operator NetTcpEndpointAddress(Uri uri) => ParseFromUri(uri);
    }

    public class NamedPipeEndpointAddress : EndpointAddress
    {
        public string Path { get; set; }

        public override Uri ToUri()
        {
            var builder = new UriBuilder(Uri.UriSchemeNetPipe, "localhost", -1, Path);
            return builder.Uri;
        }

        public static NamedPipeEndpointAddress ParseFromUri(Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            if (uri.Scheme != Uri.UriSchemeNetPipe)
            {
                throw new ArgumentException("Scheme not valid", nameof(uri));
            }

            if (!uri.IsDefaultPort)
            {
                throw new ArgumentException("Port not valid", nameof(uri));
            }

            if (uri.Host != "localhost")
            {
                throw new ArgumentException("Host not valid", nameof(uri));
            }

            return new NamedPipeEndpointAddress { Path = uri.AbsolutePath.Substring(1) };
        }

        public static implicit operator NamedPipeEndpointAddress(Uri uri) => ParseFromUri(uri);
    }

    public class BasicHttpEndpointAddress : EndpointAddress
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public string Path { get; set; }

        public override Uri ToUri()
        {
            var builder = new UriBuilder(Uri.UriSchemeHttp, Host, Port, Path);
            return builder.Uri;
        }

        public static BasicHttpEndpointAddress ParseFromUri(Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            if (uri.Scheme != Uri.UriSchemeHttp)
            {
                throw new ArgumentException("Scheme not valid", nameof(uri));
            }

            return new BasicHttpEndpointAddress { Host = uri.Host, Path = uri.AbsolutePath.Substring(1), Port = uri.Port };
        }

        public static implicit operator BasicHttpEndpointAddress(Uri uri) => ParseFromUri(uri);
    }

}
