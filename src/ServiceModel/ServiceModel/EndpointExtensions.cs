using EMG.Utilities.ServiceModel.Configuration;
using EMG.Utilities.ServiceModel.Discovery;

namespace EMG.Utilities.ServiceModel
{
    public static class EndpointExtensions
    {
        public static IEndpoint Discoverable(this IEndpoint endpoint)
        {
            endpoint.Behaviors.Add(new AnnounceableEndpointBehavior());

            return endpoint;
        }
    }
}