using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;
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
            var defaultIndex = configuration["elasticsearch:index"];

            var settings = new ConnectionSettings(new Uri(url))
                 .DefaultIndex(defaultIndex)
                 .DefaultMappingFor<TransactionLog>(m => m
                    .IndexName("ut_log")
                    .IdProperty(p => p.Id)
                );

            var client = new ElasticClient(settings);

            services.AddSingleton<IElasticClient>(client);
        }
    }
}
