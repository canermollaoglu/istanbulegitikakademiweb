using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NitelikliBilisim.Core.ComplexTypes;

namespace NitelikliBilisim.Notificator.Services
{
    public class EmailConsumer : ISubscriber
    {
        const string ServiceBusConnectionString =
            "Endpoint=sb://niteliklibus.servicebus.windows.net/;SharedAccessKeyName=oku;SharedAccessKey=5q3yIUkHyMxWMBS00ZdbBdija+5JRLmDA1O8NbJFAJ8=";

        const string QueueName = "email_notify";
        private IQueueClient _queueClient;
        private IEmailSender _emailSender;
        private readonly ILogger<EmailConsumer> _emailLogger;

        public EmailConsumer(ILogger<EmailConsumer> emailLogger,IEmailSender emailSender)
        {
            _emailLogger = emailLogger;
            _emailSender = emailSender;
        }

        public Task MainAsync()
        {
            _queueClient = new QueueClient(ServiceBusConnectionString, QueueName);
            // Register QueueClient's MessageHandler and receive messages in a loop
            RegisterOnMessageHandlerAndReceiveMessages();
            //await _queueClient.CloseAsync();
            return Task.CompletedTask;
        }

        public void RegisterOnMessageHandlerAndReceiveMessages()
        {
            // Configure the MessageHandler Options in terms of exception handling, number of concurrent messages to deliver etc.
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                // Maximum number of Concurrent calls to the callback `ProcessMessagesAsync`, set to 1 for simplicity.
                // Set it according to how many messages the application wants to process in parallel.
                MaxConcurrentCalls = 5,

                // Indicates whether MessagePump should automatically complete the messages after returning from User Callback.
                // False below indicates the Complete will be handled by the User Callback as in `ProcessMessagesAsync` below.
                AutoComplete = false
            };

            // Register the function that will process messages
            _queueClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
        }

        public async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            // Process the message
            
            _emailLogger.LogInformation($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}");

            var emailMessage = JsonConvert.DeserializeObject<EmailMessage>(Encoding.UTF8.GetString(message.Body));

            if (emailMessage == null) 
                return;
            try
            {
                await _emailSender.SendAsync(emailMessage);
                // Complete the message so that it is not received again.
                // This can be done only if the queueClient is created in ReceiveMode.PeekLock mode (which is default).
                await _queueClient.CompleteAsync(message.SystemProperties.LockToken);
                _emailLogger.LogInformation("Email Sent: {time}", DateTimeOffset.Now);
            }
            catch (Exception ex)
            {
                _emailLogger.LogError($"Error: {ex.Message} {DateTimeOffset.Now}", DateTimeOffset.Now);
                //await _queueClient.ScheduleMessageAsync(message, DateTimeOffset.Now.AddMinutes(5));
            }

            // Note: Use the cancellationToken passed as necessary to determine if the queueClient has already been closed.
            // If queueClient has already been Closed, you may chose to not call CompleteAsync() or AbandonAsync() etc. calls 
            // to avoid unnecessary exceptions.
        }

        public Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }
    }
}