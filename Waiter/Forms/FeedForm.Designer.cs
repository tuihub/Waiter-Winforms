namespace Waiter.Forms
{
    partial class FeedForm
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
            this.contentPanel = new System.Windows.Forms.Panel();
            this.mainSplit = new System.Windows.Forms.SplitContainer();
            this.lblConfigs = new System.Windows.Forms.Label();
            this._lstFeedConfigs = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.bottomSplit = new System.Windows.Forms.SplitContainer();
            this.lblItems = new System.Windows.Forms.Label();
            this._lstFeedItems = new System.Windows.Forms.ListView();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this._txtFeedContent = new System.Windows.Forms.RichTextBox();
            this.buttonPanel = new System.Windows.Forms.Panel();
            this._btnAddConfig = new System.Windows.Forms.Button();
            this._btnRefresh = new System.Windows.Forms.Button();
            this._btnClose = new System.Windows.Forms.Button();
            this._statusStrip = new System.Windows.Forms.StatusStrip();
            this._statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.contentPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainSplit)).BeginInit();
            this.mainSplit.Panel1.SuspendLayout();
            this.mainSplit.Panel2.SuspendLayout();
            this.mainSplit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bottomSplit)).BeginInit();
            this.bottomSplit.Panel1.SuspendLayout();
            this.bottomSplit.Panel2.SuspendLayout();
            this.bottomSplit.SuspendLayout();
            this.buttonPanel.SuspendLayout();
            this._statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // contentPanel
            // 
            this.contentPanel.Controls.Add(this.mainSplit);
            this.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentPanel.Location = new System.Drawing.Point(0, 0);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Padding = new System.Windows.Forms.Padding(10, 10, 10, 80);
            this.contentPanel.Size = new System.Drawing.Size(884, 539);
            this.contentPanel.TabIndex = 0;
            // 
            // mainSplit
            // 
            this.mainSplit.BackColor = System.Drawing.Color.FromArgb(27, 40, 56);
            this.mainSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainSplit.Location = new System.Drawing.Point(10, 10);
            this.mainSplit.Name = "mainSplit";
            this.mainSplit.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // mainSplit.Panel1
            // 
            this.mainSplit.Panel1.BackColor = System.Drawing.Color.FromArgb(27, 40, 56);
            this.mainSplit.Panel1.Controls.Add(this._lstFeedConfigs);
            this.mainSplit.Panel1.Controls.Add(this.lblConfigs);
            // 
            // mainSplit.Panel2
            // 
            this.mainSplit.Panel2.BackColor = System.Drawing.Color.FromArgb(27, 40, 56);
            this.mainSplit.Panel2.Controls.Add(this.bottomSplit);
            this.mainSplit.Size = new System.Drawing.Size(864, 449);
            this.mainSplit.SplitterDistance = 200;
            this.mainSplit.TabIndex = 0;
            // 
            // lblConfigs
            // 
            this.lblConfigs.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblConfigs.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblConfigs.ForeColor = System.Drawing.Color.White;
            this.lblConfigs.Location = new System.Drawing.Point(0, 0);
            this.lblConfigs.Name = "lblConfigs";
            this.lblConfigs.Size = new System.Drawing.Size(864, 30);
            this.lblConfigs.TabIndex = 0;
            this.lblConfigs.Text = "Feed Sources";
            // 
            // _lstFeedConfigs
            // 
            this._lstFeedConfigs.BackColor = System.Drawing.Color.FromArgb(45, 60, 80);
            this._lstFeedConfigs.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._lstFeedConfigs.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this._lstFeedConfigs.Dock = System.Windows.Forms.DockStyle.Fill;
            this._lstFeedConfigs.ForeColor = System.Drawing.Color.White;
            this._lstFeedConfigs.FullRowSelect = true;
            this._lstFeedConfigs.Location = new System.Drawing.Point(0, 30);
            this._lstFeedConfigs.Name = "_lstFeedConfigs";
            this._lstFeedConfigs.Size = new System.Drawing.Size(864, 170);
            this._lstFeedConfigs.TabIndex = 1;
            this._lstFeedConfigs.UseCompatibleStateImageBehavior = false;
            this._lstFeedConfigs.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 300;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Status";
            this.columnHeader2.Width = 100;
            // 
            // bottomSplit
            // 
            this.bottomSplit.BackColor = System.Drawing.Color.FromArgb(27, 40, 56);
            this.bottomSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bottomSplit.Location = new System.Drawing.Point(0, 0);
            this.bottomSplit.Name = "bottomSplit";
            this.bottomSplit.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // bottomSplit.Panel1
            // 
            this.bottomSplit.Panel1.BackColor = System.Drawing.Color.FromArgb(27, 40, 56);
            this.bottomSplit.Panel1.Controls.Add(this._lstFeedItems);
            this.bottomSplit.Panel1.Controls.Add(this.lblItems);
            // 
            // bottomSplit.Panel2
            // 
            this.bottomSplit.Panel2.BackColor = System.Drawing.Color.FromArgb(27, 40, 56);
            this.bottomSplit.Panel2.Controls.Add(this._txtFeedContent);
            this.bottomSplit.Size = new System.Drawing.Size(864, 245);
            this.bottomSplit.SplitterDistance = 150;
            this.bottomSplit.TabIndex = 0;
            // 
            // lblItems
            // 
            this.lblItems.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblItems.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblItems.ForeColor = System.Drawing.Color.White;
            this.lblItems.Location = new System.Drawing.Point(0, 0);
            this.lblItems.Name = "lblItems";
            this.lblItems.Size = new System.Drawing.Size(864, 30);
            this.lblItems.TabIndex = 0;
            this.lblItems.Text = "Feed Items";
            // 
            // _lstFeedItems
            // 
            this._lstFeedItems.BackColor = System.Drawing.Color.FromArgb(45, 60, 80);
            this._lstFeedItems.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._lstFeedItems.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4});
            this._lstFeedItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this._lstFeedItems.ForeColor = System.Drawing.Color.White;
            this._lstFeedItems.FullRowSelect = true;
            this._lstFeedItems.Location = new System.Drawing.Point(0, 30);
            this._lstFeedItems.Name = "_lstFeedItems";
            this._lstFeedItems.Size = new System.Drawing.Size(864, 120);
            this._lstFeedItems.TabIndex = 1;
            this._lstFeedItems.UseCompatibleStateImageBehavior = false;
            this._lstFeedItems.View = System.Windows.Forms.View.Details;
            this._lstFeedItems.SelectedIndexChanged += new System.EventHandler(this.LstFeedItems_SelectedIndexChanged);
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Title";
            this.columnHeader3.Width = 400;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Published";
            this.columnHeader4.Width = 150;
            // 
            // _txtFeedContent
            // 
            this._txtFeedContent.BackColor = System.Drawing.Color.FromArgb(45, 60, 80);
            this._txtFeedContent.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._txtFeedContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this._txtFeedContent.Font = new System.Drawing.Font("Segoe UI", 10F);
            this._txtFeedContent.ForeColor = System.Drawing.Color.White;
            this._txtFeedContent.Location = new System.Drawing.Point(0, 0);
            this._txtFeedContent.Name = "_txtFeedContent";
            this._txtFeedContent.ReadOnly = true;
            this._txtFeedContent.Size = new System.Drawing.Size(864, 91);
            this._txtFeedContent.TabIndex = 0;
            this._txtFeedContent.Text = "";
            // 
            // buttonPanel
            // 
            this.buttonPanel.BackColor = System.Drawing.Color.FromArgb(23, 29, 37);
            this.buttonPanel.Controls.Add(this._btnClose);
            this.buttonPanel.Controls.Add(this._btnRefresh);
            this.buttonPanel.Controls.Add(this._btnAddConfig);
            this.buttonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonPanel.Location = new System.Drawing.Point(0, 539);
            this.buttonPanel.Name = "buttonPanel";
            this.buttonPanel.Size = new System.Drawing.Size(884, 60);
            this.buttonPanel.TabIndex = 1;
            // 
            // _btnAddConfig
            // 
            this._btnAddConfig.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            this._btnAddConfig.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnAddConfig.ForeColor = System.Drawing.Color.White;
            this._btnAddConfig.Location = new System.Drawing.Point(20, 15);
            this._btnAddConfig.Name = "_btnAddConfig";
            this._btnAddConfig.Size = new System.Drawing.Size(100, 35);
            this._btnAddConfig.TabIndex = 0;
            this._btnAddConfig.Text = "Add Source";
            this._btnAddConfig.UseVisualStyleBackColor = false;
            this._btnAddConfig.Click += new System.EventHandler(this.BtnAddConfig_Click);
            // 
            // _btnRefresh
            // 
            this._btnRefresh.BackColor = System.Drawing.Color.FromArgb(60, 60, 60);
            this._btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnRefresh.ForeColor = System.Drawing.Color.White;
            this._btnRefresh.Location = new System.Drawing.Point(130, 15);
            this._btnRefresh.Name = "_btnRefresh";
            this._btnRefresh.Size = new System.Drawing.Size(100, 35);
            this._btnRefresh.TabIndex = 1;
            this._btnRefresh.Text = "Refresh";
            this._btnRefresh.UseVisualStyleBackColor = false;
            this._btnRefresh.Click += new System.EventHandler(this.BtnRefresh_Click);
            // 
            // _btnClose
            // 
            this._btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._btnClose.BackColor = System.Drawing.Color.FromArgb(60, 60, 60);
            this._btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnClose.ForeColor = System.Drawing.Color.White;
            this._btnClose.Location = new System.Drawing.Point(764, 15);
            this._btnClose.Name = "_btnClose";
            this._btnClose.Size = new System.Drawing.Size(100, 35);
            this._btnClose.TabIndex = 2;
            this._btnClose.Text = "Close";
            this._btnClose.UseVisualStyleBackColor = false;
            this._btnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // _statusStrip
            // 
            this._statusStrip.BackColor = System.Drawing.Color.FromArgb(23, 29, 37);
            this._statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._statusLabel});
            this._statusStrip.Location = new System.Drawing.Point(0, 599);
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
            this._statusLabel.Text = "Ready";
            this._statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FeedForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(27, 40, 56);
            this.ClientSize = new System.Drawing.Size(884, 621);
            this.Controls.Add(this.contentPanel);
            this.Controls.Add(this.buttonPanel);
            this.Controls.Add(this._statusStrip);
            this.MinimumSize = new System.Drawing.Size(700, 500);
            this.Name = "FeedForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Feed Manager";
            this.Load += new System.EventHandler(this.FeedForm_Load);
            this.contentPanel.ResumeLayout(false);
            this.mainSplit.Panel1.ResumeLayout(false);
            this.mainSplit.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainSplit)).EndInit();
            this.mainSplit.ResumeLayout(false);
            this.bottomSplit.Panel1.ResumeLayout(false);
            this.bottomSplit.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.bottomSplit)).EndInit();
            this.bottomSplit.ResumeLayout(false);
            this.buttonPanel.ResumeLayout(false);
            this._statusStrip.ResumeLayout(false);
            this._statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Panel contentPanel;
        private System.Windows.Forms.SplitContainer mainSplit;
        private System.Windows.Forms.Label lblConfigs;
        private System.Windows.Forms.ListView _lstFeedConfigs;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.SplitContainer bottomSplit;
        private System.Windows.Forms.Label lblItems;
        private System.Windows.Forms.ListView _lstFeedItems;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.RichTextBox _txtFeedContent;
        private System.Windows.Forms.Panel buttonPanel;
        private System.Windows.Forms.Button _btnAddConfig;
        private System.Windows.Forms.Button _btnRefresh;
        private System.Windows.Forms.Button _btnClose;
        private System.Windows.Forms.StatusStrip _statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel _statusLabel;
    }
}
