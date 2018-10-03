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

        public async Task<string> UploadBlobAsync(HttpPostedFileBase file, string containerName)
        {
            //check file size and extension
            if (FileUploadUtility.InvalidFileSize(file))
            {
                throw new FileLoadException("The file cannot be bigger than 7MB.");
            }
            if (FileUploadUtility.InvalidFileExtension(file))
            {
                throw new FileLoadException("The file type is not authorized for upload.");
            }

            CloudBlobContainer container = GetCloudBlobContainer(containerName);
            await container.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Blob, null, null);

            //build filename with timestamp to reduce risk of duplicates 
            string blobName = FileUploadUtility.GetFilenameWithTimestamp(file.FileName);

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);

            try
            {
                await blockBlob.UploadFromStreamAsync(file.InputStream);
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