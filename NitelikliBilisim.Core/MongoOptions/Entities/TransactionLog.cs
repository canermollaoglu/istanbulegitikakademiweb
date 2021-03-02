using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.MongoOptions.Entities
{
    public class TransactionLog :MongoBaseModel
    {
        [BsonElement("SessionId")]
        public string SessionId { get; set; }
        [BsonElement("UserId")]
        public string UserId { get; set; }
        [BsonElement("ControllerName")]
        public string ControllerName { get; set; }
        [BsonElement("ActionName")]
        public string ActionName { get; set; }
        [BsonElement("IpAddress")]
        public string IpAddress { get; set; }
        [BsonElement("CreatedDate")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        [BsonElement("Parameters")]
        public List<LogParameter> Parameters { get; set; }
    }
    public class LogParameter
    {
        public string ParameterName { get; set; }
        public string ParameterType { get; set; }
        public string ParameterValue { get; set; }
    }
}
