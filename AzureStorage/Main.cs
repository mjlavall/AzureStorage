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
        public ListBox ActiveListBox
        {
            get
            {
                return tabControl.SelectedTab.Controls.Find(tabControl.SelectedTab.Text + "ListBox", false).FirstOrDefault() as ListBox;
            }
        }

        public Main()
        {
            InitializeComponent();
            statusText.Text = "Connected";
            CloudAccess.CreateContainer("test");
            var containers = CloudAccess.GetContainers();
            CloudAccess.ContainerName = containers.Count > 0 ? containers[0] : "root";
            LoadTabs();
            UpdateBlobList();
        }

        public void LoadTabs()
        {
            var i = 0;
            var tabPages = CloudAccess.GetContainers().Select(container => new TabPage()
            {
                Location = new Point(0, 0), Name = container, Padding = new Padding(3), Size = new Size(500, 227), TabIndex = i++, Text = container, UseVisualStyleBackColor = true
            }).ToList();
            foreach (var tab in tabPages)
            {
                tab.Controls.Add(new ListBox()
                {
                    FormattingEnabled = true,
                    Location = new Point(-2, -1),
                    Name = tab.Text + "ListBox",
                    SelectionMode = SelectionMode.MultiExtended,
                    Size = new Size(496, 237),
                    TabIndex = 1
                });
                
                tabControl.Controls.Add(tab);
            }
        }

        public void UpdateBlobList()
        {
            var tab = tabControl.SelectedTab;
            if (ActiveListBox == null) return;
            CloudAccess.ContainerName = tab.Text;
            if (ActiveListBox.InvokeRequired)
            {
                Invoke((MethodInvoker)(() => ActiveListBox.Items.Clear()));
            }
            else
            {
                ActiveListBox.Items.Clear();
            }
            foreach (var item in CloudAccess.GetFiles())
            {
                if (!(item is CloudBlockBlob)) continue;
                var blob = (CloudBlockBlob)item;
                var blobItem = new BlobItem { Name = blob.Name, Url = blob.Uri.ToString()};
                if (ActiveListBox.InvokeRequired)
                {
                    Invoke((MethodInvoker)(() => ActiveListBox.Items.Add(blobItem)));
                }
                else
                {
                    ActiveListBox.Items.Add(blobItem);
                }
            }
        }

        private void buttonUploadFiles_Click(object sender, EventArgs e)
        {

            using (var ofd = new OpenFileDialog()
            {
                Multiselect = true,
                Filter = "All Files|*.*",
                Title = "Choose Files"
            })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    CloudAccess.UploadFiles(ofd.SafeFileNames, ofd.FileNames, this);
                }
            }
            UpdateBlobList();
        }

        private void buttonDownload_Click(object sender, EventArgs e)
        {
            var blobItems = (from object item in ActiveListBox.SelectedItems select item as BlobItem).ToList();
            if (blobItems.Count == 0) return;
            using (var fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    var path = fbd.SelectedPath;
                    CloudAccess.DownloadFiles(blobItems.Select(b => b.Name).ToArray(), blobItems.Select(b => Path.Combine(path, b.Name)).ToArray(), this);
                }
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            var blobItems = (from object item in ActiveListBox.SelectedItems select item as BlobItem).ToList();
            if (blobItems.Count == 0) return;
            var success = CloudAccess.DeleteFiles(blobItems.Select(b => b.Name).ToList());
            if (success)
            {
                UpdateStatusText(blobItems.Count == 1
                    ? string.Format("{0} was successfully deleted.", blobItems[0].Name)
                    : "All selected files were successfully deleted.");
            }
            else
            {
                UpdateStatusText(blobItems.Count == 1
                    ? string.Format("Failed to delete {0}", blobItems[0].Name)
                    : "Some selected files failed to be deleted...");
            }
            UpdateBlobList();
        }

        public void UpdateProgressBar(int progress)
        {
            if (progressBar.InvokeRequired)
            {
                Invoke((MethodInvoker)(() =>
                {
                    progressBar.Value = progress;
                }));
            }
            else
            {
                progressBar.Value = progress;
            }
        }

        public void UpdateStatusText(string status)
        {
            if (statusText.InvokeRequired)
            {
                Invoke((MethodInvoker)(() =>
                {
                    statusText.Text = status;
                }));
            }
            else
            {
                statusText.Text = status;
            }
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

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateBlobList();
        }
    }
}
