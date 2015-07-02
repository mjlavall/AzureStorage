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
        public ListView ActiveListView
        {
            get
            {
                if (tabControl.SelectedTab == null) return null;
                return tabControl.SelectedTab.Controls.Find(tabControl.SelectedTab.Name + "ListView", false).FirstOrDefault() as ListView;
            }
        }
        private ListViewColumnSorter lvwColumnSorter;

        public Main()
        {
            lvwColumnSorter = new ListViewColumnSorter();
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            statusText.Text = "Connected";
            LoadTabs();
            UpdateAllBlobLists();
        }

        public void LoadTabs()
        {
            var selectedTab = tabControl.SelectedTab ?? null;  
            tabControl.Controls.Clear();
            var i = 0;
            foreach (var container in CloudAccess.GetContainers())
            {
                container.FetchAttributes();

                var tabTitle = "--";
                container.Metadata.TryGetValue("Title", out tabTitle);
                tabControl.Controls.Add(CreateTab(container.Name, tabTitle, i++));
            }
            if (selectedTab != null)
            {
                var tabToSelect = tabControl.Controls.Find(selectedTab.Name, false).FirstOrDefault();
                if (tabToSelect != null)
                {
                    tabControl.SelectTab(selectedTab.Name);
                }
                else
                {
                    if (tabControl.Controls.Count > 0) tabControl.SelectTab(tabControl.Controls[0].Name);
                }
            }
            else
            {
                if (tabControl.Controls.Count > 0) tabControl.SelectTab(tabControl.Controls[0].Name);
            }
        }

        public void UpdateAllBlobLists()
        {
            foreach (var tab in tabControl.Controls)
            {
                UpdateBlobList(((TabPage)tab).Name);
            }
        }

        public void UpdateBlobList(string tabName = null)
        {
            if (tabName == null && tabControl.SelectedTab != null) tabName = tabControl.SelectedTab.Name;
            var tab = tabControl.Controls.Find(tabName ?? "", false).FirstOrDefault() as TabPage;
            if (tab == null) return;
            var listView = tab.Controls.Find(tab.Text + "ListView", false).FirstOrDefault() as ListView;
            if (listView == null) return;
            if (listView.InvokeRequired)
            {
                Invoke((MethodInvoker)(() => listView.Items.Clear()));
            }
            else
            {
                listView.Items.Clear();
            }
            foreach (var item in CloudAccess.GetFiles(tabName))
            {
                if (!(item is CloudBlockBlob)) continue;
                var blob = (CloudBlockBlob)item;
                var listItem = new ListViewItem(blob.Name);
                listItem.SubItems.Add(GetFileSize(blob.Properties.Length));
                listItem.SubItems.Add(blob.Properties.LastModified.HasValue ? blob.Properties.LastModified.Value.ToString("G") : "null");
                if (listView.InvokeRequired)
                {
                    Invoke((MethodInvoker)(() => listView.Items.Add(listItem)));
                }
                else
                {
                    listView.Items.Add(listItem);
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
                    bool deleteLocal =
                        MessageBox.Show("Delete local files after upload?", "Delete Local?", MessageBoxButtons.YesNo) ==
                        DialogResult.Yes;
                    CloudAccess.UploadFiles(ofd.SafeFileNames, ofd.FileNames, deleteLocal, tabControl.SelectedTab.Name, this);
                }
            }
            UpdateBlobList();
        }

        private void buttonDownload_Click(object sender, EventArgs e)
        {
            var blobItems = (from object item in ActiveListView.SelectedItems select item as ListViewItem).ToList();
            if (blobItems.Count == 0) return;
            using (var fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    var path = fbd.SelectedPath;
                    CloudAccess.DownloadFiles(blobItems.Select(b => b.Text).ToArray(), blobItems.Select(b => Path.Combine(path, b.Text)).ToArray(), tabControl.SelectedTab.Name, this);
                }
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            var blobItems = (from object item in ActiveListView.SelectedItems select item as ListViewItem).ToList();
            if (blobItems.Count == 0) return;
            if (MessageBox.Show("Are you sure you want to delete the selected files?", "Confirm File Deletion", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                var success = CloudAccess.DeleteFiles(blobItems.Select(b => b.Text).ToList(), tabControl.SelectedTab.Name);
                if (success)
                {
                    UpdateStatusText(blobItems.Count == 1
                        ? string.Format("{0} was successfully deleted.", blobItems[0].Text)
                        : "All selected files were successfully deleted.");
                }
                else
                {
                    UpdateStatusText(blobItems.Count == 1
                        ? string.Format("Failed to delete {0}", blobItems[0].Text)
                        : "Some selected files failed to be deleted...");
                }
                UpdateBlobList();
            }
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

        protected void ListViewColumnClick(object sender, ColumnClickEventArgs e)
        {
            var listView = sender as ListView;
            if (listView == null) return;
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                lvwColumnSorter.Order = lvwColumnSorter.Order == SortOrder.Ascending
                    ? SortOrder.Descending
                    : SortOrder.Ascending;
            }
            else
            {
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }
            listView.Sort();
        }

        public string GetFileSize(double bytes)
        {
            string[] units = {"B", "KB", "MB", "GB", "TB"};
            if (bytes < 1024) return "< 1 KB";
            int i = 0;
            while (bytes > 1024)
            {
                bytes /= 1024.0;
                i++;
            }
            if (i >= units.Length) return "??";
            return string.Format("{0:0.00} {1}", bytes, units[i]);
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            //UpdateBlobList();
        }

        private void buttonCreateContainer_Click(object sender, EventArgs e)
        {
            using (var createForm = new CreateContainerForm())
            {
                if (createForm.ShowDialog() == DialogResult.OK)
                {
                    var container = CloudAccess.CreateContainer(createForm.ContainerName);
                    container.FetchAttributes();
                    var tabTitle = "--";
                    container.Metadata.TryGetValue("Title", out tabTitle);
                    if (tabControl.Controls.ContainsKey(container.Name))
                    {
                        tabControl.Controls.RemoveByKey(container.Name);
                    }
                    tabControl.Controls.Add(CreateTab(container.Name, tabTitle, tabControl.Controls.Count));
                }
            }
        }

        private void buttonDeleteContainer_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(string.Format("Are you sure you want to delete container [{0}]",
                tabControl.SelectedTab.Text), "Confirm Container Deletion", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;
            CloudAccess.DeleteContainer(tabControl.SelectedTab.Name);
            tabControl.Controls.Remove(tabControl.SelectedTab);
        }

        private TabPage CreateTab(string tabName, string tabTitle, int index)
        {
            var tab = new TabPage()
            {
                Location = new Point(0, 0),
                Name = tabName,
                Padding = new Padding(3),
                Size = new Size(500, 227),
                TabIndex = index,
                Text = tabTitle,
                UseVisualStyleBackColor = true
            };
            var listView = new ListView()
            {
                FullRowSelect = true,
                GridLines = true,
                Location = new Point(-2, -1),
                Name = tab.Text + "ListView",
                Size = new Size(496, 237),
                TabIndex = 1,
                UseCompatibleStateImageBehavior = false,
                View = View.Details,
                ListViewItemSorter = lvwColumnSorter
            };
            listView.ColumnClick += ListViewColumnClick;
            listView.Columns.Add("Filename", 250);
            listView.Columns.Add("Size", 75);
            listView.Columns.Add("Date", 150);
            tab.Controls.Add(listView);
            return tab;
        }
    }
}
