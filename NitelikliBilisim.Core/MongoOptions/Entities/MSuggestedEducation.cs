using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
