using NitelikliBilisim.Core.Messages;
using NitelikliBilisim.Core.RabbitMq;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.MessageHandlers
{
    public interface IEventHandler<in TEvent, in TPk> where TEvent : IEvent where TPk : struct
    {
        Task HandleAsync(TEvent _event, ICorrelationContext<TPk> context);
    }
}
