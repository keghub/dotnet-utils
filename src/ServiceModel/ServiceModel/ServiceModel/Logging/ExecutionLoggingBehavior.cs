using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Microsoft.Extensions.Logging;

namespace EMG.Utilities.ServiceModel.Logging
{
    public class ExecutionLoggingBehavior : IServiceBehavior, IOperationBehavior
    {
        private readonly ILoggerFactory _loggerFactory;

        public ExecutionLoggingBehavior(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (var endpoint in serviceDescription.Endpoints)
            {
                foreach (var operation in endpoint.Contract.Operations)
                {
                    if (operation.Behaviors.Find<ExecutionLoggingBehavior>() == null)
                    {
                        operation.Behaviors.Add(this);
                    }
                }
            }
        }

        public void Validate(OperationDescription operationDescription) { }

        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            var logger = _loggerFactory.CreateLogger(dispatchOperation.Parent.ChannelDispatcher.Host.Description.ServiceType.FullName);
            dispatchOperation.Invoker = new ExecutionLoggingOperationInvoker(logger, dispatchOperation.Invoker, dispatchOperation);
        }

        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation) { }

        public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters) { }
    }

}