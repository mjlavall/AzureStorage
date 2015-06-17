using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.File;

namespace AzureStorage
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

        public static async void UploadFile(string fileName, string filePath, Main mainScreen)
        {
            var blockBlob = Container.GetBlockBlobReference(fileName);
            blockBlob.StreamWriteSizeInBytes = BlockSize;
            var bytesToUpload = (new FileInfo(filePath)).Length;
            long fileSize = bytesToUpload;
            if (bytesToUpload < BlockSize)
            {
                var ado = blockBlob.UploadFromFileAsync(filePath, FileMode.Open);
                await ado.ContinueWith(t =>
                {
                    UpdateProgressBar(mainScreen, 0);
                    UpdateStatusText(mainScreen, string.Format("{0} was uploaded successfully.", fileName));
                    mainScreen.UpdateBlobList();
                });
            }
            else
            {
                var blockIds = new List<string>();
                int index = 1;
                long startPosition = 0;
                long bytesUploaded = 0;
                do
                {
                    var bytesToRead = Math.Min(BlockSize, bytesToUpload);
                    var blobContents = new byte[bytesToRead];
                    using (var fileStream = new FileStream(filePath, FileMode.Open))
                    {
                        fileStream.Position = startPosition;
                        fileStream.Read(blobContents, 0, (int) bytesToRead);
                    }
                    var blockId = Convert.ToBase64String(Encoding.UTF8.GetBytes(index.ToString("d6")));
                    blockIds.Add(blockId);
                    var mre = new ManualResetEvent(false);
                    var ado = blockBlob.PutBlockAsync(blockId, new MemoryStream(blobContents), null);
                    await ado.ContinueWith(t =>
                    {
                        mre.Set();
                        bytesUploaded += bytesToRead;
                        bytesToUpload -= bytesToRead;
                        startPosition += bytesToRead;
                        index++;
                        var percentComplete = (int)Math.Min((double)bytesUploaded/fileSize*100, 100);
                        UpdateProgressBar(mainScreen, percentComplete);
                        UpdateStatusText(mainScreen, string.Format("[{0}%] Uploading {1}...", percentComplete, fileName));
                    });
                    mre.WaitOne();
                } while (bytesToUpload > 0);
                var pbl = blockBlob.PutBlockListAsync(blockIds);
                await pbl.ContinueWith(t =>
                {
                    UpdateProgressBar(mainScreen, 0);
                    UpdateStatusText(mainScreen, string.Format("{0} was uploaded successfully.", fileName));
                });
            }
        }

        public static void DownloadFile(string fileName, string filePath, Main mainScreen)
        {
            var blockBlob = Container.GetBlockBlobReference(fileName);
            blockBlob.DownloadToFile(filePath, FileMode.CreateNew);
        }

        public static void DeleteFile(string fileName)
        {
            var blockBlob = Container.GetBlockBlobReference(fileName);
            blockBlob.DeleteIfExists();
        }

        public static void UpdateProgressBar(Main mainScreen, int progress)
        {
            if (mainScreen.progressBar.InvokeRequired)
            {
                mainScreen.Invoke((MethodInvoker)(() =>
                {
                    mainScreen.progressBar.Value = progress;
                }));
            }
            else
            {
                mainScreen.progressBar.Value = progress;
            }
        }

        public static void UpdateStatusText(Main mainScreen, string status)
        {
            if (mainScreen.statusText.InvokeRequired)
            {
                mainScreen.Invoke((MethodInvoker)(() =>
                {
                    mainScreen.statusText.Text = status;
                }));
            }
            else
            {
                mainScreen.statusText.Text = status;
            }
        }
    }
}
