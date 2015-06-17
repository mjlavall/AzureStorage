using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.File;

namespace AzureStorageStreaming.Models
{
    public static class CloudAccess
    {
        private static CloudStorageAccount StorageAccount
        {
            get { return CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString")); }
        }
        private static CloudBlobClient BlobClient
        {
            get
            {
                var blobClient = StorageAccount.CreateCloudBlobClient();
                blobClient.DefaultRequestOptions.SingleBlobUploadThresholdInBytes = 1024 * 1024;
                return blobClient;
            }
        }
        private static CloudBlobContainer Container
        {
            get
            {
                var container = BlobClient.GetContainerReference("gopro");
                container.CreateIfNotExists();
                container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
                return container;
            }
        }
        private const int BlockSize = 256 * 1024;

        public static IEnumerable<IListBlobItem> GetFiles()
        {
            return Container.ListBlobs();
        }
    }
}