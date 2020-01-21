using Microsoft.WindowsAzure.Storage;
using System.IO;
using System.Threading.Tasks;
using NitelikliBilisim.Core.Services.Abstracts;
using NitelikliBilisim.Core.ViewModels.Cart;

namespace NitelikliBilisim.Core.Services
{
    public class StorageService : IStorageService
    {
        private const string ReferanceName = "nbuploads";
        private const string AccessKey = "DefaultEndpointsProtocol=https;AccountName=niteliklidatastore;AccountKey=KxpcLymDmly4Gv0UG3LhUgr1olSbsSlfJ3cOy2jAPm2DZ94rTJ6GfXZFhiUGrX+FsFFeTr91jf1gcWIg/JbZ3g==;EndpointSuffix=core.windows.net";
        private const string SasToken =
            "?sv=2019-02-02&ss=bfqt&srt=sco&sp=rwdlacup&se=2030-01-21T16:45:36Z&st=2020-01-21T08:45:36Z&spr=https&sig=ySGn3tG3eFja1a6DDNqhtgq2gyoqwBElsLdQ7kmpf5k%3D";
        private readonly CloudStorageAccount _storageAccount;

        public StorageService()
        {
            _storageAccount = CloudStorageAccount.Parse(AccessKey);
        }

        public async Task<string> UploadFile(Stream fileStream, string fileName, string folderName)
        {
            var ext = Path.GetExtension(fileName);
            fileName = Path.GetFileNameWithoutExtension(fileName);
            fileName = StringHelper.UrlFormatConverter(fileName) + StringHelper.GenerateUniqueCode() + ext;
            fileStream.Position = 0;
            var fileClient = _storageAccount.CreateCloudFileClient();
            var share = fileClient.GetShareReference(ReferanceName);
            var rootDir = share.GetRootDirectoryReference();
            var fileDir = rootDir.GetDirectoryReference(folderName);

            await fileDir.CreateIfNotExistsAsync();

            var cfile = fileDir.GetFileReference(fileName);
            await cfile.UploadFromStreamAsync(fileStream);
            return $"{folderName}/{fileName}";
        }

        public async Task<string> DownloadFile(string fileName, string folderName)
        {
            var fileClient = _storageAccount.CreateCloudFileClient();
            var share = fileClient.GetShareReference(ReferanceName);
            var rootDir = share.GetRootDirectoryReference();
            var fileDir = rootDir.GetDirectoryReference(folderName);
            var cfile = fileDir.GetFileReference(fileName);

            var isExists = await cfile.ExistsAsync();

            if (isExists) return cfile.Uri.AbsoluteUri + SasToken;

            throw new FileNotFoundException("Dosya bulunamadı");
        }

        public async Task<MemoryStream> DownloadFileStream(string fileName, string folderName)
        {
            var fileClient = _storageAccount.CreateCloudFileClient();
            var share = fileClient.GetShareReference(ReferanceName);
            var rootDir = share.GetRootDirectoryReference();
            var fileDir = rootDir.GetDirectoryReference(folderName);
            var cfile = fileDir.GetFileReference(fileName);

            var isExists = await cfile.ExistsAsync();

            if (isExists)
            {
                var memoryStream = new MemoryStream();
                await cfile.DownloadToStreamAsync(memoryStream, null, null, null);
                memoryStream.Position = 0;
                return memoryStream;
            }

            throw new FileNotFoundException("Dosya bulunamadı");
        }

        public async Task<bool> DeleteFile(string fileName, string folderName)
        {
            var fileClient = _storageAccount.CreateCloudFileClient();
            var share = fileClient.GetShareReference(ReferanceName);
            var rootDir = share.GetRootDirectoryReference();
            var fileDir = rootDir.GetDirectoryReference(folderName);
            var cfile = fileDir.GetFileReference(fileName);

            var isExists = await cfile.ExistsAsync();

            if (isExists)
            {
                await cfile.DeleteAsync();
                return true;
            }

            throw new FileNotFoundException("Dosya bulunamadı");
        }
    }
}
