namespace Waiter.Forms
{
    partial class MainForm
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
            this._menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.storageCapacityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshAppsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backgroundTasksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.feedManagerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.notificationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.appsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.storeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.categoriesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.addAppToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.accountToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.profileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.loginToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._mainSplitContainer = new System.Windows.Forms.SplitContainer();
            this.headerPanel = new System.Windows.Forms.Panel();
            this.lblLibrary = new System.Windows.Forms.Label();
            this._appListView = new System.Windows.Forms.ListView();
            this._detailPanel = new System.Windows.Forms.Panel();
            this._appCoverImage = new System.Windows.Forms.PictureBox();
            this._lblAppName = new System.Windows.Forms.Label();
            this._lblAppDescription = new System.Windows.Forms.Label();
            this._btnLaunch = new System.Windows.Forms.Button();
            this._btnDownload = new System.Windows.Forms.Button();
            this._btnSyncSave = new System.Windows.Forms.Button();
            this._statusStrip = new System.Windows.Forms.StatusStrip();
            this._statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this._taskLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this._userLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this._menuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._mainSplitContainer)).BeginInit();
            this._mainSplitContainer.Panel1.SuspendLayout();
            this._mainSplitContainer.Panel2.SuspendLayout();
            this._mainSplitContainer.SuspendLayout();
            this.headerPanel.SuspendLayout();
            this._detailPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._appCoverImage)).BeginInit();
            this._statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // _menuStrip
            // 
            this._menuStrip.BackColor = System.Drawing.Color.FromArgb(23, 29, 37);
            this._menuStrip.ForeColor = System.Drawing.Color.White;
            this._menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.appsToolStripMenuItem,
            this.accountToolStripMenuItem,
            this.helpToolStripMenuItem});
            this._menuStrip.Location = new System.Drawing.Point(0, 0);
            this._menuStrip.Name = "_menuStrip";
            this._menuStrip.Size = new System.Drawing.Size(1184, 24);
            this._menuStrip.TabIndex = 0;
            this._menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem,
            this.storageCapacityToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.settingsToolStripMenuItem.Text = "Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.SettingsToolStripMenuItem_Click);
            // 
            // storageCapacityToolStripMenuItem
            // 
            this.storageCapacityToolStripMenuItem.Name = "storageCapacityToolStripMenuItem";
            this.storageCapacityToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.storageCapacityToolStripMenuItem.Text = "Storage Capacity";
            this.storageCapacityToolStripMenuItem.Click += new System.EventHandler(this.StorageCapacityToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(177, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshAppsToolStripMenuItem,
            this.backgroundTasksToolStripMenuItem,
            this.toolStripSeparator2,
            this.feedManagerToolStripMenuItem,
            this.notificationsToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // refreshAppsToolStripMenuItem
            // 
            this.refreshAppsToolStripMenuItem.Name = "refreshAppsToolStripMenuItem";
            this.refreshAppsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.refreshAppsToolStripMenuItem.Text = "Refresh Apps";
            this.refreshAppsToolStripMenuItem.Click += new System.EventHandler(this.RefreshAppsToolStripMenuItem_Click);
            // 
            // backgroundTasksToolStripMenuItem
            // 
            this.backgroundTasksToolStripMenuItem.Name = "backgroundTasksToolStripMenuItem";
            this.backgroundTasksToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.backgroundTasksToolStripMenuItem.Text = "Background Tasks";
            this.backgroundTasksToolStripMenuItem.Click += new System.EventHandler(this.BackgroundTasksToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(177, 6);
            // 
            // feedManagerToolStripMenuItem
            // 
            this.feedManagerToolStripMenuItem.Name = "feedManagerToolStripMenuItem";
            this.feedManagerToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.feedManagerToolStripMenuItem.Text = "Feed Manager";
            this.feedManagerToolStripMenuItem.Click += new System.EventHandler(this.FeedManagerToolStripMenuItem_Click);
            // 
            // notificationsToolStripMenuItem
            // 
            this.notificationsToolStripMenuItem.Name = "notificationsToolStripMenuItem";
            this.notificationsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.notificationsToolStripMenuItem.Text = "Notifications";
            this.notificationsToolStripMenuItem.Click += new System.EventHandler(this.NotificationsToolStripMenuItem_Click);
            // 
            // appsToolStripMenuItem
            // 
            this.appsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.storeToolStripMenuItem,
            this.categoriesToolStripMenuItem,
            this.toolStripSeparator3,
            this.addAppToolStripMenuItem});
            this.appsToolStripMenuItem.Name = "appsToolStripMenuItem";
            this.appsToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.appsToolStripMenuItem.Text = "Apps";
            // 
            // storeToolStripMenuItem
            // 
            this.storeToolStripMenuItem.Name = "storeToolStripMenuItem";
            this.storeToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.storeToolStripMenuItem.Text = "Store";
            this.storeToolStripMenuItem.Click += new System.EventHandler(this.StoreToolStripMenuItem_Click);
            // 
            // categoriesToolStripMenuItem
            // 
            this.categoriesToolStripMenuItem.Name = "categoriesToolStripMenuItem";
            this.categoriesToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.categoriesToolStripMenuItem.Text = "Categories";
            this.categoriesToolStripMenuItem.Click += new System.EventHandler(this.CategoriesToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(177, 6);
            // 
            // addAppToolStripMenuItem
            // 
            this.addAppToolStripMenuItem.Name = "addAppToolStripMenuItem";
            this.addAppToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.addAppToolStripMenuItem.Text = "Add App...";
            this.addAppToolStripMenuItem.Click += new System.EventHandler(this.AddAppToolStripMenuItem_Click);
            // 
            // accountToolStripMenuItem
            // 
            this.accountToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.profileToolStripMenuItem,
            this.toolStripSeparator4,
            this.loginToolStripMenuItem,
            this.logoutToolStripMenuItem});
            this.accountToolStripMenuItem.Name = "accountToolStripMenuItem";
            this.accountToolStripMenuItem.Size = new System.Drawing.Size(64, 20);
            this.accountToolStripMenuItem.Text = "Account";
            // 
            // profileToolStripMenuItem
            // 
            this.profileToolStripMenuItem.Name = "profileToolStripMenuItem";
            this.profileToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.profileToolStripMenuItem.Text = "Profile";
            this.profileToolStripMenuItem.Click += new System.EventHandler(this.ProfileToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(177, 6);
            // 
            // loginToolStripMenuItem
            // 
            this.loginToolStripMenuItem.Name = "loginToolStripMenuItem";
            this.loginToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.loginToolStripMenuItem.Text = "Login";
            this.loginToolStripMenuItem.Click += new System.EventHandler(this.LoginToolStripMenuItem_Click);
            // 
            // logoutToolStripMenuItem
            // 
            this.logoutToolStripMenuItem.Name = "logoutToolStripMenuItem";
            this.logoutToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.logoutToolStripMenuItem.Text = "Logout";
            this.logoutToolStripMenuItem.Click += new System.EventHandler(this.LogoutToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.AboutToolStripMenuItem_Click);
            // 
            // _mainSplitContainer
            // 
            this._mainSplitContainer.BackColor = System.Drawing.Color.FromArgb(27, 40, 56);
            this._mainSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this._mainSplitContainer.Location = new System.Drawing.Point(0, 24);
            this._mainSplitContainer.Name = "_mainSplitContainer";
            // 
            // _mainSplitContainer.Panel1
            // 
            this._mainSplitContainer.Panel1.BackColor = System.Drawing.Color.FromArgb(27, 40, 56);
            this._mainSplitContainer.Panel1.Controls.Add(this._appListView);
            this._mainSplitContainer.Panel1.Controls.Add(this.headerPanel);
            this._mainSplitContainer.Panel1MinSize = 200;
            // 
            // _mainSplitContainer.Panel2
            // 
            this._mainSplitContainer.Panel2.BackColor = System.Drawing.Color.FromArgb(27, 40, 56);
            this._mainSplitContainer.Panel2.Controls.Add(this._detailPanel);
            this._mainSplitContainer.Panel2MinSize = 400;
            this._mainSplitContainer.Size = new System.Drawing.Size(1184, 615);
            this._mainSplitContainer.SplitterDistance = 300;
            this._mainSplitContainer.TabIndex = 1;
            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.Color.FromArgb(23, 29, 37);
            this.headerPanel.Controls.Add(this.lblLibrary);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(300, 50);
            this.headerPanel.TabIndex = 0;
            // 
            // lblLibrary
            // 
            this.lblLibrary.AutoSize = true;
            this.lblLibrary.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblLibrary.ForeColor = System.Drawing.Color.White;
            this.lblLibrary.Location = new System.Drawing.Point(15, 15);
            this.lblLibrary.Name = "lblLibrary";
            this.lblLibrary.Size = new System.Drawing.Size(70, 21);
            this.lblLibrary.TabIndex = 0;
            this.lblLibrary.Text = "LIBRARY";
            // 
            // _appListView
            // 
            this._appListView.BackColor = System.Drawing.Color.FromArgb(27, 40, 56);
            this._appListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._appListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._appListView.ForeColor = System.Drawing.Color.White;
            this._appListView.Location = new System.Drawing.Point(0, 50);
            this._appListView.Name = "_appListView";
            this._appListView.Size = new System.Drawing.Size(300, 565);
            this._appListView.TabIndex = 1;
            this._appListView.UseCompatibleStateImageBehavior = false;
            this._appListView.View = System.Windows.Forms.View.LargeIcon;
            this._appListView.SelectedIndexChanged += new System.EventHandler(this.AppListView_SelectedIndexChanged);
            this._appListView.DoubleClick += new System.EventHandler(this.AppListView_DoubleClick);
            // 
            // _detailPanel
            // 
            this._detailPanel.BackColor = System.Drawing.Color.FromArgb(27, 40, 56);
            this._detailPanel.Controls.Add(this._btnSyncSave);
            this._detailPanel.Controls.Add(this._btnDownload);
            this._detailPanel.Controls.Add(this._btnLaunch);
            this._detailPanel.Controls.Add(this._lblAppDescription);
            this._detailPanel.Controls.Add(this._lblAppName);
            this._detailPanel.Controls.Add(this._appCoverImage);
            this._detailPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._detailPanel.Location = new System.Drawing.Point(0, 0);
            this._detailPanel.Name = "_detailPanel";
            this._detailPanel.Size = new System.Drawing.Size(880, 615);
            this._detailPanel.TabIndex = 0;
            // 
            // _appCoverImage
            // 
            this._appCoverImage.BackColor = System.Drawing.Color.FromArgb(45, 60, 80);
            this._appCoverImage.Location = new System.Drawing.Point(20, 20);
            this._appCoverImage.Name = "_appCoverImage";
            this._appCoverImage.Size = new System.Drawing.Size(200, 280);
            this._appCoverImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this._appCoverImage.TabIndex = 0;
            this._appCoverImage.TabStop = false;
            // 
            // _lblAppName
            // 
            this._lblAppName.AutoEllipsis = true;
            this._lblAppName.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this._lblAppName.ForeColor = System.Drawing.Color.White;
            this._lblAppName.Location = new System.Drawing.Point(240, 20);
            this._lblAppName.Name = "_lblAppName";
            this._lblAppName.Size = new System.Drawing.Size(500, 40);
            this._lblAppName.TabIndex = 1;
            this._lblAppName.Text = "Select an app";
            // 
            // _lblAppDescription
            // 
            this._lblAppDescription.Font = new System.Drawing.Font("Segoe UI", 10F);
            this._lblAppDescription.ForeColor = System.Drawing.Color.LightGray;
            this._lblAppDescription.Location = new System.Drawing.Point(240, 70);
            this._lblAppDescription.Name = "_lblAppDescription";
            this._lblAppDescription.Size = new System.Drawing.Size(500, 100);
            this._lblAppDescription.TabIndex = 2;
            this._lblAppDescription.Text = "Select an app from your library to see details";
            // 
            // _btnLaunch
            // 
            this._btnLaunch.BackColor = System.Drawing.Color.FromArgb(76, 175, 80);
            this._btnLaunch.Enabled = false;
            this._btnLaunch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnLaunch.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this._btnLaunch.ForeColor = System.Drawing.Color.White;
            this._btnLaunch.Location = new System.Drawing.Point(240, 180);
            this._btnLaunch.Name = "_btnLaunch";
            this._btnLaunch.Size = new System.Drawing.Size(120, 40);
            this._btnLaunch.TabIndex = 3;
            this._btnLaunch.Text = "LAUNCH";
            this._btnLaunch.UseVisualStyleBackColor = false;
            this._btnLaunch.Click += new System.EventHandler(this.BtnLaunch_Click);
            // 
            // _btnDownload
            // 
            this._btnDownload.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            this._btnDownload.Enabled = false;
            this._btnDownload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnDownload.ForeColor = System.Drawing.Color.White;
            this._btnDownload.Location = new System.Drawing.Point(370, 180);
            this._btnDownload.Name = "_btnDownload";
            this._btnDownload.Size = new System.Drawing.Size(100, 40);
            this._btnDownload.TabIndex = 4;
            this._btnDownload.Text = "Download";
            this._btnDownload.UseVisualStyleBackColor = false;
            this._btnDownload.Click += new System.EventHandler(this.BtnDownload_Click);
            // 
            // _btnSyncSave
            // 
            this._btnSyncSave.BackColor = System.Drawing.Color.FromArgb(60, 60, 60);
            this._btnSyncSave.Enabled = false;
            this._btnSyncSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnSyncSave.ForeColor = System.Drawing.Color.White;
            this._btnSyncSave.Location = new System.Drawing.Point(480, 180);
            this._btnSyncSave.Name = "_btnSyncSave";
            this._btnSyncSave.Size = new System.Drawing.Size(100, 40);
            this._btnSyncSave.TabIndex = 5;
            this._btnSyncSave.Text = "Sync Save";
            this._btnSyncSave.UseVisualStyleBackColor = false;
            this._btnSyncSave.Click += new System.EventHandler(this.BtnSyncSave_Click);
            // 
            // _statusStrip
            // 
            this._statusStrip.BackColor = System.Drawing.Color.FromArgb(23, 29, 37);
            this._statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._statusLabel,
            this._taskLabel,
            this._userLabel});
            this._statusStrip.Location = new System.Drawing.Point(0, 639);
            this._statusStrip.Name = "_statusStrip";
            this._statusStrip.Size = new System.Drawing.Size(1184, 22);
            this._statusStrip.TabIndex = 2;
            // 
            // _statusLabel
            // 
            this._statusLabel.ForeColor = System.Drawing.Color.LightGray;
            this._statusLabel.Name = "_statusLabel";
            this._statusLabel.Size = new System.Drawing.Size(1035, 17);
            this._statusLabel.Spring = true;
            this._statusLabel.Text = "Ready";
            this._statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _taskLabel
            // 
            this._taskLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this._taskLabel.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this._taskLabel.ForeColor = System.Drawing.Color.LightGray;
            this._taskLabel.Name = "_taskLabel";
            this._taskLabel.Size = new System.Drawing.Size(53, 17);
            this._taskLabel.Text = "Tasks: 0";
            // 
            // _userLabel
            // 
            this._userLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this._userLabel.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this._userLabel.ForeColor = System.Drawing.Color.LightGray;
            this._userLabel.Name = "_userLabel";
            this._userLabel.Size = new System.Drawing.Size(81, 17);
            this._userLabel.Text = "Not logged in";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(27, 40, 56);
            this.ClientSize = new System.Drawing.Size(1184, 661);
            this.Controls.Add(this._mainSplitContainer);
            this.Controls.Add(this._menuStrip);
            this.Controls.Add(this._statusStrip);
            this.MainMenuStrip = this._menuStrip;
            this.MinimumSize = new System.Drawing.Size(800, 500);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TuiHub Waiter";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this._menuStrip.ResumeLayout(false);
            this._menuStrip.PerformLayout();
            this._mainSplitContainer.Panel1.ResumeLayout(false);
            this._mainSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._mainSplitContainer)).EndInit();
            this._mainSplitContainer.ResumeLayout(false);
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            this._detailPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._appCoverImage)).EndInit();
            this._statusStrip.ResumeLayout(false);
            this._statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.MenuStrip _menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem storageCapacityToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem refreshAppsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem backgroundTasksToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem feedManagerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem notificationsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem appsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem storeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem categoriesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem addAppToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem accountToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem profileToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem loginToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logoutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.SplitContainer _mainSplitContainer;
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Label lblLibrary;
        private System.Windows.Forms.ListView _appListView;
        private System.Windows.Forms.Panel _detailPanel;
        private System.Windows.Forms.PictureBox _appCoverImage;
        private System.Windows.Forms.Label _lblAppName;
        private System.Windows.Forms.Label _lblAppDescription;
        private System.Windows.Forms.Button _btnLaunch;
        private System.Windows.Forms.Button _btnDownload;
        private System.Windows.Forms.Button _btnSyncSave;
        private System.Windows.Forms.StatusStrip _statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel _statusLabel;
        private System.Windows.Forms.ToolStripStatusLabel _taskLabel;
        private System.Windows.Forms.ToolStripStatusLabel _userLabel;
    }
}
