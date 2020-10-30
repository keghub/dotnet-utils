using System;
using System.ServiceModel.Channels;

namespace EMG.Utilities.ServiceModel.Discovery
{
    public interface IAnnouncementClientWrapperFactory
    {
        IAnnouncementClientWrapper Create(Uri registryUrl, Binding binding);
    }
}