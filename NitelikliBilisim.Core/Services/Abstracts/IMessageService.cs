
using System.Threading.Tasks;
using NitelikliBilisim.Core.ComplexTypes;

namespace NitelikliBilisim.Core.Services.Abstracts
{
    public interface IMessageService
    {
        MessageState MessageState { get; }
        Task SendAsync(Message message, params string[] contacts);
        void Send(Message message, params string[] contacts);
    }
}