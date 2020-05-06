using System;
using System.Threading.Tasks;
using Nybus.Bridge;

namespace EMG.Utilities
{
    public static class NybusBridgeExtensions
    {
        public static Task InvokeCommand<TCommand>(this INybusBridge bridge, TCommand command) where TCommand : class, ICommand
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            var correlationId = Guid.NewGuid();

            return bridge.InvokeCommand(command, correlationId);
        }

        public static Task RaiseEvent<TEvent>(this INybusBridge bridge, TEvent @event) where TEvent : class, IEvent
        {
            if (@event == null) throw new ArgumentNullException(nameof(@event));

            var correlationId = Guid.NewGuid();

            return bridge.RaiseEvent(@event, correlationId);
        }
    }
}