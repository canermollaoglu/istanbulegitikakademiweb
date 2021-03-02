using NitelikliBilisim.Core.MongoOptions.Entities;

namespace NitelikliBilisim.Business.Repositories.MongoDbRepositories
{
    public class TransactionLogRepository : BaseMongoRepository<TransactionLog>
    {
        public TransactionLogRepository(string mongoDBConnectionString, string dbName, string collectionName) : base(mongoDBConnectionString, dbName, collectionName)
        {

        }
    }
}
