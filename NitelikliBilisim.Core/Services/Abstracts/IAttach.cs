using System.IO;

namespace NitelikliBilisim.Core.Services.Abstracts
{
    public interface IAttachModel
    {
        MemoryStream Attachment { get; set; }
        string Name { get; set; }
    }
}