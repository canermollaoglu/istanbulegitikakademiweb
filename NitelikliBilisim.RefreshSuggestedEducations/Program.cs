using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using NitelikliBilisim.Business.Repositories.MongoDbRepositories;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.MongoOptions;
using NitelikliBilisim.Data;
using NitelikliBilisim.Notificator.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NitelikliBilisim.RefreshSuggestedEducations
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
                    services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
                    services.Configure<AppSettings>(hostContext.Configuration.GetSection("Config"));
                    string mongoDbName = hostContext.Configuration["MongoDbSettings:DatabaseName"];
                    string mongoConnectionString = hostContext.Configuration["MongoDbSettings:ConnectionString"];
                    services.AddTransient(s => new BlogViewLogRepository(mongoConnectionString, mongoDbName, MongoCollectionNames.BlogViewLog));
                    services.AddTransient(s => new TransactionLogRepository(mongoConnectionString, mongoDbName, MongoCollectionNames.TransactionLog));
                    services.AddTransient(s => new CampaignLogRepository(mongoConnectionString, mongoDbName, MongoCollectionNames.CampaignLog));
                    services.AddTransient(s => new ExceptionInfoRepository(mongoConnectionString, mongoDbName, MongoCollectionNames.ExceptionLog));
                    services.AddTransient(s => new SuggestedEducationsRepository(mongoConnectionString, mongoDbName, MongoCollectionNames.SuggestedEducations));
                    services.AddDbContext<NbDataContext>(options =>
                options.UseSqlServer(hostContext.Configuration.GetConnectionString("SqlConnectionString")));
                    services.AddHostedService<Worker>();
                    services.AddSingleton<IEmailSender, EmailSender>();
                    services.AddScoped<IScopedProcessingService, ScopedProcessingService>();
                });
    }
}
