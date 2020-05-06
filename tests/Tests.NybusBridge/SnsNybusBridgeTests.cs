using System;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SimpleNotificationService;
using AutoFixture.Idioms;
using AutoFixture.NUnit3;
using EMG.Utilities;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Nybus.Bridge;
using Shouldly;

namespace Tests
{
    [TestFixture]
    public class SnsNybusBridgeTests
    {
        [Test, CustomAutoData]
        public void Constructor_is_guarded(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(SnsNybusBridge).GetConstructors());
        }

        [Test, CustomAutoData]
        public async Task InvokeCommand_should_publish_correct_message([Frozen] IAmazonSimpleNotificationService sns, [Frozen] NybusBridgeOptions options, SnsNybusBridge sut, TestCommand command, Guid correlationId)
        {
            await sut.InvokeCommand(command, correlationId);

            Mock.Get(sns).Verify(p => p.PublishAsync(options.TopicArn, It.Is<string>(message => ValidateCommand(message, command)), It.IsAny<CancellationToken>()));
        }

        [Test, CustomAutoData]
        public void InvokeCommand_throws_if_command_is_null(SnsNybusBridge sut, Guid correlationId)
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => sut.InvokeCommand<TestCommand>(null, correlationId));
        }

        [Test, CustomAutoData]
        public void InvokeCommand_should_throw_if_TopicArn_is_null([Frozen] NybusBridgeOptions options, SnsNybusBridge sut, TestCommand command, Guid correlationId)
        {
            options.TopicArn = null;

            Assert.ThrowsAsync<ArgumentNullException>(() => sut.InvokeCommand(command, correlationId));
        }

        [Test, CustomAutoData]
        public async Task RaiseEvent_should_publish_correct_message([Frozen] IAmazonSimpleNotificationService sns, [Frozen] NybusBridgeOptions options, SnsNybusBridge sut, TestEvent @event, Guid correlationId)
        {
            await sut.RaiseEvent(@event, correlationId);

            Mock.Get(sns).Verify(p => p.PublishAsync(options.TopicArn, It.Is<string>(message => ValidateEvent(message, @event)), It.IsAny<CancellationToken>()));
        }

        [Test, CustomAutoData]
        public void RaiseEvent_throws_if_event_is_null(SnsNybusBridge sut, Guid correlationId)
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => sut.RaiseEvent<TestEvent>(null, correlationId));
        }

        [Test, CustomAutoData]
        public void RaiseEvent_should_throw_if_TopicArn_is_null([Frozen] NybusBridgeOptions options, SnsNybusBridge sut, TestEvent @event, Guid correlationId)
        {
            options.TopicArn = null;

            Assert.ThrowsAsync<ArgumentNullException>(() => sut.RaiseEvent(@event, correlationId));
        }

        private bool ValidateCommand<TCommand>(string notification, TCommand command) where TCommand : class, ICommand
        {
            var commandType = typeof(TCommand);

            var message = JsonConvert.DeserializeObject<Message<TCommand>>(notification);

            message.Type.ShouldStartWith(commandType.Namespace);
            message.Type.ShouldEndWith(commandType.Name);

            message.Body.ShouldNotBeNull();

            return true;
        }

        private static bool ValidateEvent<TEvent>(string notification, TEvent @event) where TEvent : class, IEvent
        {
            var eventType = typeof(TEvent);

            var message = JsonConvert.DeserializeObject<Message<TEvent>>(notification);

            message.Type.ShouldStartWith(eventType.Namespace);
            message.Type.ShouldEndWith(eventType.Name);

            message.Body.ShouldNotBeNull();

            return true;
        }
    }
}