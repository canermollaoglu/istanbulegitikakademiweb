using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace NitelikliBilisim.Notificator.Services
{
    public interface ISubscriber
    {
        Task MainAsync();
        void RegisterOnMessageHandlerAndReceiveMessages();
        Task ProcessMessagesAsync(Message message, CancellationToken token);
        Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs);
    }
}
