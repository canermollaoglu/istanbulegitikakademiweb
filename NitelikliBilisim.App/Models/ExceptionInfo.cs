using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NitelikliBilisim.App.Models
{
    public class ExceptionInfo
    {
        public ExceptionInfo()
        {
            Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }
        public string ControllerName { get; internal set; }
        public string ActionName { get; internal set; }
        public string UserId { get; internal set; }
        public string Message { get; internal set; }
        public string StackTrace { get; internal set; }
        public string InnerException { get; internal set; }
    }
}
