namespace Waiter.Forms
{
    partial class LoginForm
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
            this.lblServer = new System.Windows.Forms.Label();
            this._txtServerUrl = new System.Windows.Forms.TextBox();
            this.lblUsername = new System.Windows.Forms.Label();
            this._txtUsername = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this._txtPassword = new System.Windows.Forms.TextBox();
            this._lblStatus = new System.Windows.Forms.Label();
            this._progressBar = new System.Windows.Forms.ProgressBar();
            this._btnLogin = new System.Windows.Forms.Button();
            this._btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(20, 15);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(154, 32);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "TuiHub Login";
            // 
            // lblServer
            // 
            this.lblServer.AutoSize = true;
            this.lblServer.ForeColor = System.Drawing.Color.LightGray;
            this.lblServer.Location = new System.Drawing.Point(20, 60);
            this.lblServer.Name = "lblServer";
            this.lblServer.Size = new System.Drawing.Size(66, 15);
            this.lblServer.TabIndex = 1;
            this.lblServer.Text = "Server URL:";
            // 
            // _txtServerUrl
            // 
            this._txtServerUrl.BackColor = System.Drawing.Color.FromArgb(45, 45, 45);
            this._txtServerUrl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._txtServerUrl.ForeColor = System.Drawing.Color.White;
            this._txtServerUrl.Location = new System.Drawing.Point(20, 80);
            this._txtServerUrl.Name = "_txtServerUrl";
            this._txtServerUrl.Size = new System.Drawing.Size(340, 23);
            this._txtServerUrl.TabIndex = 2;
            // 
            // lblUsername
            // 
            this.lblUsername.AutoSize = true;
            this.lblUsername.ForeColor = System.Drawing.Color.LightGray;
            this.lblUsername.Location = new System.Drawing.Point(20, 115);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(63, 15);
            this.lblUsername.TabIndex = 3;
            this.lblUsername.Text = "Username:";
            // 
            // _txtUsername
            // 
            this._txtUsername.BackColor = System.Drawing.Color.FromArgb(45, 45, 45);
            this._txtUsername.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._txtUsername.ForeColor = System.Drawing.Color.White;
            this._txtUsername.Location = new System.Drawing.Point(20, 135);
            this._txtUsername.Name = "_txtUsername";
            this._txtUsername.Size = new System.Drawing.Size(340, 23);
            this._txtUsername.TabIndex = 4;
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.ForeColor = System.Drawing.Color.LightGray;
            this.lblPassword.Location = new System.Drawing.Point(20, 170);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(60, 15);
            this.lblPassword.TabIndex = 5;
            this.lblPassword.Text = "Password:";
            // 
            // _txtPassword
            // 
            this._txtPassword.BackColor = System.Drawing.Color.FromArgb(45, 45, 45);
            this._txtPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._txtPassword.ForeColor = System.Drawing.Color.White;
            this._txtPassword.Location = new System.Drawing.Point(20, 190);
            this._txtPassword.Name = "_txtPassword";
            this._txtPassword.Size = new System.Drawing.Size(340, 23);
            this._txtPassword.TabIndex = 6;
            this._txtPassword.UseSystemPasswordChar = true;
            this._txtPassword.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtPassword_KeyPress);
            // 
            // _lblStatus
            // 
            this._lblStatus.ForeColor = System.Drawing.Color.OrangeRed;
            this._lblStatus.Location = new System.Drawing.Point(20, 220);
            this._lblStatus.Name = "_lblStatus";
            this._lblStatus.Size = new System.Drawing.Size(340, 20);
            this._lblStatus.TabIndex = 7;
            this._lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _progressBar
            // 
            this._progressBar.Location = new System.Drawing.Point(20, 220);
            this._progressBar.Name = "_progressBar";
            this._progressBar.Size = new System.Drawing.Size(340, 20);
            this._progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this._progressBar.TabIndex = 8;
            this._progressBar.Visible = false;
            // 
            // _btnLogin
            // 
            this._btnLogin.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            this._btnLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnLogin.ForeColor = System.Drawing.Color.White;
            this._btnLogin.Location = new System.Drawing.Point(120, 250);
            this._btnLogin.Name = "_btnLogin";
            this._btnLogin.Size = new System.Drawing.Size(80, 30);
            this._btnLogin.TabIndex = 9;
            this._btnLogin.Text = "Login";
            this._btnLogin.UseVisualStyleBackColor = false;
            this._btnLogin.Click += new System.EventHandler(this.BtnLogin_Click);
            // 
            // _btnCancel
            // 
            this._btnCancel.BackColor = System.Drawing.Color.FromArgb(60, 60, 60);
            this._btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnCancel.ForeColor = System.Drawing.Color.White;
            this._btnCancel.Location = new System.Drawing.Point(210, 250);
            this._btnCancel.Name = "_btnCancel";
            this._btnCancel.Size = new System.Drawing.Size(80, 30);
            this._btnCancel.TabIndex = 10;
            this._btnCancel.Text = "Cancel";
            this._btnCancel.UseVisualStyleBackColor = false;
            this._btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // LoginForm
            // 
            this.AcceptButton = this._btnLogin;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this.CancelButton = this._btnCancel;
            this.ClientSize = new System.Drawing.Size(384, 291);
            this.Controls.Add(this._btnCancel);
            this.Controls.Add(this._btnLogin);
            this.Controls.Add(this._progressBar);
            this.Controls.Add(this._lblStatus);
            this.Controls.Add(this._txtPassword);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this._txtUsername);
            this.Controls.Add(this.lblUsername);
            this.Controls.Add(this._txtServerUrl);
            this.Controls.Add(this.lblServer);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TuiHub - Login";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblServer;
        private System.Windows.Forms.TextBox _txtServerUrl;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.TextBox _txtUsername;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox _txtPassword;
        private System.Windows.Forms.Label _lblStatus;
        private System.Windows.Forms.ProgressBar _progressBar;
        private System.Windows.Forms.Button _btnLogin;
        private System.Windows.Forms.Button _btnCancel;
    }
}
