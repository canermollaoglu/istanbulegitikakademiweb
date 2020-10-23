using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.Core.ComplexTypes;
using System;

namespace NitelikliBilisim.App.Extensions
{
    public static class ElasticsearchExtensions
    {
        public static void AddElasticsearch(
        this IServiceCollection services, IConfiguration configuration)
        {
            var url = configuration["elasticsearch:url"];
            var defaultIndex = configuration["elasticsearch:userlogindex"];
            var userName = configuration["elasticsearch:username"];
            var password = configuration["elasticsearch:password"];


            var settings = new ConnectionSettings(new Uri(url))
                 .DefaultIndex(defaultIndex)
                 .DefaultMappingFor<TransactionLog>(m => m
                    .IndexName("ut_log")
                    .IdProperty(p => p.Id)
                )
                 .DefaultMappingFor<ExceptionInfo>(m => m
                 .IndexName("exception_log")
                 .IdProperty(p => p.Id));
            settings.BasicAuthentication(userName, password);

            var client = new ElasticClient(settings);
            //var resp = client.Ping().DebugInformation;
            services.AddSingleton<IElasticClient>(client);
        }
    }
}
