using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EMG.Utilities.ServiceModel.DependencyInjection
{
    public class DependencyInjectionServiceBehavior : IServiceBehavior
    {
        private readonly IServiceProvider _serviceProvider;

        public DependencyInjectionServiceBehavior(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase) { }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters) { }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            var logger = _serviceProvider.GetRequiredService<ILogger<DependencyInjectionInstanceProvider>>();
            var instanceProvider = new DependencyInjectionInstanceProvider(_serviceProvider, serviceDescription.ServiceType, logger);

            foreach (var dispatcher in serviceHostBase.ChannelDispatchers)
            {
                if (dispatcher is ChannelDispatcher channelDispatcher)
                {
                    foreach (var endpointDispatcher in channelDispatcher.Endpoints)
                    {
                        endpointDispatcher.DispatchRuntime.InstanceProvider = instanceProvider;
                    }
                }
            }
        }
    }

}