using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NitelikliBilisim.Core.Messages;
using RawRabbit;
using RawRabbit.Enrichers.MessageContext;

namespace NitelikliBilisim.Core.RabbitMq
{
    public class BusPublisher<TPk> : IBusPublisher<TPk>
    {
        private readonly IBusClient _busClient;
        private readonly string _defaultNamespace;

        public BusPublisher(IBusClient busClient, string defaultNamespace)
        {
            _busClient = busClient;
            _defaultNamespace = defaultNamespace;
        }

        public async Task SendAsync<TCommand>(TCommand command, ICorrelationContext<TPk> context)
            where TCommand : ICommand
        {
            var commandName = command.GetType().Name;

            await _busClient.PublishAsync(command, ctx => ctx.UseMessageContext(context)
                .UsePublishConfiguration(p => p.WithRoutingKey(GetRoutingKey(command))));
        }

        public Task PublishAsync<TEvent>(TEvent _event, ICorrelationContext<TPk> context) where TEvent : IEvent
        {
            throw new NotImplementedException();
        }

        private string GetRoutingKey<T>(T message)
        {
            var _namespace = message.GetType().GetCustomAttribute<MessageNamespaceAttribute>()?.Namespace ?? _defaultNamespace;
            _namespace = string.IsNullOrWhiteSpace(_namespace) ? string.Empty : $"{_namespace}.";

            return $"{_namespace}{typeof(T).Name.Underscore()}".ToLowerInvariant();
        }
    }
}
