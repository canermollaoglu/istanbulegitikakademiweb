using System.Threading.Tasks;
using NitelikliBilisim.Core.Messages;
using NitelikliBilisim.Core.RabbitMq;

namespace NitelikliBilisim.Core.MessageHandlers
{
    public interface ICommandHandler<in TCommand, in TPk> where TCommand : ICommand where TPk : struct
    {
        Task HandleAsync(TCommand command, ICorrelationContext<TPk> context);
    }
}
