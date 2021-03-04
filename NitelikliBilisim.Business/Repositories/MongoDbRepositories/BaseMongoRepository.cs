using MongoDB.Bson;
using MongoDB.Driver;
using NitelikliBilisim.Core.MongoOptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NitelikliBilisim.Business.Repositories.MongoDbRepositories
{
    public class BaseMongoRepository<TModel> where TModel:MongoBaseModel
    {
        private readonly IMongoCollection<TModel> mongoCollection;
        public BaseMongoRepository(string mongoDBConnectionString, string dbName, string collectionName)
        {
            var client = new MongoClient(mongoDBConnectionString);
            var database = client.GetDatabase(dbName);
            mongoCollection = database.GetCollection<TModel>(collectionName);
        }

        public virtual IQueryable<TModel> GetListQueryable()
        {
            return mongoCollection.AsQueryable();
        }

        public virtual List<TModel> GetList(Expression<Func<TModel, bool>> predicate = null)
        {
            return mongoCollection.Find(predicate).ToList();
        }

        public virtual TModel GetById(string id)
        {
            var docId = new ObjectId(id);
            return mongoCollection.Find<TModel>(m => m.Id == docId).FirstOrDefault();
        }

        public virtual TModel Create(TModel model)
        {
            mongoCollection.InsertOne(model);
            return model;
        }

        public virtual void Update(string id, TModel model)
        {
            var docId = new ObjectId(id);
            mongoCollection.ReplaceOne(m => m.Id == docId, model);
        }

        public virtual void Delete(TModel model)
        {
            mongoCollection.DeleteOne(m => m.Id == model.Id);
        }

        public virtual void Delete(string id)
        {
            var docId = new ObjectId(id);
            mongoCollection.DeleteOne(m => m.Id == docId);
        }

        public virtual bool Any(Expression<Func<TModel, bool>> predicate = null)
        {
            return mongoCollection.AsQueryable<TModel>().Any(predicate);
        }
        public virtual int Count(Expression<Func<TModel, bool>> predicate = null)
        {
            return mongoCollection.AsQueryable<TModel>().Count(predicate);
        }
    }
}
