using System;
using System.Threading.Tasks;
using Nybus.Bridge;

namespace EMG.Utilities
{
    public interface INybusBridge
    {
        Task InvokeCommand<TCommand>(TCommand command, Guid correlationId) where TCommand : class, ICommand;

        Task RaiseEvent<TEvent>(TEvent @event, Guid correlationId) where TEvent : class, IEvent;
    }
}
