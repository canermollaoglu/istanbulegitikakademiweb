using NitelikliBilisim.Core.MongoOptions.Entities;

namespace NitelikliBilisim.Business.Repositories.MongoDbRepositories
{
    public class BlogViewLogRepository:BaseMongoRepository<BlogViewLog>
    {
        public BlogViewLogRepository(string mongoDBConnectionString, string dbName, string collectionName) : base(mongoDBConnectionString, dbName, collectionName)
        {

        }
    }
}
