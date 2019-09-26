using System;
using System.ServiceModel.Description;
using EMG.Utilities.ServiceModel.Configuration;
using EMG.Utilities.ServiceModel.Discovery;

namespace EMG.Utilities.ServiceModel
{
    public static class EndpointExtensions
    {
        public static IEndpoint AddBehavior<TBehavior>(this IEndpoint endpoint, Action<TBehavior> configureBehavior = null) where TBehavior : class, IEndpointBehavior, new()
        {
            var behavior = new TBehavior();
            configureBehavior?.Invoke(behavior);

            return AddBehavior(endpoint, behavior);
        }

        public static IEndpoint AddBehavior(this IEndpoint endpoint, IEndpointBehavior behavior)
        {
            if (endpoint == null)
            {
                throw new ArgumentNullException(nameof(endpoint));
            }

            if (behavior == null)
            {
                throw new ArgumentNullException(nameof(behavior));
            }

            endpoint.Behaviors.Add(behavior);

            return endpoint;
        }

        public static IEndpoint Discoverable(this IEndpoint endpoint)
        {
            return endpoint.AddBehavior(new AnnounceableBehavior());
        }
    }
}