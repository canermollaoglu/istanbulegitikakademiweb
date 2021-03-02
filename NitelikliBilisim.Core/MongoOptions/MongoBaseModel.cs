using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.MongoOptions
{
    public abstract class MongoBaseModel
    {
        public ObjectId Id { get; set; }
    }
}
