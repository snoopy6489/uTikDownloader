namespace uTikDownloadHelper
{
    partial class frmList
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmList));
            this.lstMain = new System.Windows.Forms.ListView();
            this.titleID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.dlc = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.name = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.region = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.size = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.comboRegion = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnOpen = new System.Windows.Forms.Button();
            this.btnDownload = new System.Windows.Forms.Button();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.lblLoading = new System.Windows.Forms.Label();
            this.btnTitleKeyCheck = new System.Windows.Forms.Button();
            this.ofdTik = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // lstMain
            // 
            resources.ApplyResources(this.lstMain, "lstMain");
            this.lstMain.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.titleID,
            this.dlc,
            this.name,
            this.region,
            this.size});
            this.lstMain.FullRowSelect = true;
            this.lstMain.GridLines = true;
            this.lstMain.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lstMain.Name = "lstMain";
            this.lstMain.UseCompatibleStateImageBehavior = false;
            this.lstMain.View = System.Windows.Forms.View.Details;
            this.lstMain.SelectedIndexChanged += new System.EventHandler(this.lstMain_SelectedIndexChanged);
            this.lstMain.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lstMain_MouseDoubleClick);
            // 
            // titleID
            // 
            resources.ApplyResources(this.titleID, "titleID");
            // 
            // dlc
            // 
            resources.ApplyResources(this.dlc, "dlc");
            // 
            // name
            // 
            resources.ApplyResources(this.name, "name");
            // 
            // region
            // 
            resources.ApplyResources(this.region, "region");
            // 
            // size
            // 
            resources.ApplyResources(this.size, "size");
            // 
            // comboRegion
            // 
            resources.ApplyResources(this.comboRegion, "comboRegion");
            this.comboRegion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboRegion.FormattingEnabled = true;
            this.comboRegion.Name = "comboRegion";
            this.comboRegion.SelectedIndexChanged += new System.EventHandler(this.comboRegion_SelectedIndexChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // btnOpen
            // 
            resources.ApplyResources(this.btnOpen, "btnOpen");
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnDownload
            // 
            resources.ApplyResources(this.btnDownload, "btnDownload");
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.UseVisualStyleBackColor = true;
            this.btnDownload.Click += new System.EventHandler(this.handleDownload);
            // 
            // txtSearch
            // 
            resources.ApplyResources(this.txtSearch, "txtSearch");
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // lblLoading
            // 
            resources.ApplyResources(this.lblLoading, "lblLoading");
            this.lblLoading.BackColor = System.Drawing.Color.Transparent;
            this.lblLoading.Name = "lblLoading";
            // 
            // btnTitleKeyCheck
            // 
            resources.ApplyResources(this.btnTitleKeyCheck, "btnTitleKeyCheck");
            this.btnTitleKeyCheck.Name = "btnTitleKeyCheck";
            this.btnTitleKeyCheck.UseVisualStyleBackColor = true;
            this.btnTitleKeyCheck.Click += new System.EventHandler(this.btnTitleKeyCheck_Click);
            // 
            // ofdTik
            // 
            resources.ApplyResources(this.ofdTik, "ofdTik");
            this.ofdTik.Multiselect = true;
            // 
            // frmList
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnTitleKeyCheck);
            this.Controls.Add(this.lblLoading);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.btnDownload);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.comboRegion);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lstMain);
            this.Icon = global::uTikDownloadHelper.Properties.Resources.AppIcon;
            this.Name = "frmList";
            this.Load += new System.EventHandler(this.frmList_Load);
            this.Shown += new System.EventHandler(this.frmList_Shown);
            this.SizeChanged += new System.EventHandler(this.frmList_SizeChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lstMain;
        private System.Windows.Forms.ColumnHeader name;
        private System.Windows.Forms.ColumnHeader titleID;
        private System.Windows.Forms.ColumnHeader region;
        private System.Windows.Forms.ComboBox comboRegion;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label lblLoading;
        private System.Windows.Forms.Button btnTitleKeyCheck;
        private System.Windows.Forms.ColumnHeader size;
        private System.Windows.Forms.OpenFileDialog ofdTik;
        private System.Windows.Forms.ColumnHeader dlc;
    }
}