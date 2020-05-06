using System;

namespace EMG.Utilities
{
    public class Message
    {
        public static Message<T> Create<T>(T message, Guid correlationId) where T : class
        {
            var messageType = message.GetType();

            return new Message<T>
            {
                Type = $"{messageType.Namespace}:{messageType.Name}",
                Body = message,
                CorrelationId = correlationId
            };
        }
    }

    public class Message<T> where T : class
    {
        public string Type { get; set; }

        public T Body { get; set; }

        public Guid CorrelationId { get; set; }

    }
}