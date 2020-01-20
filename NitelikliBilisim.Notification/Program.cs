using System;
using NitelikliBilisim.Notification.Services;

namespace NitelikliBilisim.Notification
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var emailconsumer = new EmailConsumer();

            emailconsumer.MainAsync().Wait();
            Console.ReadKey();
        }
    }
}
