using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using NitelikliBilisim.Core.ComplexTypes;

namespace NitelikliBilisim.Notificator.Services
{
    public class EmailSender
    {
        public string SenderMail { get; set; }
        public string Password { get; set; }
        public string Smtp { get; set; }
        public int SmtpPort { get; set; }
        public EmailSender()
        {
            this.SenderMail = "site@wissenakademie.com";
            this.Password = "*+Wissen2018*+";
            this.Smtp = "smtp.office365.com";
            this.SmtpPort = 587;
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
