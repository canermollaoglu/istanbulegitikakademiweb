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
        const string ServiceBusConnectionString = "Endpoint=sb://niteliklibus.servicebus.windows.net/;SharedAccessKeyName=gonder;SharedAccessKey=/RWJTMDvhg80ttMer7ULlJFOjunH5GeWnqHwMaGYXKo=";
        const string QueueName = "email_notify";
        private IQueueClient _queueClient;

        public async Task SendAsync(string messageBody)
        {
            try
            {
                _queueClient = new QueueClient(ServiceBusConnectionString, QueueName);
                var message = new Message(Encoding.UTF8.GetBytes(messageBody))
                {
                    SessionId = Guid.NewGuid().ToString(),
                    MessageId = Guid.NewGuid().ToString()
                };
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