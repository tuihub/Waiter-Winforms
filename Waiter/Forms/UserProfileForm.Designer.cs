namespace Waiter.Forms
{
    partial class UserProfileForm
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
            this.lblTitle = new System.Windows.Forms.Label();
            this._lblUsername = new System.Windows.Forms.Label();
            this._lblUserId = new System.Windows.Forms.Label();
            this._lblStatus = new System.Windows.Forms.Label();
            this.lblSessions = new System.Windows.Forms.Label();
            this._lstSessions = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this._btnDeleteSession = new System.Windows.Forms.Button();
            this._btnRefresh = new System.Windows.Forms.Button();
            this.buttonPanel = new System.Windows.Forms.Panel();
            this._btnClose = new System.Windows.Forms.Button();
            this.buttonPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(20, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(157, 25);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "User Information";
            // 
            // _lblUsername
            // 
            this._lblUsername.Font = new System.Drawing.Font("Segoe UI", 11F);
            this._lblUsername.ForeColor = System.Drawing.Color.LightGray;
            this._lblUsername.Location = new System.Drawing.Point(20, 55);
            this._lblUsername.Name = "_lblUsername";
            this._lblUsername.Size = new System.Drawing.Size(400, 25);
            this._lblUsername.TabIndex = 1;
            this._lblUsername.Text = "Username: Loading...";
            // 
            // _lblUserId
            // 
            this._lblUserId.Font = new System.Drawing.Font("Segoe UI", 11F);
            this._lblUserId.ForeColor = System.Drawing.Color.LightGray;
            this._lblUserId.Location = new System.Drawing.Point(20, 85);
            this._lblUserId.Name = "_lblUserId";
            this._lblUserId.Size = new System.Drawing.Size(400, 25);
            this._lblUserId.TabIndex = 2;
            this._lblUserId.Text = "User ID: Loading...";
            // 
            // _lblStatus
            // 
            this._lblStatus.Font = new System.Drawing.Font("Segoe UI", 11F);
            this._lblStatus.ForeColor = System.Drawing.Color.LightGray;
            this._lblStatus.Location = new System.Drawing.Point(20, 115);
            this._lblStatus.Name = "_lblStatus";
            this._lblStatus.Size = new System.Drawing.Size(400, 25);
            this._lblStatus.TabIndex = 3;
            this._lblStatus.Text = "Status: Loading...";
            // 
            // lblSessions
            // 
            this.lblSessions.AutoSize = true;
            this.lblSessions.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblSessions.ForeColor = System.Drawing.Color.White;
            this.lblSessions.Location = new System.Drawing.Point(20, 160);
            this.lblSessions.Name = "lblSessions";
            this.lblSessions.Size = new System.Drawing.Size(124, 21);
            this.lblSessions.TabIndex = 4;
            this.lblSessions.Text = "Active Sessions";
            // 
            // _lstSessions
            // 
            this._lstSessions.BackColor = System.Drawing.Color.FromArgb(45, 60, 80);
            this._lstSessions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._lstSessions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this._lstSessions.ForeColor = System.Drawing.Color.White;
            this._lstSessions.FullRowSelect = true;
            this._lstSessions.Location = new System.Drawing.Point(20, 190);
            this._lstSessions.Name = "_lstSessions";
            this._lstSessions.Size = new System.Drawing.Size(440, 200);
            this._lstSessions.TabIndex = 5;
            this._lstSessions.UseCompatibleStateImageBehavior = false;
            this._lstSessions.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Device";
            this.columnHeader1.Width = 200;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Session ID";
            this.columnHeader2.Width = 200;
            // 
            // _btnDeleteSession
            // 
            this._btnDeleteSession.BackColor = System.Drawing.Color.FromArgb(200, 50, 50);
            this._btnDeleteSession.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnDeleteSession.ForeColor = System.Drawing.Color.White;
            this._btnDeleteSession.Location = new System.Drawing.Point(470, 190);
            this._btnDeleteSession.Name = "_btnDeleteSession";
            this._btnDeleteSession.Size = new System.Drawing.Size(100, 30);
            this._btnDeleteSession.TabIndex = 6;
            this._btnDeleteSession.Text = "End Session";
            this._btnDeleteSession.UseVisualStyleBackColor = false;
            this._btnDeleteSession.Click += new System.EventHandler(this.BtnDeleteSession_Click);
            // 
            // _btnRefresh
            // 
            this._btnRefresh.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            this._btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnRefresh.ForeColor = System.Drawing.Color.White;
            this._btnRefresh.Location = new System.Drawing.Point(470, 230);
            this._btnRefresh.Name = "_btnRefresh";
            this._btnRefresh.Size = new System.Drawing.Size(100, 30);
            this._btnRefresh.TabIndex = 7;
            this._btnRefresh.Text = "Refresh";
            this._btnRefresh.UseVisualStyleBackColor = false;
            this._btnRefresh.Click += new System.EventHandler(this.BtnRefresh_Click);
            // 
            // buttonPanel
            // 
            this.buttonPanel.BackColor = System.Drawing.Color.FromArgb(23, 29, 37);
            this.buttonPanel.Controls.Add(this._btnClose);
            this.buttonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonPanel.Location = new System.Drawing.Point(0, 401);
            this.buttonPanel.Name = "buttonPanel";
            this.buttonPanel.Size = new System.Drawing.Size(584, 60);
            this.buttonPanel.TabIndex = 8;
            // 
            // _btnClose
            // 
            this._btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._btnClose.BackColor = System.Drawing.Color.FromArgb(60, 60, 60);
            this._btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnClose.ForeColor = System.Drawing.Color.White;
            this._btnClose.Location = new System.Drawing.Point(464, 15);
            this._btnClose.Name = "_btnClose";
            this._btnClose.Size = new System.Drawing.Size(100, 35);
            this._btnClose.TabIndex = 0;
            this._btnClose.Text = "Close";
            this._btnClose.UseVisualStyleBackColor = false;
            this._btnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // UserProfileForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(27, 40, 56);
            this.ClientSize = new System.Drawing.Size(584, 461);
            this.Controls.Add(this.buttonPanel);
            this.Controls.Add(this._btnRefresh);
            this.Controls.Add(this._btnDeleteSession);
            this.Controls.Add(this._lstSessions);
            this.Controls.Add(this.lblSessions);
            this.Controls.Add(this._lblStatus);
            this.Controls.Add(this._lblUserId);
            this.Controls.Add(this._lblUsername);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(500, 400);
            this.Name = "UserProfileForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "User Profile";
            this.Load += new System.EventHandler(this.UserProfileForm_Load);
            this.buttonPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label _lblUsername;
        private System.Windows.Forms.Label _lblUserId;
        private System.Windows.Forms.Label _lblStatus;
        private System.Windows.Forms.Label lblSessions;
        private System.Windows.Forms.ListView _lstSessions;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Button _btnDeleteSession;
        private System.Windows.Forms.Button _btnRefresh;
        private System.Windows.Forms.Panel buttonPanel;
        private System.Windows.Forms.Button _btnClose;
    }
}
