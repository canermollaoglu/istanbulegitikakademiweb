using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NitelikliBilisim.Core.ComplexTypes;

namespace NitelikliBilisim.Notificator.Services
{
    public class EmailSender :IEmailSender
    {
        private readonly IConfiguration _configuration;
        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string SenderMail => _configuration.GetSection("EmailOptions:SenderMail").Value;
        public string Password => _configuration.GetSection("EmailOptions:Password").Value;
        public string Smtp => _configuration.GetSection("EmailOptions:Smtp").Value;
        public int SmtpPort => Convert.ToInt32(_configuration.GetSection("EmailOptions:SmtpPort").Value);
        public EmailSender()
        {
            //this.SenderMail = "noreply@niteliklibilisim.com.tr";
            //this.Password = "zexgtuuxppulprjc";
            //this.Smtp = "smtp.yandex.com";
            //this.SmtpPort = 587;
        }
        public async Task SendAsync(EmailMessage message)
        {
            var mail = new MailMessage { From = new MailAddress(this.SenderMail) };

            if (message.AttachList.Any())
            {
                foreach (var attachModel in message.AttachList)
                {
                    var a = new Attachment(contentStream: attachModel.Attachment, name: attachModel.Name);
                    mail.Attachments.Add(a);
                }
            }
            foreach (var c in message.Contacts)
            {
                mail.To.Add(c);
            }

            if (message.Cc != null && message.Cc.Length > 0)
            {
                foreach (var cc in message.Cc)
                {
                    mail.CC.Add(new MailAddress(cc));
                }
            }
            if (message.Bcc != null && message.Bcc.Length > 0)
            {
                foreach (var bcc in message.Bcc)
                {
                    mail.Bcc.Add(new MailAddress(bcc));
                }
            }
            mail.Subject = message.Subject;
            mail.Body = message.Body;

            mail.IsBodyHtml = true;
            mail.BodyEncoding = Encoding.UTF8;
            mail.SubjectEncoding = Encoding.UTF8;
            mail.HeadersEncoding = Encoding.UTF8;

            var smptClient = new SmtpClient(this.Smtp, this.SmtpPort)
            {
                Credentials = new NetworkCredential(this.SenderMail, this.Password),
                EnableSsl = true
            };
            await smptClient.SendMailAsync(mail);

        }
    }
}
