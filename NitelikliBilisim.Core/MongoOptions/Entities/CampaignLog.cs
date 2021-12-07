using MongoDB.Bson.Serialization.Attributes;

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
