using System;
using System.ServiceModel;
using EMG.Utilities.ServiceModel.DependencyInjection;

namespace EMG.Utilities.ServiceModel.Hosting
{
    public interface IServiceHostBuilder
    {
        ServiceHost Build(Type serviceType);
    }

    public class ServiceHostBuilder : IServiceHostBuilder
    {
        private readonly IServiceProvider _serviceProvider;

        public ServiceHostBuilder(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public ServiceHost Build(Type serviceType)
        {
            var serviceHost = new ServiceHost(serviceType);

            serviceHost.Description.Behaviors.Add(new DependencyInjectionServiceBehavior(_serviceProvider));

            return serviceHost;
        }
    }

}