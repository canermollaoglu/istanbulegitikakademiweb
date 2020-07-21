using Elasticsearch.Net;
using Nest;
using NitelikliBilisim.Core.ComplexTypes;
using System;

namespace NitelikliBilisim.App.ELK
{
    public static class ElasticSearchClientHelper
    {

        public static ElasticClient CreateElasticClient()
        {
            var node = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));
            var settings = new ConnectionSettings(node)
                 .DefaultMappingFor<TransactionLog>(m => m
                    .IndexName("usertransactionlog")
                    .IdProperty(p=>p.Id)
                )
                .PrettyJson(true)
                .RequestTimeout(TimeSpan.FromSeconds(10));
            return new ElasticClient(settings);
        }


        public static void CheckIndex<T>(ElasticClient client, string indexName) where T : class
        {
            var response = client.Indices.Exists(indexName);
            if (!response.Exists)
            {
                client.Indices.Create(indexName, index =>
                   index.Map<T>(x => x.AutoMap()));
            }
        }
    }
}
