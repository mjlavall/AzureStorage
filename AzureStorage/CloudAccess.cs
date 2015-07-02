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
        private const int BlockSize = 256 * 1024;

        public static IEnumerable<IListBlobItem> GetFiles(string container)
        {
            return GetContainer(container).ListBlobs();
        }

        public static CloudBlobContainer GetContainer(string containerName)
        {
            return BlobClient.GetContainerReference(containerName);
        }

        public static CloudBlobContainer CreateContainer(string containerName)
        {
            var newContainer = BlobClient.GetContainerReference(containerName.ToLower().Replace(" ",""));
            
            if (newContainer.Exists())
            {
                newContainer.Metadata["Title"] = containerName;
                newContainer.SetMetadata();
                return newContainer;
            }
            newContainer.CreateIfNotExists();
            newContainer.Metadata.Add("Title", containerName);
            newContainer.SetMetadata();
            newContainer.FetchAttributes();
            return newContainer;
        }

        public static void DeleteContainer(string containerName)
        {
            BlobClient.GetContainerReference(containerName).DeleteIfExists();
        }

        public static List<CloudBlobContainer> GetContainers()
        {
            return BlobClient.ListContainers().ToList();
        }

        public static async Task<bool> UploadFile(string fileName, string filePath, int totalFiles, int currentFile, string container, Main mainScreen)
        {
            var Container = BlobClient.GetContainerReference(container);
            var blockBlob = Container.GetBlockBlobReference(fileName);
            blockBlob.StreamWriteSizeInBytes = BlockSize;
            var bytesToUpload = (new FileInfo(filePath)).Length;
            long fileSize = bytesToUpload;
            if (bytesToUpload <= BlockSize)
            {
                await blockBlob.UploadFromFileAsync(filePath, FileMode.Open);
            }
            else
            {
                var blockIds = new List<string>();
                int index = 1;
                long startPosition = 0;
                long bytesUploaded = 0;

                // upload bytesToRead bytes at a time
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
                    // upload current bytesToRead bytes
                    var ado = blockBlob.PutBlockAsync(blockId, new MemoryStream(blobContents), null);
                    await ado.ContinueWith(t =>
                    {
                        mre.Set();
                        bytesUploaded += bytesToRead;
                        bytesToUpload -= bytesToRead;
                        startPosition += bytesToRead;
                        index++;
                        var percentComplete = (int)Math.Min((double)bytesUploaded/fileSize*100, 100);
                        mainScreen.UpdateProgressBar(percentComplete);
                        mainScreen.UpdateStatusText(string.Format("{0} ({1}%) Uploading {2}...", totalFiles > 1 ? string.Format("[{0}/{1}]", currentFile, totalFiles) : "", percentComplete, fileName));
                    });
                    mre.WaitOne();
                } while (bytesToUpload > 0);

                // finalize the upload
                await blockBlob.PutBlockListAsync(blockIds);
            }
            return true;
        }

        public static async void UploadFiles(string[] fileNames, string[] filePaths, bool deleteLocal, string container, Main mainScreen)
        {
            for (var i = 0; i < fileNames.Length; i++)
            {
                // upload the file
                await UploadFile(fileNames[i], filePaths[i], fileNames.Length, i + 1, container, mainScreen);
                if(deleteLocal) File.Delete(filePaths[i]);
                // update the mainscreen to clear progress bar
                mainScreen.UpdateProgressBar(0);
                mainScreen.UpdateStatusText(string.Format("{0} was uploaded successfully.", fileNames[i]));
                mainScreen.UpdateBlobList(container);
            }
        }

        public static async Task<bool> DownloadFile(string fileName, string filePath, int totalFiles, int currentFile, string container, Main mainScreen)
        {
            var blockBlob = GetContainer(container).GetBlobReferenceFromServer(fileName);
            var bytesToDownload = blockBlob.Properties.Length;
            long fileSize = bytesToDownload;
            if (fileSize <= BlockSize)
            {
                mainScreen.UpdateStatusText(string.Format("{0} Downloading {1}...", totalFiles > 1 ? string.Format("[{0}/{1}]", currentFile, totalFiles) : "", fileName));
                await blockBlob.DownloadToFileAsync(filePath, FileMode.Create);
            }
            else
            {
                int index = 1;
                long startPosition = 0;
                long bytesDownloaded = 0;

                // upload bytesToRead bytes at a time
                if(File.Exists(filePath + ".tmp")) File.Delete(filePath + ".tmp");
                var fileStream = new FileStream(filePath + ".tmp", FileMode.Create);
                do
                {
                    var bytesToWrite = Math.Min(BlockSize, bytesToDownload);
                    var blobContents = new byte[bytesToWrite];
                    var mre = new ManualResetEvent(false);
                    // download current bytesToWrite bytes
                    var ado = blockBlob.DownloadRangeToByteArrayAsync(blobContents, 0, startPosition,
                        (int) bytesToWrite);
                    await ado.ContinueWith(t =>
                    {
                        fileStream.Position = startPosition;
                        fileStream.Write(blobContents, 0, (int) bytesToWrite);
                        mre.Set();
                        bytesDownloaded += bytesToWrite;
                        bytesToDownload -= bytesToWrite;
                        startPosition += bytesToWrite;
                        index++;
                        var percentComplete = (int) Math.Min((double) bytesDownloaded/fileSize*100, 100);
                        mainScreen.UpdateProgressBar(percentComplete);
                        mainScreen.UpdateStatusText(string.Format("{0} ({1}%) Downloading {2}...",
                            totalFiles > 1 ? string.Format("[{0}/{1}]", currentFile, totalFiles) : "",
                            percentComplete, fileName));

                    });
                    mre.WaitOne();
                } while (bytesToDownload > 0);
                fileStream.Close();
            }
            return true;
        }

        public static async void DownloadFiles(string[] fileNames, string[] filePaths, string container, Main mainScreen)
        {
            for (var i = 0; i < fileNames.Length; i++)
            {
                await DownloadFile(fileNames[i], filePaths[i], fileNames.Length, i + 1, container, mainScreen);
                if (File.Exists(filePaths[i])) File.Delete(filePaths[i]);
                File.Move(filePaths[i] + ".tmp", filePaths[i]);
                if (File.Exists(filePaths[i] + ".tmp")) File.Delete(filePaths[i] + ".tmp");

                // update the mainscreen to clear progress bar
                mainScreen.UpdateProgressBar(0);
                mainScreen.UpdateStatusText(string.Format("{0} was downloaded successfully.", fileNames[i]));
                mainScreen.UpdateBlobList(container);
            }
        }

        public static bool DeleteFile(string fileName, string container)
        {
            var blockBlob = GetContainer(container).GetBlockBlobReference(fileName);
            return blockBlob.DeleteIfExists();
        }

        public static bool DeleteFiles(List<string> fileNames, string container)
        {
            return fileNames.Aggregate(true, (current, deleteFile) => DeleteFile(deleteFile, container) && current);
        }
    }
}
