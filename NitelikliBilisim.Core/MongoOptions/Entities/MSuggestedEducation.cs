using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace NitelikliBilisim.Core.MongoOptions.Entities
{
    public class MSuggestedEducation : MongoBaseModel
    {
        [BsonElement("UserId")]
        public string UserId { get; set; }
        [BsonElement("Educations")]
        public Dictionary<string,double> Educations { get; set; }
    }
}
