namespace Waiter.Forms
{
    partial class StorageCapacityForm
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
            this._progressStorage = new System.Windows.Forms.ProgressBar();
            this._lblPercentage = new System.Windows.Forms.Label();
            this._lblStorageInfo = new System.Windows.Forms.Label();
            this.buttonPanel = new System.Windows.Forms.Panel();
            this._btnRefresh = new System.Windows.Forms.Button();
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
            this.lblTitle.Size = new System.Drawing.Size(169, 25);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Storage Overview";
            // 
            // _progressStorage
            // 
            this._progressStorage.Location = new System.Drawing.Point(20, 60);
            this._progressStorage.Maximum = 100;
            this._progressStorage.Name = "_progressStorage";
            this._progressStorage.Size = new System.Drawing.Size(440, 25);
            this._progressStorage.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this._progressStorage.TabIndex = 1;
            // 
            // _lblPercentage
            // 
            this._lblPercentage.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this._lblPercentage.ForeColor = System.Drawing.Color.White;
            this._lblPercentage.Location = new System.Drawing.Point(380, 95);
            this._lblPercentage.Name = "_lblPercentage";
            this._lblPercentage.Size = new System.Drawing.Size(80, 25);
            this._lblPercentage.TabIndex = 2;
            this._lblPercentage.Text = "Loading...";
            this._lblPercentage.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // _lblStorageInfo
            // 
            this._lblStorageInfo.Font = new System.Drawing.Font("Segoe UI", 10F);
            this._lblStorageInfo.ForeColor = System.Drawing.Color.LightGray;
            this._lblStorageInfo.Location = new System.Drawing.Point(20, 95);
            this._lblStorageInfo.Name = "_lblStorageInfo";
            this._lblStorageInfo.Size = new System.Drawing.Size(350, 50);
            this._lblStorageInfo.TabIndex = 3;
            this._lblStorageInfo.Text = "Loading storage information...";
            // 
            // buttonPanel
            // 
            this.buttonPanel.BackColor = System.Drawing.Color.FromArgb(23, 29, 37);
            this.buttonPanel.Controls.Add(this._btnClose);
            this.buttonPanel.Controls.Add(this._btnRefresh);
            this.buttonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonPanel.Location = new System.Drawing.Point(0, 161);
            this.buttonPanel.Name = "buttonPanel";
            this.buttonPanel.Size = new System.Drawing.Size(484, 60);
            this.buttonPanel.TabIndex = 4;
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
            this._btnClose.Location = new System.Drawing.Point(364, 15);
            this._btnClose.Name = "_btnClose";
            this._btnClose.Size = new System.Drawing.Size(100, 35);
            this._btnClose.TabIndex = 1;
            this._btnClose.Text = "Close";
            this._btnClose.UseVisualStyleBackColor = false;
            this._btnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // StorageCapacityForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(27, 40, 56);
            this.ClientSize = new System.Drawing.Size(484, 221);
            this.Controls.Add(this.buttonPanel);
            this.Controls.Add(this._lblStorageInfo);
            this.Controls.Add(this._lblPercentage);
            this.Controls.Add(this._progressStorage);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "StorageCapacityForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Storage Capacity";
            this.Load += new System.EventHandler(this.StorageCapacityForm_Load);
            this.buttonPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.ProgressBar _progressStorage;
        private System.Windows.Forms.Label _lblPercentage;
        private System.Windows.Forms.Label _lblStorageInfo;
        private System.Windows.Forms.Panel buttonPanel;
        private System.Windows.Forms.Button _btnRefresh;
        private System.Windows.Forms.Button _btnClose;
    }
}
