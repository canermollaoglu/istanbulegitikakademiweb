using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NitelikliBilisim.Notificator.Services;

namespace NitelikliBilisim.Notificator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.AddSingleton<IEmailSender, EmailSender>();
                });
    }
}
