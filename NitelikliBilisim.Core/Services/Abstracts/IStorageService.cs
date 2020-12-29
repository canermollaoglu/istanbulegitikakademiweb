using System.IO;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.Services.Abstracts
{
    public interface IStorageService
    {
        Task<string> UploadFile(Stream fileStream, string fileName, string folderName);
        Task<Stream> DownloadFile(string fileName, string folderName);
        public string BlobUrl { get; }
        //Task<MemoryStream> DownloadFileStream(string fileName, string folderName);
        Task<bool> DeleteFile(string fileName, string folderName);
    }
}