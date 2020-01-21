using Microsoft.WindowsAzure.Storage;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.File;


namespace NitelikliBilisim.Core.Services
{
    public interface IStorageService
    {
        Task<string> UploadFile(Stream fileStream, string fileName, string folderName);
        Task<string> DownloadFile(string fileName, string folderName);
    }
    public class StorageService : IStorageService
    {
        private const string ReferanceName = "nbuploads";
        private const string AccessKey = "DefaultEndpointsProtocol=https;AccountName=niteliklidatastore;AccountKey=KxpcLymDmly4Gv0UG3LhUgr1olSbsSlfJ3cOy2jAPm2DZ94rTJ6GfXZFhiUGrX+FsFFeTr91jf1gcWIg/JbZ3g==;EndpointSuffix=core.windows.net";
        private readonly CloudStorageAccount _storageAccount;

        public StorageService()
        {
            _storageAccount = CloudStorageAccount.Parse(AccessKey);
        }

        public async Task<string> UploadFile(Stream fileStream, string fileName, string folderName)
        {
            //var ext = Path.GetExtension(fileName);
            fileName = StringHelper.GenerateUniqueCode() + StringHelper.UrlFormatConverter(fileName);
            fileStream.Position = 0;
            var fileClient = _storageAccount.CreateCloudFileClient();
            var share = fileClient.GetShareReference(ReferanceName);
            var rootDir = share.GetRootDirectoryReference();
            var fileDir = rootDir.GetDirectoryReference(folderName);

            if (fileDir.CreateIfNotExistsAsync().Result)
            {
                var cfile = fileDir.GetFileReference(fileName);
                await cfile.UploadFromStreamAsync(fileStream);
                return fileName;
            }

            return null;
        }

        public async Task<string> DownloadFile(string fileName, string folderName)
        {
            var fileClient = _storageAccount.CreateCloudFileClient();
            var share = fileClient.GetShareReference(ReferanceName);
            var rootDir = share.GetRootDirectoryReference();
            var fileDir = rootDir.GetDirectoryReference(folderName);
            var cfile = fileDir.GetFileReference(fileName);

            var isExists = await cfile.ExistsAsync();

            if (isExists) return cfile.Uri.AbsoluteUri;

            throw new FileNotFoundException("Dosya bulunamadı");
        }
    }
}
