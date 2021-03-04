using NitelikliBilisim.Core.MongoOptions.Entities;

namespace NitelikliBilisim.Business.Repositories.MongoDbRepositories
{
    public class ExceptionInfoRepository:BaseMongoRepository<ExceptionInfo>
    {
        public ExceptionInfoRepository(string mongoDBConnectionString, string dbName, string collectionName) : base(mongoDBConnectionString, dbName, collectionName)
        {

        }
    }
}
