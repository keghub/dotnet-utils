using System;
using System.Threading.Tasks;
using Amazon.SimpleNotificationService;
using Newtonsoft.Json;
using Nybus.Bridge;

namespace EMG.Utilities
{
    public class SnsNybusBridge : INybusBridge
    {
        private readonly NybusBridgeOptions _options;
        private readonly IAmazonSimpleNotificationService _sns;

        public SnsNybusBridge(IAmazonSimpleNotificationService sns, NybusBridgeOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _sns = sns ?? throw new ArgumentNullException(nameof(sns));
        }

        public Task InvokeCommand<TCommand>(TCommand command, Guid correlationId) where TCommand : class, ICommand
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            return SendMessage(command, correlationId);
        }

        public Task RaiseEvent<TEvent>(TEvent @event, Guid correlationId) where TEvent : class, IEvent
        {
            if (@event == null)
            {
                throw new ArgumentNullException(nameof(@event));
            }

            return SendMessage(@event, correlationId);
        }

        private async Task SendMessage<T>(T item, Guid correlationId) where T : class
        {
            if (_options.TopicArn == null)
            {
                throw new ArgumentNullException(nameof(NybusBridgeOptions.TopicArn));
            }

            var message = Message.Create(item, correlationId);

            var body = JsonConvert.SerializeObject(message, _options.SerializerSettings);

            await _sns.PublishAsync(_options.TopicArn, body).ConfigureAwait(false);
        }
    }

    public class NybusBridgeOptions
    {
        public string TopicArn { get; set; }

        public JsonSerializerSettings SerializerSettings { get; } = new JsonSerializerSettings();
    }
}