using System;
using System.ServiceModel;
using EMG.Utilities.ServiceModel;
using Moq;
using NUnit.Framework;
// ReSharper disable InvokeAsExtensionMethod

namespace Tests.ServiceModel
{
    [TestFixture]
    public class BasicHttpBindingExtensionsTests
    {
        [Test, CustomAutoData]
        public void UseHttps_sets_security_mode_to_Transport(BasicHttpBinding binding)
        {
            BindingExtensions.UseHttps(binding);

            Assert.That(binding.Security.Mode, Is.EqualTo(BasicHttpSecurityMode.Transport));
        }

        [Test, CustomAutoData]
        public void UseHttps_uses_delegate_to_configure_transport_node(BasicHttpBinding binding, Action<HttpTransportSecurity> testDelegate)
        {
            BindingExtensions.UseHttps(binding, testDelegate);

            Mock.Get(testDelegate).Verify(p => p(binding.Security.Transport));
        }

        [Test, CustomAutoData]
        public void UseHttps_returns_binding(BasicHttpBinding binding)
        {
            var result = BindingExtensions.UseHttps(binding);

            Assert.That(result, Is.SameAs(binding));
        }

        [Test, CustomAutoData]
        public void WithNoSecurity_sets_security_mode_to_None(BasicHttpBinding binding)
        {
            BindingExtensions.WithNoSecurity(binding);

            Assert.That(binding.Security.Mode, Is.EqualTo(BasicHttpSecurityMode.None));
        }

        [Test, CustomAutoData]
        public void WithNoSecurity_returns_binding(BasicHttpBinding binding)
        {
            var result = BindingExtensions.WithNoSecurity(binding);

            Assert.That(result, Is.SameAs(binding));
        }
    }

    [TestFixture]
    public class NetTcpBindingExtensionsTests
    {
        [Test, CustomAutoData]
        public void UseHttps_sets_security_mode_to_Transport(NetTcpBinding binding)
        {
            BindingExtensions.UseSecureChannel(binding);

            Assert.That(binding.Security.Mode, Is.EqualTo(SecurityMode.Transport));
        }

        [Test, CustomAutoData]
        public void UseHttps_uses_delegate_to_configure_transport_node(NetTcpBinding binding, Action<TcpTransportSecurity> testDelegate)
        {
            BindingExtensions.UseSecureChannel(binding, testDelegate);

            Mock.Get(testDelegate).Verify(p => p(binding.Security.Transport));
        }

        [Test, CustomAutoData]
        public void UseHttps_returns_binding(NetTcpBinding binding)
        {
            var result = BindingExtensions.UseSecureChannel(binding);

            Assert.That(result, Is.SameAs(binding));
        }

        [Test, CustomAutoData]
        public void WithNoSecurity_sets_security_mode_to_None(NetTcpBinding binding)
        {
            BindingExtensions.WithNoSecurity(binding);

            Assert.That(binding.Security.Mode, Is.EqualTo(SecurityMode.None));

        }

        [Test, CustomAutoData]
        public void WithNoSecurity_returns_binding(NetTcpBinding binding)
        {
            var result = BindingExtensions.WithNoSecurity(binding);

            Assert.That(result, Is.SameAs(binding));
        }
    }

    [TestFixture]
    public class NetNamedPipeBindingExtensionsTests
    {
        [Test, CustomAutoData]
        public void UseHttps_sets_security_mode_to_Transport(NetNamedPipeBinding binding)
        {
            BindingExtensions.UseSecureChannel(binding);

            Assert.That(binding.Security.Mode, Is.EqualTo(NetNamedPipeSecurityMode.Transport));
        }

        [Test, CustomAutoData]
        public void UseHttps_uses_delegate_to_configure_transport_node(NetNamedPipeBinding binding, Action<NamedPipeTransportSecurity> testDelegate)
        {
            BindingExtensions.UseSecureChannel(binding, testDelegate);

            Mock.Get(testDelegate).Verify(p => p(binding.Security.Transport));
        }

        [Test, CustomAutoData]
        public void UseHttps_returns_binding(NetNamedPipeBinding binding)
        {
            var result = BindingExtensions.UseSecureChannel(binding);

            Assert.That(result, Is.SameAs(binding));
        }

        [Test, CustomAutoData]
        public void WithNoSecurity_sets_security_mode_to_None(NetNamedPipeBinding binding)
        {
            BindingExtensions.WithNoSecurity(binding);

            Assert.That(binding.Security.Mode, Is.EqualTo(NetNamedPipeSecurityMode.None));

        }

        [Test, CustomAutoData]
        public void WithNoSecurity_returns_binding(NetNamedPipeBinding binding)
        {
            var result = BindingExtensions.WithNoSecurity(binding);

            Assert.That(result, Is.SameAs(binding));
        }
    }
}