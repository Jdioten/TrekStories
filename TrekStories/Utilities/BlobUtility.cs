using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
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

        public CloudBlockBlob UploadBlob(string blobName, string containerName, Stream stream)
        {
            CloudBlobContainer container = GetCloudBlobContainer(containerName);
            container.CreateIfNotExists(BlobContainerPublicAccessType.Container, null, null);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);

            try
            {
                blockBlob.UploadFromStream(stream);
                return blockBlob;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void DeleteBlob(string blobName, string containerName)
        {
            CloudBlobContainer container = GetCloudBlobContainer(containerName);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);
            blockBlob.DeleteIfExists();
        }
    }
}