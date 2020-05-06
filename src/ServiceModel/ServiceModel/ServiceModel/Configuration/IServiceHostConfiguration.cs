using System;
using System.ServiceModel;

namespace EMG.Utilities.ServiceModel.Configuration
{
    public interface IServiceHostConfiguration
    {
        void ConfigureServiceHost(ServiceHost serviceHost);

        Type ServiceType { get; }
    }

    public interface IServiceHostConfiguration<TService> : IServiceHostConfiguration
    {

    }
}