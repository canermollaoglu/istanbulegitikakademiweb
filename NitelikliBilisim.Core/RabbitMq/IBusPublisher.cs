using NitelikliBilisim.Core.Messages;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.RabbitMq
{
    public interface IBusPublisher<in TPk>
    {
        Task SendAsync<TCommand>(TCommand command, ICorrelationContext<TPk> context)
            where TCommand : ICommand;

        Task PublishAsync<TEvent>(TEvent _event, ICorrelationContext<TPk> context)
            where TEvent : IEvent;
    }
}
