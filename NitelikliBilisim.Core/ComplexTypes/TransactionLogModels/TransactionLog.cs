using Nest;
using NitelikliBilisim.Core.ComplexTypes.TransactionLogModels;
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
        [Keyword]
        public  Guid Id { get; set; }
        [Keyword]
        public string SessionId { get; set; }
        [Keyword]
        public string UserId { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string IpAddress { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public List<LogParameter> Parameters { get; set; }
    }

    public class TransactionLogListViewModel
    {
        public Guid Id { get; set; }
        public string SessionId { get; set; }
        public string UserId { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string IpAddress { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Parameters { get; set; }
    }
}
