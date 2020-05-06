using System;
using System.Collections.Generic;
using System.ServiceModel.Dispatcher;
using System.Text;
using Microsoft.Extensions.Logging;

namespace EMG.Utilities.ServiceModel.Logging
{
    public class ExecutionLoggingOperationInvoker : IOperationInvoker
    {
        private readonly ILogger _logger;
        private readonly IOperationInvoker _invoker;
        private readonly DispatchOperation _operation;

        public ExecutionLoggingOperationInvoker(ILogger logger, IOperationInvoker invoker, DispatchOperation operation)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _invoker = invoker ?? throw new ArgumentNullException(nameof(invoker));
            _operation = operation ?? throw new ArgumentNullException(nameof(operation));
        }

        public object[] AllocateInputs() => _invoker.AllocateInputs();

        public object Invoke(object instance, object[] inputs, out object[] outputs)
        {
            LogInvoke(inputs);

            object result;

            try
            {
                result = _invoker.Invoke(instance, inputs, out outputs);
                LogComplete();
            }
            catch (Exception ex)
            {
                LogError(inputs, ex);
                outputs = Array.Empty<object>();
                return null;
            }

            return result;
        }

        public IAsyncResult InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state)
        {
            LogInvoke(inputs);
            return _invoker.InvokeBegin(instance, inputs, callback, state);
        }

        public object InvokeEnd(object instance, out object[] outputs, IAsyncResult result)
        {
            LogComplete();
            return _invoker.InvokeEnd(instance, out outputs, result);
        }

        public bool IsSynchronous => _invoker.IsSynchronous;

        private void LogInvoke(object[] inputs)
        {
            var (format, parameters) = FormatInputs(inputs);
            _logger.LogTrace($"Invoked {format}", parameters);
        }

        private void LogError(object[] inputs, Exception exception)
        {
            var (format, parameters) = FormatInputs(inputs);
            _logger.LogError(exception, $"An error occurred while processing {format}", parameters);
        }

        private void LogComplete()
        {
            _logger.LogTrace("Completed {OPERATION}", _operation.Name);
        }

        private (string format, object[] parameters) FormatInputs(object[] inputs)
        {
            var builder = new StringBuilder();
            builder.Append("{OPERATION}(");

            var items = new List<object> { _operation.Name };

            var parameters = new List<string>();
            for (var i = 0; i < inputs.Length; i++)
            {
                parameters.Add($"\"{{P{i}}}\" [{{T{i}}}]");
                items.Add(inputs[i]);
                items.Add(inputs[i].GetType());
            }

            builder.Append(string.Join(",", parameters));
            builder.Append(")");

            return (builder.ToString(), items.ToArray());
        }
    }

}