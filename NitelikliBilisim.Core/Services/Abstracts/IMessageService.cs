using NitelikliBilisim.Core.Enums;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.Services.Abstracts
{
    public interface IMessageService
    {
        MessageStates MessageState { get; }
        Task SendAsync(string messageBody);
    }
}