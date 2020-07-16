using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ComplexTypes
{
    public class TransactionLog
    {
        public string SessionId { get; set; }
        public string UserId { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string IpAddress { get; set; }
    }
}
