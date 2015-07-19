using System;

namespace AzureStorage
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonUploadFiles = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.buttonDownload = new System.Windows.Forms.Button();
            this.statusText = new System.Windows.Forms.Label();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.buttonCreateContainer = new System.Windows.Forms.Button();
            this.buttonDeleteContainer = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonUploadFiles
            // 
            this.buttonUploadFiles.Location = new System.Drawing.Point(20, 18);
            this.buttonUploadFiles.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonUploadFiles.Name = "buttonUploadFiles";
            this.buttonUploadFiles.Size = new System.Drawing.Size(112, 35);
            this.buttonUploadFiles.TabIndex = 0;
            this.buttonUploadFiles.Text = "Upload Files";
            this.buttonUploadFiles.UseVisualStyleBackColor = true;
            this.buttonUploadFiles.Click += new System.EventHandler(this.buttonUploadFiles_Click);
            // 
            // buttonDelete
            // 
            this.buttonDelete.Location = new System.Drawing.Point(286, 18);
            this.buttonDelete.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(123, 35);
            this.buttonDelete.TabIndex = 2;
            this.buttonDelete.Text = "Delete Files";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(22, 471);
            this.progressBar.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(746, 35);
            this.progressBar.TabIndex = 3;
            // 
            // buttonDownload
            // 
            this.buttonDownload.Location = new System.Drawing.Point(141, 18);
            this.buttonDownload.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonDownload.Name = "buttonDownload";
            this.buttonDownload.Size = new System.Drawing.Size(136, 35);
            this.buttonDownload.TabIndex = 4;
            this.buttonDownload.Text = "Download Files";
            this.buttonDownload.UseVisualStyleBackColor = true;
            this.buttonDownload.Click += new System.EventHandler(this.buttonDownload_Click);
            // 
            // statusText
            // 
            this.statusText.AutoSize = true;
            this.statusText.Location = new System.Drawing.Point(18, 511);
            this.statusText.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.statusText.Name = "statusText";
            this.statusText.Size = new System.Drawing.Size(56, 20);
            this.statusText.TabIndex = 5;
            this.statusText.Text = "Status";
            // 
            // tabControl
            // 
            this.tabControl.Location = new System.Drawing.Point(22, 63);
            this.tabControl.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(750, 398);
            this.tabControl.TabIndex = 6;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
            // 
            // buttonCreateContainer
            // 
            this.buttonCreateContainer.Location = new System.Drawing.Point(448, 18);
            this.buttonCreateContainer.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonCreateContainer.Name = "buttonCreateContainer";
            this.buttonCreateContainer.Size = new System.Drawing.Size(156, 35);
            this.buttonCreateContainer.TabIndex = 7;
            this.buttonCreateContainer.Text = "Create Container";
            this.buttonCreateContainer.UseVisualStyleBackColor = true;
            this.buttonCreateContainer.Click += new System.EventHandler(this.buttonCreateContainer_Click);
            // 
            // buttonDeleteContainer
            // 
            this.buttonDeleteContainer.Location = new System.Drawing.Point(614, 18);
            this.buttonDeleteContainer.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonDeleteContainer.Name = "buttonDeleteContainer";
            this.buttonDeleteContainer.Size = new System.Drawing.Size(154, 35);
            this.buttonDeleteContainer.TabIndex = 8;
            this.buttonDeleteContainer.Text = "Delete Container";
            this.buttonDeleteContainer.UseVisualStyleBackColor = true;
            this.buttonDeleteContainer.Click += new System.EventHandler(this.buttonDeleteContainer_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(795, 540);
            this.Controls.Add(this.buttonDeleteContainer);
            this.Controls.Add(this.buttonCreateContainer);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.statusText);
            this.Controls.Add(this.buttonDownload);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.buttonUploadFiles);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "Main";
            this.Text = "Azure Storage";
            this.Load += new System.EventHandler(this.Main_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonUploadFiles;
        private System.Windows.Forms.Button buttonDelete;
        public System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button buttonDownload;
        public System.Windows.Forms.Label statusText;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.Button buttonCreateContainer;
        private System.Windows.Forms.Button buttonDeleteContainer;
    }
}

