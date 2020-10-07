using System.Collections.Generic;

namespace NitelikliBilisim.App.Models
{
    public class ResponseModel
    {
        public bool isSuccess { get; set; }
        public string message { get; set; }
        public object data { get; set; }
        public IEnumerable<string> errors { get; set; }
    }
}
