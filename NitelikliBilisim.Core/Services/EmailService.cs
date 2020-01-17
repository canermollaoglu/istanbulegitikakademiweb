using System;
using System.Text;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.Services.Abstracts;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace NitelikliBilisim.Core.Services
{
    public class EmailService : IMessageService
    {
        public MessageStates MessageState { get; private set; }
        const string ServiceBusConnectionString = "Endpoint=sb://niteliklibus.servicebus.windows.net/;SharedAccessKeyName=emailgonder;SharedAccessKey=ds+L/BgTpX1ookhAjdaWHoMaXfUZTU6Ox0FuEZbxLf8=;EntityPath=email_notify";
        const string QueueName = "email_notify";
        private IQueueClient _queueClient;

        public async Task SendAsync(string messageBody)
        {
            try
            {
                _queueClient = new QueueClient(ServiceBusConnectionString, QueueName);
                var message = new Message(Encoding.UTF8.GetBytes(messageBody));
                await _queueClient.SendAsync(message);
                MessageState = MessageStates.Delivered;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                MessageState = MessageStates.NotDelivered;
                throw;
            }
        }
    }
}