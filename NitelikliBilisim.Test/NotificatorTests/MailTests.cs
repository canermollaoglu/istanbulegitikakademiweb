using NitelikliBilisim.Core.ComplexTypes;
using NitelikliBilisim.Notificator.Services;
using Xunit;

namespace NitelikliBilisim.Test.NotificatorTests
{
    public class MailTests
    {
        private readonly EmailSender _emailSender;
        private static readonly string[] mailsToSend =  { "alper.dagli@wissenakademie.com", "eser.canik@wissenakademie.com" };

        public MailTests()
        {
            _emailSender = new EmailSender();
        }

        [Fact]
        public void Should_Send_Email()
        {
            var result = _emailSender.SendAsync(new EmailMessage()
            {
                Subject = "Test Mail | Nitelikli Bilişim",
                Body = "Test",
                Contacts = mailsToSend
            });

            Assert.True(result.IsCompletedSuccessfully);
        }
    }
}
