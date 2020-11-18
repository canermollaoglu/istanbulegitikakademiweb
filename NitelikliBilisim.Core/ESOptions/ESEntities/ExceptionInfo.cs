using System;

namespace NitelikliBilisim.Core.ESOptions.ESEntities
{
    public class ExceptionInfo
    {
        public ExceptionInfo()
        {
            Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }
        public string ControllerName { get;  set; }
        public string ActionName { get;  set; }
        public string UserId { get;  set; }
        public string Message { get;  set; }
        public string StackTrace { get;  set; }
        public string InnerException { get;  set; }
    }
}
