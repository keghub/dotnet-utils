using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EMG.Utilities.ServiceModel.DependencyInjection
{
    public class DependencyInjectionInstanceProvider : IInstanceProvider
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Type _serviceType;
        private readonly ILogger<DependencyInjectionInstanceProvider> _logger;

        public DependencyInjectionInstanceProvider(IServiceProvider serviceProvider, Type serviceType, ILogger<DependencyInjectionInstanceProvider> logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public object GetInstance(InstanceContext instanceContext) => GetInstance(instanceContext, null);

        public object GetInstance(InstanceContext instanceContext, Message message)
        {
            if (instanceContext == null)
            {
                throw new ArgumentNullException(nameof(instanceContext));
            }

            try
            {
                var service = _serviceProvider.GetRequiredService(_serviceType);
                return service;
            }
            catch
            {
                _logger.LogError("Impossible to resolve {SERVICE}", _serviceType.FullName);
                throw;
            }
        }

        public void ReleaseInstance(InstanceContext instanceContext, object instance) { }
    }
}