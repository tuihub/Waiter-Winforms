namespace Waiter.Forms
{
    partial class StoreForm
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
            this.searchPanel = new System.Windows.Forms.Panel();
            this.lblSearch = new System.Windows.Forms.Label();
            this._txtSearch = new System.Windows.Forms.TextBox();
            this._btnSearch = new System.Windows.Forms.Button();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this._lstStoreApps = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this._detailPanel = new System.Windows.Forms.Panel();
            this._lblAppName = new System.Windows.Forms.Label();
            this._lblAppDescription = new System.Windows.Forms.Label();
            this._btnAcquire = new System.Windows.Forms.Button();
            this._btnClose = new System.Windows.Forms.Button();
            this._statusStrip = new System.Windows.Forms.StatusStrip();
            this._statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.searchPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this._detailPanel.SuspendLayout();
            this._statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // searchPanel
            // 
            this.searchPanel.BackColor = System.Drawing.Color.FromArgb(23, 29, 37);
            this.searchPanel.Controls.Add(this._btnSearch);
            this.searchPanel.Controls.Add(this._txtSearch);
            this.searchPanel.Controls.Add(this.lblSearch);
            this.searchPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.searchPanel.Location = new System.Drawing.Point(0, 0);
            this.searchPanel.Name = "searchPanel";
            this.searchPanel.Padding = new System.Windows.Forms.Padding(10);
            this.searchPanel.Size = new System.Drawing.Size(884, 60);
            this.searchPanel.TabIndex = 0;
            // 
            // lblSearch
            // 
            this.lblSearch.AutoSize = true;
            this.lblSearch.ForeColor = System.Drawing.Color.White;
            this.lblSearch.Location = new System.Drawing.Point(10, 18);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(75, 15);
            this.lblSearch.TabIndex = 0;
            this.lblSearch.Text = "Search Store:";
            // 
            // _txtSearch
            // 
            this._txtSearch.BackColor = System.Drawing.Color.FromArgb(45, 60, 80);
            this._txtSearch.ForeColor = System.Drawing.Color.White;
            this._txtSearch.Location = new System.Drawing.Point(100, 15);
            this._txtSearch.Name = "_txtSearch";
            this._txtSearch.Size = new System.Drawing.Size(300, 23);
            this._txtSearch.TabIndex = 1;
            this._txtSearch.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtSearch_KeyPress);
            // 
            // _btnSearch
            // 
            this._btnSearch.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            this._btnSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnSearch.ForeColor = System.Drawing.Color.White;
            this._btnSearch.Location = new System.Drawing.Point(410, 13);
            this._btnSearch.Name = "_btnSearch";
            this._btnSearch.Size = new System.Drawing.Size(80, 28);
            this._btnSearch.TabIndex = 2;
            this._btnSearch.Text = "Search";
            this._btnSearch.UseVisualStyleBackColor = false;
            this._btnSearch.Click += new System.EventHandler(this.BtnSearch_Click);
            // 
            // splitContainer
            // 
            this.splitContainer.BackColor = System.Drawing.Color.FromArgb(27, 40, 56);
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 60);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.BackColor = System.Drawing.Color.FromArgb(27, 40, 56);
            this.splitContainer.Panel1.Controls.Add(this._lstStoreApps);
            this.splitContainer.Panel1MinSize = 200;
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.BackColor = System.Drawing.Color.FromArgb(27, 40, 56);
            this.splitContainer.Panel2.Controls.Add(this._detailPanel);
            this.splitContainer.Panel2MinSize = 250;
            this.splitContainer.Size = new System.Drawing.Size(884, 467);
            this.splitContainer.SplitterDistance = 400;
            this.splitContainer.TabIndex = 1;
            // 
            // _lstStoreApps
            // 
            this._lstStoreApps.BackColor = System.Drawing.Color.FromArgb(27, 40, 56);
            this._lstStoreApps.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._lstStoreApps.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this._lstStoreApps.Dock = System.Windows.Forms.DockStyle.Fill;
            this._lstStoreApps.ForeColor = System.Drawing.Color.White;
            this._lstStoreApps.FullRowSelect = true;
            this._lstStoreApps.Location = new System.Drawing.Point(0, 0);
            this._lstStoreApps.Name = "_lstStoreApps";
            this._lstStoreApps.Size = new System.Drawing.Size(400, 467);
            this._lstStoreApps.TabIndex = 0;
            this._lstStoreApps.UseCompatibleStateImageBehavior = false;
            this._lstStoreApps.View = System.Windows.Forms.View.Details;
            this._lstStoreApps.SelectedIndexChanged += new System.EventHandler(this.LstStoreApps_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "App Name";
            this.columnHeader1.Width = 350;
            // 
            // _detailPanel
            // 
            this._detailPanel.BackColor = System.Drawing.Color.FromArgb(27, 40, 56);
            this._detailPanel.Controls.Add(this._btnClose);
            this._detailPanel.Controls.Add(this._btnAcquire);
            this._detailPanel.Controls.Add(this._lblAppDescription);
            this._detailPanel.Controls.Add(this._lblAppName);
            this._detailPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._detailPanel.Location = new System.Drawing.Point(0, 0);
            this._detailPanel.Name = "_detailPanel";
            this._detailPanel.Padding = new System.Windows.Forms.Padding(20);
            this._detailPanel.Size = new System.Drawing.Size(480, 467);
            this._detailPanel.TabIndex = 0;
            // 
            // _lblAppName
            // 
            this._lblAppName.AutoEllipsis = true;
            this._lblAppName.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this._lblAppName.ForeColor = System.Drawing.Color.White;
            this._lblAppName.Location = new System.Drawing.Point(20, 20);
            this._lblAppName.Name = "_lblAppName";
            this._lblAppName.Size = new System.Drawing.Size(350, 30);
            this._lblAppName.TabIndex = 0;
            this._lblAppName.Text = "Select an app";
            // 
            // _lblAppDescription
            // 
            this._lblAppDescription.Font = new System.Drawing.Font("Segoe UI", 10F);
            this._lblAppDescription.ForeColor = System.Drawing.Color.LightGray;
            this._lblAppDescription.Location = new System.Drawing.Point(20, 60);
            this._lblAppDescription.Name = "_lblAppDescription";
            this._lblAppDescription.Size = new System.Drawing.Size(350, 120);
            this._lblAppDescription.TabIndex = 1;
            this._lblAppDescription.Text = "Select an app from the store to see details and acquire it.";
            // 
            // _btnAcquire
            // 
            this._btnAcquire.BackColor = System.Drawing.Color.FromArgb(76, 175, 80);
            this._btnAcquire.Enabled = false;
            this._btnAcquire.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnAcquire.ForeColor = System.Drawing.Color.White;
            this._btnAcquire.Location = new System.Drawing.Point(20, 200);
            this._btnAcquire.Name = "_btnAcquire";
            this._btnAcquire.Size = new System.Drawing.Size(130, 35);
            this._btnAcquire.TabIndex = 2;
            this._btnAcquire.Text = "Acquire App";
            this._btnAcquire.UseVisualStyleBackColor = false;
            this._btnAcquire.Click += new System.EventHandler(this.BtnAcquire_Click);
            // 
            // _btnClose
            // 
            this._btnClose.BackColor = System.Drawing.Color.FromArgb(60, 60, 60);
            this._btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnClose.ForeColor = System.Drawing.Color.White;
            this._btnClose.Location = new System.Drawing.Point(160, 200);
            this._btnClose.Name = "_btnClose";
            this._btnClose.Size = new System.Drawing.Size(100, 35);
            this._btnClose.TabIndex = 3;
            this._btnClose.Text = "Close";
            this._btnClose.UseVisualStyleBackColor = false;
            this._btnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // _statusStrip
            // 
            this._statusStrip.BackColor = System.Drawing.Color.FromArgb(23, 29, 37);
            this._statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._statusLabel});
            this._statusStrip.Location = new System.Drawing.Point(0, 527);
            this._statusStrip.Name = "_statusStrip";
            this._statusStrip.Size = new System.Drawing.Size(884, 22);
            this._statusStrip.TabIndex = 2;
            // 
            // _statusLabel
            // 
            this._statusLabel.ForeColor = System.Drawing.Color.LightGray;
            this._statusLabel.Name = "_statusLabel";
            this._statusLabel.Size = new System.Drawing.Size(869, 17);
            this._statusLabel.Spring = true;
            this._statusLabel.Text = "Enter a search term to find apps in the store";
            this._statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // StoreForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(27, 40, 56);
            this.ClientSize = new System.Drawing.Size(884, 549);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.searchPanel);
            this.Controls.Add(this._statusStrip);
            this.MinimumSize = new System.Drawing.Size(700, 450);
            this.Name = "StoreForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "TuiHub Store";
            this.searchPanel.ResumeLayout(false);
            this.searchPanel.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this._detailPanel.ResumeLayout(false);
            this._statusStrip.ResumeLayout(false);
            this._statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Panel searchPanel;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.TextBox _txtSearch;
        private System.Windows.Forms.Button _btnSearch;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.ListView _lstStoreApps;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Panel _detailPanel;
        private System.Windows.Forms.Label _lblAppName;
        private System.Windows.Forms.Label _lblAppDescription;
        private System.Windows.Forms.Button _btnAcquire;
        private System.Windows.Forms.Button _btnClose;
        private System.Windows.Forms.StatusStrip _statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel _statusLabel;
    }
}
