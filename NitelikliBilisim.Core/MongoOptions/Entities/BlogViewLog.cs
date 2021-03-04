using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.MongoOptions.Entities
{
    public class BlogViewLog :MongoBaseModel
    {
        [BsonElement("IpAddress")]
        public string IpAddress { get; set; }
        [BsonElement("UserId")]
        public string UserId { get; set; }
        [BsonElement("SessionId")]
        public string SessionId { get; set; }
        [BsonElement("CatSeoUrl")]
        public string CatSeoUrl { get; set; }
        [BsonElement("SeoUrl")]
        public string SeoUrl { get; set; }
        [BsonElement("CreatedDate")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
