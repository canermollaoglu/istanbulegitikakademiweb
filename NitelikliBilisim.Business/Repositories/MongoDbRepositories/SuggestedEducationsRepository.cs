using MongoDB.Driver;
using NitelikliBilisim.Core.MongoOptions.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Business.Repositories.MongoDbRepositories
{

    public class SuggestedEducationsRepository : BaseMongoRepository<MSuggestedEducation>
    {
        private readonly IMongoCollection<MSuggestedEducation> mongoCollection;
        public SuggestedEducationsRepository(string mongoDBConnectionString, string dbName, string collectionName) : base(mongoDBConnectionString, dbName, collectionName)
        {
            var client = new MongoClient(mongoDBConnectionString);
            var database = client.GetDatabase(dbName);
            mongoCollection = database.GetCollection<MSuggestedEducation>(collectionName);
        }


        public virtual MSuggestedEducation GetByUserId(string id)
        {
            return mongoCollection.Find<MSuggestedEducation>(m => m.UserId == id).FirstOrDefault();
        }


    }
}
