using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using EMG.Utilities.ServiceModel.Configuration;
using EMG.Utilities.ServiceModel.Logging;

namespace EMG.Utilities.ServiceModel
{
    public static class ServiceHostConfiguratorExtensions
    {
        public static void AddMetadataEndpoints(this IServiceHostConfigurator configurator, Action<ServiceMetadataBehavior> serviceMetadataBehaviorConfigurator = null, string endpointAddress = "mex")
        {
            if (configurator is null)
            {
                throw new ArgumentNullException(nameof(configurator));
            }

            configurator.ServiceHostConfigurations.Add(host =>
            {
                var behavior = host.Description.Behaviors.Find<ServiceMetadataBehavior>();

                if (behavior == null)
                {
                    behavior = new ServiceMetadataBehavior();

                    host.Description.Behaviors.Add(behavior);
                }

                var hasSupportedEndpoint = false;

                if (TryFindEndpointByBinding<WSHttpBinding>(host, out var wsEndpoint))
                {
                    var address = new Uri($"{wsEndpoint.Address.Uri}/{endpointAddress}");

                    behavior.HttpGetEnabled = true;
                    behavior.HttpGetUrl = address;

                    host.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName, MetadataExchangeBindings.CreateMexHttpBinding(), address);

                    hasSupportedEndpoint = true;
                }

                if (TryFindEndpointByBinding<BasicHttpBinding>(host, out var httpEndpoint))
                {
                    var address = new Uri($"{httpEndpoint.Address.Uri}/{endpointAddress}");

                    behavior.HttpGetEnabled = true;
                    behavior.HttpGetUrl = address;

                    host.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName, new BasicHttpBinding(), address);

                    hasSupportedEndpoint = true;
                }

                if (TryFindEndpointByBinding<NetNamedPipeBinding>(host, out var netNamedEndpoint))
                {
                    host.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName, MetadataExchangeBindings.CreateMexNamedPipeBinding(), $"{netNamedEndpoint.Address.Uri}/{endpointAddress}");

                    hasSupportedEndpoint = true;
                }

                if (TryFindEndpointByBinding<NetTcpBinding>(host, out var netTcpEndpoint))
                {
                    host.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName, MetadataExchangeBindings.CreateMexTcpBinding(), $"{netTcpEndpoint.Address.Uri}/{endpointAddress}");

                    hasSupportedEndpoint = true;
                }

                if (!hasSupportedEndpoint)
                {
                    throw new InvalidOperationException("No supported endpoint has been found.");
                }

                serviceMetadataBehaviorConfigurator?.Invoke(behavior);
            });
        }

        public static void ConfigureService(this IServiceHostConfigurator configurator, Action<ServiceBehaviorAttribute> configureService)
        {
            if (configurator is null)
            {
                throw new ArgumentNullException(nameof(configurator));
            }

            if (configureService is null)
            {
                throw new ArgumentNullException(nameof(configureService));
            }

            configurator.ServiceHostConfigurations.Add(host =>
            {
                var serviceBehavior = host.Description.Behaviors.Find<ServiceBehaviorAttribute>();
                configureService(serviceBehavior);
            });
        }

        public static void AcceptAllIncomingRequests(this IServiceHostConfigurator configurator)
        {
            ConfigureService(configurator, svc => svc.AddressFilterMode = AddressFilterMode.Any);
        }

        public static void AddServiceBehavior(this IServiceHostConfigurator configurator, IServiceBehavior behavior)
        {
            if (configurator is null)
            {
                throw new ArgumentNullException(nameof(configurator));
            }

            if (behavior is null)
            {
                throw new ArgumentNullException(nameof(behavior));
            }

            configurator.ServiceHostConfigurations.Add(host => host.Description.Behaviors.Add(behavior));
        }

        public static void AddExecutionLogging(this IServiceHostConfigurator configurator)
        {
            if (configurator is null)
            {
                throw new ArgumentNullException(nameof(configurator));
            }

            configurator.AddServiceBehavior(new ExecutionLoggingBehavior(configurator.LoggerFactory));
        }

        private static bool TryFindEndpointByBinding<TBinding>(ServiceHost host, out ServiceEndpoint endpoint)
        {
            endpoint = host.Description.Endpoints.FirstOrDefault(e => e.Binding is TBinding);
            return endpoint != null;
        }

        public static IEndpoint AddBasicHttpEndpoint(this IServiceHostConfigurator configurator, Type contract, HttpEndpointAddress address, Action<BasicHttpBinding> configureBinding = null) => configurator.AddEndpoint(contract, address, configureBinding);

        public static IEndpoint AddSecureBasicHttpEndpoint(this IServiceHostConfigurator configurator, Type contract, HttpEndpointAddress address, Action<BasicHttpBinding> configureBinding = null)
        {
            if (configurator is null)
            {
                throw new ArgumentNullException(nameof(configurator));
            }

            if (contract is null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            if (address is null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            var secureAddress = new HttpEndpointAddress { Host = address.Host, IsSecure = true, Path = address.Path, Port = address.Port };
            return configurator.AddBasicHttpEndpoint(contract, secureAddress, ConfigureBinding);

            void ConfigureBinding(BasicHttpBinding binding)
            {
                binding.UseHttps();
                configureBinding?.Invoke(binding);
            }
        }

        public static IEndpoint AddWSHttpEndpoint(this IServiceHostConfigurator configurator, Type contract, HttpEndpointAddress address, Action<WSHttpBinding> configureBinding = null) => configurator.AddEndpoint(contract, address, configureBinding);

        public static IEndpoint AddSecureWSHttpEndpoint(this IServiceHostConfigurator configurator, Type contract, HttpEndpointAddress address, Action<WSHttpBinding> configureBinding = null)
        {
            if (configurator is null)
            {
                throw new ArgumentNullException(nameof(configurator));
            }

            if (contract is null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            if (address is null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            var secureAddress = new HttpEndpointAddress { Host = address.Host, IsSecure = true, Path = address.Path, Port = address.Port };
            return configurator.AddWSHttpEndpoint(contract, secureAddress, ConfigureBinding);

            void ConfigureBinding(WSHttpBinding binding)
            {
                binding.UseHttps();
                configureBinding?.Invoke(binding);
            }
        }

        public static IEndpoint AddNetTcpEndpoint(this IServiceHostConfigurator configurator, Type contract, NetTcpEndpointAddress address, Action<NetTcpBinding> configureBinding = null) => configurator.AddEndpoint(contract, address, configureBinding);

        public static IEndpoint AddNamedPipeEndpoint(this IServiceHostConfigurator configurator, Type contract, NamedPipeEndpointAddress address, Action<NetNamedPipeBinding> configureBinding = null) => configurator.AddEndpoint(contract, address, configureBinding);
    }

}