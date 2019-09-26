using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Microsoft.Extensions.Logging;

namespace EMG.Utilities.ServiceModel.Configuration
{
    public interface IServiceHostConfigurator
    {
        IEndpoint AddEndpoint<TBinding>(Type contract, Uri address, Action<TBinding> configureBinding = null)
            where TBinding : Binding, new();

        IList<Action<ServiceHost>> ServiceHostConfigurations { get; }

        ILoggerFactory LoggerFactory { get; }
    }

    public interface IServiceHostConfigurator<TService> : IServiceHostConfigurator
        where TService : class
    {

    }
}