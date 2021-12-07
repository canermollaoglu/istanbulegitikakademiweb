using MongoDB.Bson;

namespace NitelikliBilisim.Core.MongoOptions
{
    public abstract class MongoBaseModel
    {
        public ObjectId Id { get; set; }
    }
}
