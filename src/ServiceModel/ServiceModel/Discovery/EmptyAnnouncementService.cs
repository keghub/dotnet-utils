using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.ServiceModel.Description;

namespace EMG.Utilities.ServiceModel.Discovery
{
    public class EmptyAnnouncementService : IAnnouncementService
    {
        public IDisposable AnnounceEndpoints(IReadOnlyList<ServiceEndpoint> endpoints)
        {
            return Disposable.Empty;
        }
    }
}