using System;
using System.Collections.Generic;
using System.ServiceModel.Description;

namespace EMG.Utilities.ServiceModel.Discovery
{
    public interface IAnnouncementService
    {
        IDisposable AnnounceEndpoints(IReadOnlyList<ServiceEndpoint> endpoints);
    }
}