using System.IO;
using NitelikliBilisim.Core.Services.Abstracts;

namespace NitelikliBilisim.Core.ComplexTypes
{
    public class AttachModel : IAttachModel
    {
        public MemoryStream Attachment { get; set; }
        public string Name { get; set; }
    }
}