namespace uTikDownloadHelper
{
    partial class frmDownload
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDownload));
            this.progMain = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.lblCurrentFile = new System.Windows.Forms.Label();
            this.btnDownload = new System.Windows.Forms.Button();
            this.fbd = new System.Windows.Forms.FolderBrowserDialog();
            this.label2 = new System.Windows.Forms.Label();
            this.lblTransferRate = new System.Windows.Forms.Label();
            this.chkTitle = new System.Windows.Forms.CheckBox();
            this.chkUpdate = new System.Windows.Forms.CheckBox();
            this.progressTimer = new System.Windows.Forms.Timer(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.lblAvgTransferRate = new System.Windows.Forms.Label();
            this.folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.lblDownloadingMetadata = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblTimeRemaining = new System.Windows.Forms.Label();
            this.chkDLC = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // progMain
            // 
            resources.ApplyResources(this.progMain, "progMain");
            this.progMain.Maximum = 10000;
            this.progMain.Name = "progMain";
            this.progMain.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // lblCurrentFile
            // 
            resources.ApplyResources(this.lblCurrentFile, "lblCurrentFile");
            this.lblCurrentFile.Name = "lblCurrentFile";
            // 
            // btnDownload
            // 
            resources.ApplyResources(this.btnDownload, "btnDownload");
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.UseVisualStyleBackColor = true;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // fbd
            // 
            resources.ApplyResources(this.fbd, "fbd");
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // lblTransferRate
            // 
            resources.ApplyResources(this.lblTransferRate, "lblTransferRate");
            this.lblTransferRate.Name = "lblTransferRate";
            // 
            // chkTitle
            // 
            resources.ApplyResources(this.chkTitle, "chkTitle");
            this.chkTitle.Name = "chkTitle";
            this.chkTitle.UseVisualStyleBackColor = true;
            // 
            // chkUpdate
            // 
            resources.ApplyResources(this.chkUpdate, "chkUpdate");
            this.chkUpdate.Name = "chkUpdate";
            this.chkUpdate.UseVisualStyleBackColor = true;
            // 
            // progressTimer
            // 
            this.progressTimer.Interval = 1000;
            this.progressTimer.Tick += new System.EventHandler(this.progressTimer_Tick);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // lblAvgTransferRate
            // 
            resources.ApplyResources(this.lblAvgTransferRate, "lblAvgTransferRate");
            this.lblAvgTransferRate.Name = "lblAvgTransferRate";
            // 
            // folderDialog
            // 
            resources.ApplyResources(this.folderDialog, "folderDialog");
            // 
            // lblDownloadingMetadata
            // 
            resources.ApplyResources(this.lblDownloadingMetadata, "lblDownloadingMetadata");
            this.lblDownloadingMetadata.Name = "lblDownloadingMetadata";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // lblTimeRemaining
            // 
            resources.ApplyResources(this.lblTimeRemaining, "lblTimeRemaining");
            this.lblTimeRemaining.Name = "lblTimeRemaining";
            // 
            // chkDLC
            // 
            resources.ApplyResources(this.chkDLC, "chkDLC");
            this.chkDLC.Name = "chkDLC";
            this.chkDLC.UseVisualStyleBackColor = true;
            // 
            // frmDownload
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblDownloadingMetadata);
            this.Controls.Add(this.chkDLC);
            this.Controls.Add(this.chkUpdate);
            this.Controls.Add(this.chkTitle);
            this.Controls.Add(this.btnDownload);
            this.Controls.Add(this.lblTimeRemaining);
            this.Controls.Add(this.lblAvgTransferRate);
            this.Controls.Add(this.lblTransferRate);
            this.Controls.Add(this.lblCurrentFile);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.progMain);
            this.Icon = global::uTikDownloadHelper.Properties.Resources.AppIcon;
            this.Name = "frmDownload";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmDownload_FormClosing);
            this.Load += new System.EventHandler(this.frmDownload_Load);
            this.Shown += new System.EventHandler(this.frmDownload_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progMain;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblCurrentFile;
        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.FolderBrowserDialog fbd;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblTransferRate;
        private System.Windows.Forms.CheckBox chkTitle;
        private System.Windows.Forms.CheckBox chkUpdate;
        private System.Windows.Forms.Timer progressTimer;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblAvgTransferRate;
        private System.Windows.Forms.FolderBrowserDialog folderDialog;
        private System.Windows.Forms.Label lblDownloadingMetadata;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblTimeRemaining;
        private System.Windows.Forms.CheckBox chkDLC;
    }
}