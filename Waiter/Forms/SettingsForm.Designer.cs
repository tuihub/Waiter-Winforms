namespace Waiter.Forms
{
    partial class SettingsForm
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
            this.lblServerSection = new System.Windows.Forms.Label();
            this.lblServerUrl = new System.Windows.Forms.Label();
            this._txtServerUrl = new System.Windows.Forms.TextBox();
            this._lblStatus = new System.Windows.Forms.Label();
            this._btnSave = new System.Windows.Forms.Button();
            this._btnCancel = new System.Windows.Forms.Button();
            this._btnLogout = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(20, 15);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(93, 30);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Settings";
            // 
            // lblServerSection
            // 
            this.lblServerSection.AutoSize = true;
            this.lblServerSection.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblServerSection.ForeColor = System.Drawing.Color.LightGray;
            this.lblServerSection.Location = new System.Drawing.Point(20, 55);
            this.lblServerSection.Name = "lblServerSection";
            this.lblServerSection.Size = new System.Drawing.Size(149, 19);
            this.lblServerSection.TabIndex = 1;
            this.lblServerSection.Text = "Server Configuration";
            // 
            // lblServerUrl
            // 
            this.lblServerUrl.AutoSize = true;
            this.lblServerUrl.ForeColor = System.Drawing.Color.LightGray;
            this.lblServerUrl.Location = new System.Drawing.Point(20, 80);
            this.lblServerUrl.Name = "lblServerUrl";
            this.lblServerUrl.Size = new System.Drawing.Size(66, 15);
            this.lblServerUrl.TabIndex = 2;
            this.lblServerUrl.Text = "Server URL:";
            // 
            // _txtServerUrl
            // 
            this._txtServerUrl.BackColor = System.Drawing.Color.FromArgb(45, 45, 45);
            this._txtServerUrl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._txtServerUrl.ForeColor = System.Drawing.Color.White;
            this._txtServerUrl.Location = new System.Drawing.Point(20, 100);
            this._txtServerUrl.Name = "_txtServerUrl";
            this._txtServerUrl.Size = new System.Drawing.Size(390, 23);
            this._txtServerUrl.TabIndex = 3;
            // 
            // _lblStatus
            // 
            this._lblStatus.ForeColor = System.Drawing.Color.LimeGreen;
            this._lblStatus.Location = new System.Drawing.Point(20, 130);
            this._lblStatus.Name = "_lblStatus";
            this._lblStatus.Size = new System.Drawing.Size(390, 20);
            this._lblStatus.TabIndex = 4;
            this._lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _btnSave
            // 
            this._btnSave.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            this._btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnSave.ForeColor = System.Drawing.Color.White;
            this._btnSave.Location = new System.Drawing.Point(140, 165);
            this._btnSave.Name = "_btnSave";
            this._btnSave.Size = new System.Drawing.Size(80, 30);
            this._btnSave.TabIndex = 5;
            this._btnSave.Text = "Save";
            this._btnSave.UseVisualStyleBackColor = false;
            this._btnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // _btnCancel
            // 
            this._btnCancel.BackColor = System.Drawing.Color.FromArgb(60, 60, 60);
            this._btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnCancel.ForeColor = System.Drawing.Color.White;
            this._btnCancel.Location = new System.Drawing.Point(230, 165);
            this._btnCancel.Name = "_btnCancel";
            this._btnCancel.Size = new System.Drawing.Size(80, 30);
            this._btnCancel.TabIndex = 6;
            this._btnCancel.Text = "Cancel";
            this._btnCancel.UseVisualStyleBackColor = false;
            this._btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // _btnLogout
            // 
            this._btnLogout.BackColor = System.Drawing.Color.FromArgb(180, 50, 50);
            this._btnLogout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnLogout.ForeColor = System.Drawing.Color.White;
            this._btnLogout.Location = new System.Drawing.Point(320, 165);
            this._btnLogout.Name = "_btnLogout";
            this._btnLogout.Size = new System.Drawing.Size(80, 30);
            this._btnLogout.TabIndex = 7;
            this._btnLogout.Text = "Logout";
            this._btnLogout.UseVisualStyleBackColor = false;
            this._btnLogout.Click += new System.EventHandler(this.BtnLogout_Click);
            // 
            // SettingsForm
            // 
            this.AcceptButton = this._btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this.CancelButton = this._btnCancel;
            this.ClientSize = new System.Drawing.Size(434, 211);
            this.Controls.Add(this._btnLogout);
            this.Controls.Add(this._btnCancel);
            this.Controls.Add(this._btnSave);
            this.Controls.Add(this._lblStatus);
            this.Controls.Add(this._txtServerUrl);
            this.Controls.Add(this.lblServerUrl);
            this.Controls.Add(this.lblServerSection);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.Text = "Settings";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblServerSection;
        private System.Windows.Forms.Label lblServerUrl;
        private System.Windows.Forms.TextBox _txtServerUrl;
        private System.Windows.Forms.Label _lblStatus;
        private System.Windows.Forms.Button _btnSave;
        private System.Windows.Forms.Button _btnCancel;
        private System.Windows.Forms.Button _btnLogout;
    }
}
