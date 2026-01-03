namespace Waiter.Forms
{
    partial class AppDetailForm
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
            this.lblName = new System.Windows.Forms.Label();
            this._txtName = new System.Windows.Forms.TextBox();
            this.lblType = new System.Windows.Forms.Label();
            this._cboType = new System.Windows.Forms.ComboBox();
            this.lblPublisher = new System.Windows.Forms.Label();
            this._txtPublisher = new System.Windows.Forms.TextBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this._txtDescription = new System.Windows.Forms.TextBox();
            this.buttonPanel = new System.Windows.Forms.Panel();
            this._btnSave = new System.Windows.Forms.Button();
            this._btnCancel = new System.Windows.Forms.Button();
            this._btnDelete = new System.Windows.Forms.Button();
            this.buttonPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblName
            // 
            this.lblName.ForeColor = System.Drawing.Color.White;
            this.lblName.Location = new System.Drawing.Point(20, 20);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(100, 20);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Name:";
            // 
            // _txtName
            // 
            this._txtName.BackColor = System.Drawing.Color.FromArgb(45, 60, 80);
            this._txtName.ForeColor = System.Drawing.Color.White;
            this._txtName.Location = new System.Drawing.Point(120, 17);
            this._txtName.Name = "_txtName";
            this._txtName.Size = new System.Drawing.Size(380, 23);
            this._txtName.TabIndex = 1;
            // 
            // lblType
            // 
            this.lblType.ForeColor = System.Drawing.Color.White;
            this.lblType.Location = new System.Drawing.Point(20, 60);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(100, 20);
            this.lblType.TabIndex = 2;
            this.lblType.Text = "Type:";
            // 
            // _cboType
            // 
            this._cboType.BackColor = System.Drawing.Color.FromArgb(45, 60, 80);
            this._cboType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cboType.ForeColor = System.Drawing.Color.White;
            this._cboType.FormattingEnabled = true;
            this._cboType.Items.AddRange(new object[] {
            "Game",
            "Other"});
            this._cboType.Location = new System.Drawing.Point(120, 57);
            this._cboType.Name = "_cboType";
            this._cboType.Size = new System.Drawing.Size(200, 23);
            this._cboType.TabIndex = 3;
            // 
            // lblPublisher
            // 
            this.lblPublisher.ForeColor = System.Drawing.Color.White;
            this.lblPublisher.Location = new System.Drawing.Point(20, 100);
            this.lblPublisher.Name = "lblPublisher";
            this.lblPublisher.Size = new System.Drawing.Size(100, 20);
            this.lblPublisher.TabIndex = 4;
            this.lblPublisher.Text = "Publisher:";
            // 
            // _txtPublisher
            // 
            this._txtPublisher.BackColor = System.Drawing.Color.FromArgb(45, 60, 80);
            this._txtPublisher.ForeColor = System.Drawing.Color.White;
            this._txtPublisher.Location = new System.Drawing.Point(120, 97);
            this._txtPublisher.Name = "_txtPublisher";
            this._txtPublisher.Size = new System.Drawing.Size(380, 23);
            this._txtPublisher.TabIndex = 5;
            // 
            // lblDescription
            // 
            this.lblDescription.ForeColor = System.Drawing.Color.White;
            this.lblDescription.Location = new System.Drawing.Point(20, 140);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(100, 20);
            this.lblDescription.TabIndex = 6;
            this.lblDescription.Text = "Description:";
            // 
            // _txtDescription
            // 
            this._txtDescription.BackColor = System.Drawing.Color.FromArgb(45, 60, 80);
            this._txtDescription.ForeColor = System.Drawing.Color.White;
            this._txtDescription.Location = new System.Drawing.Point(120, 137);
            this._txtDescription.Multiline = true;
            this._txtDescription.Name = "_txtDescription";
            this._txtDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._txtDescription.Size = new System.Drawing.Size(380, 100);
            this._txtDescription.TabIndex = 7;
            // 
            // buttonPanel
            // 
            this.buttonPanel.BackColor = System.Drawing.Color.FromArgb(23, 29, 37);
            this.buttonPanel.Controls.Add(this._btnDelete);
            this.buttonPanel.Controls.Add(this._btnCancel);
            this.buttonPanel.Controls.Add(this._btnSave);
            this.buttonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonPanel.Location = new System.Drawing.Point(0, 281);
            this.buttonPanel.Name = "buttonPanel";
            this.buttonPanel.Size = new System.Drawing.Size(534, 60);
            this.buttonPanel.TabIndex = 8;
            // 
            // _btnSave
            // 
            this._btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._btnSave.BackColor = System.Drawing.Color.FromArgb(76, 175, 80);
            this._btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnSave.ForeColor = System.Drawing.Color.White;
            this._btnSave.Location = new System.Drawing.Point(304, 15);
            this._btnSave.Name = "_btnSave";
            this._btnSave.Size = new System.Drawing.Size(100, 35);
            this._btnSave.TabIndex = 0;
            this._btnSave.Text = "Save";
            this._btnSave.UseVisualStyleBackColor = false;
            this._btnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // _btnCancel
            // 
            this._btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._btnCancel.BackColor = System.Drawing.Color.FromArgb(60, 60, 60);
            this._btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnCancel.ForeColor = System.Drawing.Color.White;
            this._btnCancel.Location = new System.Drawing.Point(414, 15);
            this._btnCancel.Name = "_btnCancel";
            this._btnCancel.Size = new System.Drawing.Size(100, 35);
            this._btnCancel.TabIndex = 1;
            this._btnCancel.Text = "Cancel";
            this._btnCancel.UseVisualStyleBackColor = false;
            this._btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // _btnDelete
            // 
            this._btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._btnDelete.BackColor = System.Drawing.Color.FromArgb(200, 50, 50);
            this._btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnDelete.ForeColor = System.Drawing.Color.White;
            this._btnDelete.Location = new System.Drawing.Point(20, 15);
            this._btnDelete.Name = "_btnDelete";
            this._btnDelete.Size = new System.Drawing.Size(100, 35);
            this._btnDelete.TabIndex = 2;
            this._btnDelete.Text = "Delete";
            this._btnDelete.UseVisualStyleBackColor = false;
            this._btnDelete.Visible = false;
            this._btnDelete.Click += new System.EventHandler(this.BtnDelete_Click);
            // 
            // AppDetailForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(27, 40, 56);
            this.ClientSize = new System.Drawing.Size(534, 341);
            this.Controls.Add(this.buttonPanel);
            this.Controls.Add(this._txtDescription);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this._txtPublisher);
            this.Controls.Add(this.lblPublisher);
            this.Controls.Add(this._cboType);
            this.Controls.Add(this.lblType);
            this.Controls.Add(this._txtName);
            this.Controls.Add(this.lblName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "AppDetailForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "App Details";
            this.Load += new System.EventHandler(this.AppDetailForm_Load);
            this.buttonPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox _txtName;
        private System.Windows.Forms.Label lblType;
        private System.Windows.Forms.ComboBox _cboType;
        private System.Windows.Forms.Label lblPublisher;
        private System.Windows.Forms.TextBox _txtPublisher;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.TextBox _txtDescription;
        private System.Windows.Forms.Panel buttonPanel;
        private System.Windows.Forms.Button _btnSave;
        private System.Windows.Forms.Button _btnCancel;
        private System.Windows.Forms.Button _btnDelete;
    }
}
