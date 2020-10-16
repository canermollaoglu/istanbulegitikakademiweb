using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using NitelikliBilisim.Core.ComplexTypes;
using System;
using Elasticsearch.Net;

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
            settings.BasicAuthentication("elastic", "LJrAagiPVWpNSxpjSXMdpURA");
            //settings.ApiKeyAuthentication("nitelikliapp", "SThXbU1YVUJTTkJsZlRzUG5XVHQ6Zm9JMlYzLVVUNXk4NFhQVmp2MkpHQQ==");
            
            var client = new ElasticClient(settings);

            var resp = client.Ping().DebugInformation;

            services.AddSingleton<IElasticClient>(client);
        }
    }
}
