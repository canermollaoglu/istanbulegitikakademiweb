using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NitelikliBilisim.App.Models
{
    public class ResponseModel
    {
        public bool isSuccess { get; set; }
        public object data { get; set; }
        public IEnumerable<string> errors { get; set; }
    }
}
