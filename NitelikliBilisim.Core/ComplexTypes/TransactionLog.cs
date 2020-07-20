using System;
using System.Collections.Generic;

namespace NitelikliBilisim.Core.ComplexTypes
{
    public class TransactionLog
    {
        public TransactionLog()
        {
            Id = Guid.NewGuid();
        }
        public  Guid Id { get; set; }
        public string SessionId { get; set; }
        public string UserId { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string IpAddress { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public IDictionary<string, object> ActionArguments { get; set; }
    }
}
