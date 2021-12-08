using MongoDB.Bson.Serialization.Attributes;

namespace NitelikliBilisim.Core.MongoOptions.Entities
{
    public class ExceptionInfo : MongoBaseModel
    {
        [BsonElement("ControllerName")]
        public string ControllerName { get; set; }
        [BsonElement("ActionName")]
        public string ActionName { get; set; }
        [BsonElement("UserId")]
        public string UserId { get; set; }
        [BsonElement("Message")]
        public string Message { get; set; }
        [BsonElement("StackTrace")]
        public string StackTrace { get; set; }
        [BsonElement("InnerException")]
        public string InnerException { get; set; }
    }
}
