using System.Collections.Generic;

namespace NitelikliBilisim.Core.ComplexTypes
{
    public class EmailMessage
    {
        public string[] Contacts { get; set; }
        public string[] Cc { get; set; }
        public string[] Bcc { get; set; }
        public List<AttachModel> AttachList { get; set; } = new List<AttachModel>();
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}