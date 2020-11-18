using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NitelikliBilisim.Notificator.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NitelikliBilisim.Notificator
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ILogger<EmailConsumer> _emailLogger;
        private Task _executingTask;
        private CancellationTokenSource _cts;

        public Worker(ILogger<Worker> logger, ILogger<EmailConsumer> emailLogger)
        {
            _logger = logger;
            _emailLogger = emailLogger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogWarning("Worker service started.");

            //var emailConsumer = new EmailConsumer(_emailLogger);
           // emailConsumer.MainAsync().Wait(cancellationToken);

            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _executingTask = ExecuteAsync(_cts.Token);

            return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000000, stoppingToken);

            }
        }
    }
}
