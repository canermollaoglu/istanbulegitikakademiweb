using MongoDB.Bson;
using NitelikliBilisim.Core.MongoOptions.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.student
{
    public class TransactionLogListViewModel
    {
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public DateTime CreatedDate { get; set; }
        public ObjectId Id { get; set; }
        public string IpAddress { get; set; }
        public string SessionId { get; set; }
        public string UserId { get; set; }
        public List<LogParameter> Parameters { get; set; }
    }
}
