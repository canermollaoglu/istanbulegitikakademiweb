using NitelikliBilisim.Core.Messages;

namespace NitelikliBilisim.Core.RabbitMq
{
    public interface IBusSubscriber
    {
        IBusSubscriber SubscribeCommand<TCommand>(string _namespace = null)
            where TCommand : ICommand;

        IBusSubscriber SubscribeEvent<TEvent>(string _namespace = null)
            where TEvent : IEvent;
    }
}
