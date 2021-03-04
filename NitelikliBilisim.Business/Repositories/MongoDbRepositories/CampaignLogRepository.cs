using NitelikliBilisim.Core.MongoOptions.Entities;

namespace NitelikliBilisim.Business.Repositories.MongoDbRepositories
{
    public class CampaignLogRepository :BaseMongoRepository<CampaignLog>
    {
        public CampaignLogRepository(string mongoDBConnectionString, string dbName, string collectionName) : base(mongoDBConnectionString, dbName, collectionName)
        {

        }
    }
}
