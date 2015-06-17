using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
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
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            statusText.Text = "Connected";
            UpdateBlobList();
        }

        public void UpdateBlobList()
        {
            if (listBoxBlobs.InvokeRequired)
            {
                Invoke((MethodInvoker)(() => listBoxBlobs.Items.Clear()));
            }
            else
            {
                listBoxBlobs.Items.Clear();
            }
            foreach (var item in CloudAccess.GetFiles())
            {
                if (item is CloudBlockBlob)
                {
                    var blob = (CloudBlockBlob)item;
                    var blobItem = new BlobItem { Name = blob.Name, Url = blob.Uri.ToString()};
                    if (listBoxBlobs.InvokeRequired)
                    {
                        Invoke((MethodInvoker)(() => listBoxBlobs.Items.Add(blobItem)));
                    }
                    else
                    {
                        listBoxBlobs.Items.Add(blobItem);
                    }
                }
            }
        }

        private void buttonUploadFile_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog
            {
                Filter = "All Files|*.*",
                Title = "Choose a file"
            })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    CloudAccess.UploadFile(ofd.SafeFileName, ofd.FileName, this);
                }
            }
            UpdateBlobList();
        }

        private void buttonDownload_Click(object sender, EventArgs e)
        {
            var blobItem = listBoxBlobs.SelectedItem as BlobItem;
            if (blobItem == null) return;
            using (var sfd = new SaveFileDialog
            {
                FileName = blobItem.Name,
                Filter = "All Files|*.*",
                Title = "Save the file"
            })
            {
                if (sfd.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(sfd.FileName))
                {
                    CloudAccess.DownloadFile(blobItem.Name, sfd.FileName, this);
                }
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            var blobItem = listBoxBlobs.SelectedItem as BlobItem;
            if (blobItem == null) return;
            CloudAccess.DeleteFile(blobItem.Name);
            UpdateBlobList();
        }

        public class BlobItem
        {
            public string Name { get; set; }
            public string Url { get; set; }
            public override string ToString()
            {
                return Name;
            }
        }
    }
}
