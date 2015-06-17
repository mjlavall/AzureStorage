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
            this.buttonUploadFile = new System.Windows.Forms.Button();
            this.listBoxBlobs = new System.Windows.Forms.ListBox();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.buttonDownload = new System.Windows.Forms.Button();
            this.statusText = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonUploadFile
            // 
            this.buttonUploadFile.Location = new System.Drawing.Point(13, 12);
            this.buttonUploadFile.Name = "buttonUploadFile";
            this.buttonUploadFile.Size = new System.Drawing.Size(75, 23);
            this.buttonUploadFile.TabIndex = 0;
            this.buttonUploadFile.Text = "Upload File";
            this.buttonUploadFile.UseVisualStyleBackColor = true;
            this.buttonUploadFile.Click += new System.EventHandler(this.buttonUploadFile_Click);
            // 
            // listBoxBlobs
            // 
            this.listBoxBlobs.FormattingEnabled = true;
            this.listBoxBlobs.Location = new System.Drawing.Point(13, 41);
            this.listBoxBlobs.Name = "listBoxBlobs";
            this.listBoxBlobs.Size = new System.Drawing.Size(236, 251);
            this.listBoxBlobs.TabIndex = 1;
            // 
            // buttonDelete
            // 
            this.buttonDelete.Location = new System.Drawing.Point(174, 12);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(75, 23);
            this.buttonDelete.TabIndex = 2;
            this.buttonDelete.Text = "Delete";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(13, 299);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(503, 23);
            this.progressBar.TabIndex = 3;
            // 
            // buttonDownload
            // 
            this.buttonDownload.Location = new System.Drawing.Point(93, 12);
            this.buttonDownload.Name = "buttonDownload";
            this.buttonDownload.Size = new System.Drawing.Size(75, 23);
            this.buttonDownload.TabIndex = 4;
            this.buttonDownload.Text = "Download";
            this.buttonDownload.UseVisualStyleBackColor = true;
            this.buttonDownload.Click += new System.EventHandler(this.buttonDownload_Click);
            // 
            // statusText
            // 
            this.statusText.AutoSize = true;
            this.statusText.Location = new System.Drawing.Point(12, 326);
            this.statusText.Name = "statusText";
            this.statusText.Size = new System.Drawing.Size(37, 13);
            this.statusText.TabIndex = 5;
            this.statusText.Text = "Status";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(528, 344);
            this.Controls.Add(this.statusText);
            this.Controls.Add(this.buttonDownload);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.listBoxBlobs);
            this.Controls.Add(this.buttonUploadFile);
            this.Name = "Main";
            this.Text = "Azure Storage";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonUploadFile;
        public System.Windows.Forms.ListBox listBoxBlobs;
        private System.Windows.Forms.Button buttonDelete;
        public System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button buttonDownload;
        public System.Windows.Forms.Label statusText;
    }
}

