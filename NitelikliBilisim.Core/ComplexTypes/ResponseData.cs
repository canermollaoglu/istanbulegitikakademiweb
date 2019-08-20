using System;

namespace NitelikliBilisim.Core.ComplexTypes
{
    public class ResponseData
    {
        public string Message { get; set; }
        public bool Success { get; set; }
        public object Data { get; set; }
        public DateTime ResponseTime { get; set; } = DateTime.Now;
        public string ResponseTimeU { get; set; } = $"{DateTime.Now:O}";
    }
}