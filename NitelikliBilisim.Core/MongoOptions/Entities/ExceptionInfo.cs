using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
