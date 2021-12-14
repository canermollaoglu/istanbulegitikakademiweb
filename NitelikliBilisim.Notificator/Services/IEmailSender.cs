using NitelikliBilisim.Core.ComplexTypes;
using System.Threading.Tasks;

namespace NitelikliBilisim.Notificator.Services
{
    public interface IEmailSender
    {
         Task SendAsync(EmailMessage message);

    }
}
