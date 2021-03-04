using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.MongoOptions.Entities
{
    public class CampaignLog :MongoBaseModel
    {
        [BsonElement("RefererUrl")]
        public string RefererUrl { get; set; }
        [BsonElement("CampaignName")]
        public string CampaignName { get; set; }
        [BsonElement("IpAddress")]
        public string IpAddress { get; set; }
    }
}
