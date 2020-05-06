using System;
using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace EMG.Utilities.ServiceModel.Configuration
{
    public interface IEndpoint
    {
        EndpointAddress Address { get; }

        Type Contract { get; }

        Binding Binding { get; }

        IList<IEndpointBehavior> Behaviors { get; }
    }

    public class WcfEndpoint : IEndpoint
    {
        public WcfEndpoint(Type contract, EndpointAddress address, Binding binding)
        {
            Contract = contract;
            Address = address;
            Binding = binding;
            Behaviors = new List<IEndpointBehavior>();
        }

        public Type Contract { get; }

        public EndpointAddress Address { get; }

        public Binding Binding { get; }

        public IList<IEndpointBehavior> Behaviors { get; }
    }

}