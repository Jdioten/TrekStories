using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;

namespace TrekStories.Utilities
{
    public class BlobUtility
    {
        private CloudBlobContainer GetCloudBlobContainer(string containerName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                    CloudConfigurationManager.GetSetting("trekstoriesblobstorage_AzureStorageConnectionString"));
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);
            return container;
        }

        public async Task<string> UploadBlobAsync(string blobName, string containerName, Stream stream)
        {
            CloudBlobContainer container = GetCloudBlobContainer(containerName);
            await container.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Container, null, null);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);

            try
            {
                await blockBlob.UploadFromStreamAsync(stream);
                return blockBlob.Uri.ToString();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task DeleteBlobAsync(string blobName, string containerName)
        {
            CloudBlobContainer container = GetCloudBlobContainer(containerName);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);
            await blockBlob.DeleteIfExistsAsync();
        }
    }
}