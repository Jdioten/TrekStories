﻿using Microsoft.Azure;
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
            container.CreateIfNotExists();
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);

            try
            {
                blockBlob.UploadFromStream(stream);
                return blockBlob;
            }
            catch (Exception e)
            {
                var r = e.Message;
                return null;
            }
        }

        public void DeleteBlob(string blobName, string containerName)
        {
            CloudBlobContainer container = GetCloudBlobContainer(containerName);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);
            blockBlob.DeleteIfExists();
        }


        //This method will most probably not be required as it can be downloaded straight from URL
        //public CloudBlockBlob DownloadBlob(string blobName, string containerName)
        //{
        //    CloudBlobContainer container = GetCloudBlobContainer(containerName);
        //    CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);
        //    // blockBlob.DownloadToStream(Response.OutputStream);
        //    return blockBlob;
        //}
    }
}