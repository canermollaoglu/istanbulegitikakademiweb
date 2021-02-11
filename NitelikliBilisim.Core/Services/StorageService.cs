using Azure.Storage.Blobs;
using MUsefulMethods;
using NitelikliBilisim.Core.Services.Abstracts;
using System.IO;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.Services
{
    //TODO : Storage paketi güncellenmeli. Depreched 
    public class StorageService : IStorageService
    {
        //commit  
        private const string ReferanceName = "nbuploads";
        private const string AccessKey = "DefaultEndpointsProtocol=https;AccountName=niteliklidatastore;AccountKey=KxpcLymDmly4Gv0UG3LhUgr1olSbsSlfJ3cOy2jAPm2DZ94rTJ6GfXZFhiUGrX+FsFFeTr91jf1gcWIg/JbZ3g==;EndpointSuffix=core.windows.net";
        private const string SasToken =
            "?sv=2019-02-02&ss=bfqt&srt=sco&sp=rwdlacup&se=2030-01-21T16:45:36Z&st=2020-01-21T08:45:36Z&spr=https&sig=ySGn3tG3eFja1a6DDNqhtgq2gyoqwBElsLdQ7kmpf5k%3D";
        private readonly BlobServiceClient _storageAccount;

        public StorageService()
        {
            _storageAccount = new BlobServiceClient(AccessKey);
        }

        public string BlobUrl => "https://niteliklidatastore.blob.core.windows.net/";
        public async Task<string> UploadFile(Stream fileStream, string fileName, string containerName)
        {
            var containerClient = _storageAccount.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();
            await containerClient.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);
            var ext = Path.GetExtension(fileName);
            fileName = Path.GetFileNameWithoutExtension(fileName);
            fileName = StringHelpers.CharacterConverter(fileName) + StringHelpers.GenerateUniqueCode() + ext;
            var blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(fileStream);
            return $"{containerName}/{fileName}";
        }

        public async Task<Stream> DownloadFile(string fileName, string containerName)
        {
            var containerClient = _storageAccount.GetBlobContainerClient(containerName);
            //var blobClient = containerClient.GetBlobClient(fileName);
            var blobClient = containerClient.GetBlobClient(fileName);
            var info = await blobClient.DownloadAsync();
            return info.Value.Content;


            //var share = fileClient.GetShareReference(ReferanceName);
            //var rootDir = share.GetRootDirectoryReference();
            //var fileDir = rootDir.GetDirectoryReference(folderName);
            //var cfile = fileDir.GetFileReference(fileName);

            //var isExists = await cfile.ExistsAsync();

            //if (isExists) return cfile.Uri.AbsoluteUri + SasToken;

            //throw new FileNotFoundException("Dosya bulunamadı");
        }

        public async Task<bool> DeleteFile(string fileName, string containerName)
        {
            var containerClient = _storageAccount.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(fileName);
             var result = await blobClient.DeleteAsync();
            return result.Status == 1 ? true : false ;
        }
    }
}
