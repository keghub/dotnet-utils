using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace EMG.Utilities.ServiceModel.Configuration
{
    public abstract class EndpointAddress
    {
        public abstract Uri ToUri();

        public abstract bool CanSupportBinding(Binding binding);

        public override string ToString()
        {
            return ToUri().ToString();
        }

        public static implicit operator Uri(EndpointAddress address) => address.ToUri();

        public static implicit operator EndpointAddress(Uri uri)
        {
            switch (uri.Scheme)
            {
                case "https":
                case "http":
                    return HttpEndpointAddress.ParseFromUri(uri);
                case "net.tcp":
                    return NetTcpEndpointAddress.ParseFromUri(uri);
                case "net.pipe":
                    return NamedPipeEndpointAddress.ParseFromUri(uri);
                default:
                    return new GenericEndpointAddress(uri);
            }
        }

        public static NetTcpEndpointAddress ForNetTcp(int port, string host = "localhost", string path = "")
        {
            if (host == null)
            {
                throw new ArgumentNullException(nameof(host));
            }

            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            return new NetTcpEndpointAddress { Host = host, Path = path, Port = port };
        }

        public static NamedPipeEndpointAddress ForNamedPipe(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            return new NamedPipeEndpointAddress { Path = path };
        }

        public static HttpEndpointAddress ForHttp(string host = "localhost", string path = "", int port = -1, bool isSecure = false)
        {
            if (host == null)
            {
                throw new ArgumentNullException(nameof(host));
            }

            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            return new HttpEndpointAddress { Host = host, Path = path, Port = port, IsSecure = isSecure };
        }
    }

    public class GenericEndpointAddress : EndpointAddress
    {
        private readonly Uri _uri;

        public GenericEndpointAddress(Uri uri)
        {
            _uri = uri ?? throw new ArgumentNullException(nameof(uri));
        }

        public override Uri ToUri() => _uri;

        public override bool CanSupportBinding(Binding binding) => string.Equals(binding.Scheme, _uri.Scheme, StringComparison.OrdinalIgnoreCase);
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

        public override bool CanSupportBinding(Binding binding) => binding is NetTcpBinding;

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

        public override bool CanSupportBinding(Binding binding) => binding is NetNamedPipeBinding;

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

    public class HttpEndpointAddress : EndpointAddress
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public string Path { get; set; }

        public bool IsSecure { get; set; }

        public override Uri ToUri()
        {
            var scheme = IsSecure ? Uri.UriSchemeHttps : Uri.UriSchemeHttp;
            var builder = new UriBuilder(scheme, Host, Port, Path);

            return builder.Uri;
        }

        public override bool CanSupportBinding(Binding binding) => binding is BasicHttpBinding || binding is WSHttpBinding;

        public static HttpEndpointAddress ParseFromUri(Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
            {
                throw new ArgumentException("Scheme not valid", nameof(uri));
            }

            return new HttpEndpointAddress { Host = uri.Host, Path = uri.AbsolutePath.Substring(1), Port = uri.Port, IsSecure = uri.Scheme == Uri.UriSchemeHttps };
        }

        public static implicit operator HttpEndpointAddress(Uri uri) => ParseFromUri(uri);
    }

}
