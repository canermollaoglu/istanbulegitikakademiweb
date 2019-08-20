using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using NitelikliBilisim.Core.ComplexTypes;
using NitelikliBilisim.Core.Services.Abstracts;

namespace NitelikliBilisim.Core.Services
{
    public class EmailService : IMessageService
    {
        public MessageState MessageState { get; set; }
        public string[] Cc { get; set; }
        public string[] Bcc { get; set; }
        public string FilePath { get; set; }
        public List<AttachModel> AttachList { get; set; } = new List<AttachModel>();
        public string SenderMail { get; set; }
        public string Password { get; set; }
        public string Smtp { get; set; }
        public int SmtpPort { get; set; }
        public EmailService()
        {
            this.SenderMail = "site@wissenakademie.com";
            this.Password = "*+Wissen2018*+";
            this.Smtp = "smtp.office365.com";
            this.SmtpPort = 587;
        }
        public EmailService(string senderMail, string password, string smtp, int smtpPort)
        {
            this.SenderMail = senderMail;
            this.Password = password;
            this.Smtp = smtp;
            this.SmtpPort = smtpPort;
        }
        public void Send(Message message, params string[] contacts)
        {
            this.SendAsync(message,contacts).Wait();
        }

        public async Task SendAsync(Message message, params string[] contacts)
        {
            try
            {
                var mail = new MailMessage { From = new MailAddress(this.SenderMail) };
                if (!string.IsNullOrEmpty(FilePath))
                {
                    mail.Attachments.Add(new Attachment(FilePath));
                }
                if (this.AttachList.Any())
                {
                    foreach (var attachModel in this.AttachList)
                    {
                        var a = new Attachment(contentStream: attachModel.Attachment, name: attachModel.Name);
                        mail.Attachments.Add(a);
                    }
                }
                foreach (var c in contacts)
                {
                    mail.To.Add(c);
                }

                if (Cc != null && Cc.Length > 0)
                {
                    foreach (var cc in Cc)
                    {
                        mail.CC.Add(new MailAddress(cc));
                    }
                }
                if (Bcc != null && Bcc.Length > 0)
                {
                    foreach (var bcc in Bcc)
                    {
                        mail.Bcc.Add(new MailAddress(bcc));
                    }
                }
                mail.Subject = message.Subject;
                mail.Body = message.Body;

                mail.IsBodyHtml = true;
                mail.BodyEncoding = Encoding.UTF8;
                var smptClient = new SmtpClient(this.Smtp, this.SmtpPort)
                {
                    Credentials = new NetworkCredential(this.SenderMail, this.Password),
                    EnableSsl = true
                };
                await smptClient.SendMailAsync(mail);
                MessageState = MessageState.Delivered;
            }
            catch 
            {
                MessageState = MessageState.NotDelivered;
                throw;
            }
        }
    }
}