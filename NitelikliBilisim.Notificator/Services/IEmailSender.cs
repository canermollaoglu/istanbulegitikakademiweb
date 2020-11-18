using NitelikliBilisim.Core.ComplexTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Notificator.Services
{
    public interface IEmailSender
    {
         Task SendAsync(EmailMessage message);

    }
}
