namespace Waiter.Forms
{
    partial class NotificationForm
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
            this.tabPanel = new System.Windows.Forms.Panel();
            this._tabControl = new System.Windows.Forms.TabControl();
            this.notificationsTab = new System.Windows.Forms.TabPage();
            this.lblNotificationsTitle = new System.Windows.Forms.Label();
            this._lstNotifications = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this._btnMarkRead = new System.Windows.Forms.Button();
            this.targetsTab = new System.Windows.Forms.TabPage();
            this.lblTargetsTitle = new System.Windows.Forms.Label();
            this._lstTargets = new System.Windows.Forms.ListView();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
            this._btnAddTarget = new System.Windows.Forms.Button();
            this.flowsTab = new System.Windows.Forms.TabPage();
            this.lblFlowsTitle = new System.Windows.Forms.Label();
            this._lstFlows = new System.Windows.Forms.ListView();
            this.columnHeader7 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader8 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader9 = new System.Windows.Forms.ColumnHeader();
            this._btnAddFlow = new System.Windows.Forms.Button();
            this.buttonPanel = new System.Windows.Forms.Panel();
            this._btnRefresh = new System.Windows.Forms.Button();
            this._btnClose = new System.Windows.Forms.Button();
            this._statusStrip = new System.Windows.Forms.StatusStrip();
            this._statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabPanel.SuspendLayout();
            this._tabControl.SuspendLayout();
            this.notificationsTab.SuspendLayout();
            this.targetsTab.SuspendLayout();
            this.flowsTab.SuspendLayout();
            this.buttonPanel.SuspendLayout();
            this._statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabPanel
            // 
            this.tabPanel.Controls.Add(this._tabControl);
            this.tabPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabPanel.Location = new System.Drawing.Point(0, 0);
            this.tabPanel.Name = "tabPanel";
            this.tabPanel.Padding = new System.Windows.Forms.Padding(10, 10, 10, 80);
            this.tabPanel.Size = new System.Drawing.Size(784, 489);
            this.tabPanel.TabIndex = 0;
            // 
            // _tabControl
            // 
            this._tabControl.Controls.Add(this.notificationsTab);
            this._tabControl.Controls.Add(this.targetsTab);
            this._tabControl.Controls.Add(this.flowsTab);
            this._tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tabControl.Location = new System.Drawing.Point(10, 10);
            this._tabControl.Name = "_tabControl";
            this._tabControl.SelectedIndex = 0;
            this._tabControl.Size = new System.Drawing.Size(764, 399);
            this._tabControl.TabIndex = 0;
            // 
            // notificationsTab
            // 
            this.notificationsTab.BackColor = System.Drawing.Color.FromArgb(27, 40, 56);
            this.notificationsTab.Controls.Add(this._btnMarkRead);
            this.notificationsTab.Controls.Add(this._lstNotifications);
            this.notificationsTab.Controls.Add(this.lblNotificationsTitle);
            this.notificationsTab.Location = new System.Drawing.Point(4, 24);
            this.notificationsTab.Name = "notificationsTab";
            this.notificationsTab.Padding = new System.Windows.Forms.Padding(3);
            this.notificationsTab.Size = new System.Drawing.Size(756, 371);
            this.notificationsTab.TabIndex = 0;
            this.notificationsTab.Text = "System Notifications";
            // 
            // lblNotificationsTitle
            // 
            this.lblNotificationsTitle.AutoSize = true;
            this.lblNotificationsTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblNotificationsTitle.ForeColor = System.Drawing.Color.White;
            this.lblNotificationsTitle.Location = new System.Drawing.Point(20, 10);
            this.lblNotificationsTitle.Name = "lblNotificationsTitle";
            this.lblNotificationsTitle.Size = new System.Drawing.Size(172, 21);
            this.lblNotificationsTitle.TabIndex = 0;
            this.lblNotificationsTitle.Text = "System Notifications";
            // 
            // _lstNotifications
            // 
            this._lstNotifications.BackColor = System.Drawing.Color.FromArgb(45, 60, 80);
            this._lstNotifications.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._lstNotifications.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this._lstNotifications.ForeColor = System.Drawing.Color.White;
            this._lstNotifications.FullRowSelect = true;
            this._lstNotifications.Location = new System.Drawing.Point(20, 45);
            this._lstNotifications.Name = "_lstNotifications";
            this._lstNotifications.Size = new System.Drawing.Size(600, 320);
            this._lstNotifications.TabIndex = 1;
            this._lstNotifications.UseCompatibleStateImageBehavior = false;
            this._lstNotifications.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Title";
            this.columnHeader1.Width = 200;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Content";
            this.columnHeader2.Width = 250;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Status";
            this.columnHeader3.Width = 100;
            // 
            // _btnMarkRead
            // 
            this._btnMarkRead.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            this._btnMarkRead.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnMarkRead.ForeColor = System.Drawing.Color.White;
            this._btnMarkRead.Location = new System.Drawing.Point(630, 45);
            this._btnMarkRead.Name = "_btnMarkRead";
            this._btnMarkRead.Size = new System.Drawing.Size(100, 30);
            this._btnMarkRead.TabIndex = 2;
            this._btnMarkRead.Text = "Mark Read";
            this._btnMarkRead.UseVisualStyleBackColor = false;
            this._btnMarkRead.Click += new System.EventHandler(this.BtnMarkRead_Click);
            // 
            // targetsTab
            // 
            this.targetsTab.BackColor = System.Drawing.Color.FromArgb(27, 40, 56);
            this.targetsTab.Controls.Add(this._btnAddTarget);
            this.targetsTab.Controls.Add(this._lstTargets);
            this.targetsTab.Controls.Add(this.lblTargetsTitle);
            this.targetsTab.Location = new System.Drawing.Point(4, 24);
            this.targetsTab.Name = "targetsTab";
            this.targetsTab.Padding = new System.Windows.Forms.Padding(3);
            this.targetsTab.Size = new System.Drawing.Size(756, 371);
            this.targetsTab.TabIndex = 1;
            this.targetsTab.Text = "Notify Targets";
            // 
            // lblTargetsTitle
            // 
            this.lblTargetsTitle.AutoSize = true;
            this.lblTargetsTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTargetsTitle.ForeColor = System.Drawing.Color.White;
            this.lblTargetsTitle.Location = new System.Drawing.Point(20, 10);
            this.lblTargetsTitle.Name = "lblTargetsTitle";
            this.lblTargetsTitle.Size = new System.Drawing.Size(162, 21);
            this.lblTargetsTitle.TabIndex = 0;
            this.lblTargetsTitle.Text = "Notification Targets";
            // 
            // _lstTargets
            // 
            this._lstTargets.BackColor = System.Drawing.Color.FromArgb(45, 60, 80);
            this._lstTargets.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._lstTargets.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6});
            this._lstTargets.ForeColor = System.Drawing.Color.White;
            this._lstTargets.FullRowSelect = true;
            this._lstTargets.Location = new System.Drawing.Point(20, 45);
            this._lstTargets.Name = "_lstTargets";
            this._lstTargets.Size = new System.Drawing.Size(600, 320);
            this._lstTargets.TabIndex = 1;
            this._lstTargets.UseCompatibleStateImageBehavior = false;
            this._lstTargets.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Name";
            this.columnHeader4.Width = 200;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Description";
            this.columnHeader5.Width = 250;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Status";
            this.columnHeader6.Width = 100;
            // 
            // _btnAddTarget
            // 
            this._btnAddTarget.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            this._btnAddTarget.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnAddTarget.ForeColor = System.Drawing.Color.White;
            this._btnAddTarget.Location = new System.Drawing.Point(630, 45);
            this._btnAddTarget.Name = "_btnAddTarget";
            this._btnAddTarget.Size = new System.Drawing.Size(100, 30);
            this._btnAddTarget.TabIndex = 2;
            this._btnAddTarget.Text = "Add Target";
            this._btnAddTarget.UseVisualStyleBackColor = false;
            this._btnAddTarget.Click += new System.EventHandler(this.BtnAddTarget_Click);
            // 
            // flowsTab
            // 
            this.flowsTab.BackColor = System.Drawing.Color.FromArgb(27, 40, 56);
            this.flowsTab.Controls.Add(this._btnAddFlow);
            this.flowsTab.Controls.Add(this._lstFlows);
            this.flowsTab.Controls.Add(this.lblFlowsTitle);
            this.flowsTab.Location = new System.Drawing.Point(4, 24);
            this.flowsTab.Name = "flowsTab";
            this.flowsTab.Size = new System.Drawing.Size(756, 371);
            this.flowsTab.TabIndex = 2;
            this.flowsTab.Text = "Notify Flows";
            // 
            // lblFlowsTitle
            // 
            this.lblFlowsTitle.AutoSize = true;
            this.lblFlowsTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblFlowsTitle.ForeColor = System.Drawing.Color.White;
            this.lblFlowsTitle.Location = new System.Drawing.Point(20, 10);
            this.lblFlowsTitle.Name = "lblFlowsTitle";
            this.lblFlowsTitle.Size = new System.Drawing.Size(146, 21);
            this.lblFlowsTitle.TabIndex = 0;
            this.lblFlowsTitle.Text = "Notification Flows";
            // 
            // _lstFlows
            // 
            this._lstFlows.BackColor = System.Drawing.Color.FromArgb(45, 60, 80);
            this._lstFlows.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._lstFlows.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader7,
            this.columnHeader8,
            this.columnHeader9});
            this._lstFlows.ForeColor = System.Drawing.Color.White;
            this._lstFlows.FullRowSelect = true;
            this._lstFlows.Location = new System.Drawing.Point(20, 45);
            this._lstFlows.Name = "_lstFlows";
            this._lstFlows.Size = new System.Drawing.Size(600, 320);
            this._lstFlows.TabIndex = 1;
            this._lstFlows.UseCompatibleStateImageBehavior = false;
            this._lstFlows.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Name";
            this.columnHeader7.Width = 200;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "Description";
            this.columnHeader8.Width = 250;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "Status";
            this.columnHeader9.Width = 100;
            // 
            // _btnAddFlow
            // 
            this._btnAddFlow.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            this._btnAddFlow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnAddFlow.ForeColor = System.Drawing.Color.White;
            this._btnAddFlow.Location = new System.Drawing.Point(630, 45);
            this._btnAddFlow.Name = "_btnAddFlow";
            this._btnAddFlow.Size = new System.Drawing.Size(100, 30);
            this._btnAddFlow.TabIndex = 2;
            this._btnAddFlow.Text = "Add Flow";
            this._btnAddFlow.UseVisualStyleBackColor = false;
            this._btnAddFlow.Click += new System.EventHandler(this.BtnAddFlow_Click);
            // 
            // buttonPanel
            // 
            this.buttonPanel.BackColor = System.Drawing.Color.FromArgb(23, 29, 37);
            this.buttonPanel.Controls.Add(this._btnClose);
            this.buttonPanel.Controls.Add(this._btnRefresh);
            this.buttonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonPanel.Location = new System.Drawing.Point(0, 489);
            this.buttonPanel.Name = "buttonPanel";
            this.buttonPanel.Size = new System.Drawing.Size(784, 60);
            this.buttonPanel.TabIndex = 1;
            // 
            // _btnRefresh
            // 
            this._btnRefresh.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            this._btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnRefresh.ForeColor = System.Drawing.Color.White;
            this._btnRefresh.Location = new System.Drawing.Point(20, 15);
            this._btnRefresh.Name = "_btnRefresh";
            this._btnRefresh.Size = new System.Drawing.Size(100, 35);
            this._btnRefresh.TabIndex = 0;
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
            this._btnClose.Location = new System.Drawing.Point(664, 15);
            this._btnClose.Name = "_btnClose";
            this._btnClose.Size = new System.Drawing.Size(100, 35);
            this._btnClose.TabIndex = 1;
            this._btnClose.Text = "Close";
            this._btnClose.UseVisualStyleBackColor = false;
            this._btnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // _statusStrip
            // 
            this._statusStrip.BackColor = System.Drawing.Color.FromArgb(23, 29, 37);
            this._statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._statusLabel});
            this._statusStrip.Location = new System.Drawing.Point(0, 549);
            this._statusStrip.Name = "_statusStrip";
            this._statusStrip.Size = new System.Drawing.Size(784, 22);
            this._statusStrip.TabIndex = 2;
            // 
            // _statusLabel
            // 
            this._statusLabel.ForeColor = System.Drawing.Color.LightGray;
            this._statusLabel.Name = "_statusLabel";
            this._statusLabel.Size = new System.Drawing.Size(769, 17);
            this._statusLabel.Spring = true;
            this._statusLabel.Text = "Ready";
            this._statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // NotificationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(27, 40, 56);
            this.ClientSize = new System.Drawing.Size(784, 571);
            this.Controls.Add(this.tabPanel);
            this.Controls.Add(this.buttonPanel);
            this.Controls.Add(this._statusStrip);
            this.MinimumSize = new System.Drawing.Size(700, 450);
            this.Name = "NotificationForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Notification Settings";
            this.Load += new System.EventHandler(this.NotificationForm_Load);
            this.tabPanel.ResumeLayout(false);
            this._tabControl.ResumeLayout(false);
            this.notificationsTab.ResumeLayout(false);
            this.notificationsTab.PerformLayout();
            this.targetsTab.ResumeLayout(false);
            this.targetsTab.PerformLayout();
            this.flowsTab.ResumeLayout(false);
            this.flowsTab.PerformLayout();
            this.buttonPanel.ResumeLayout(false);
            this._statusStrip.ResumeLayout(false);
            this._statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Panel tabPanel;
        private System.Windows.Forms.TabControl _tabControl;
        private System.Windows.Forms.TabPage notificationsTab;
        private System.Windows.Forms.Label lblNotificationsTitle;
        private System.Windows.Forms.ListView _lstNotifications;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.Button _btnMarkRead;
        private System.Windows.Forms.TabPage targetsTab;
        private System.Windows.Forms.Label lblTargetsTitle;
        private System.Windows.Forms.ListView _lstTargets;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.Button _btnAddTarget;
        private System.Windows.Forms.TabPage flowsTab;
        private System.Windows.Forms.Label lblFlowsTitle;
        private System.Windows.Forms.ListView _lstFlows;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.Button _btnAddFlow;
        private System.Windows.Forms.Panel buttonPanel;
        private System.Windows.Forms.Button _btnRefresh;
        private System.Windows.Forms.Button _btnClose;
        private System.Windows.Forms.StatusStrip _statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel _statusLabel;
    }
}
