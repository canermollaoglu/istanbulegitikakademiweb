using System.Threading.Tasks;
using NitelikliBilisim.Core.ComplexTypes;
using NitelikliBilisim.Core.Enums;

namespace NitelikliBilisim.Core.Services.Abstracts
{
    public interface IMessageService
    {
        MessageStates MessageState { get; }
        Task SendAsync(string messageBody);
    }
}