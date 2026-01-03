namespace Waiter.Forms
{
    partial class CategoryEditDialog
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
            this._btnOk = new System.Windows.Forms.Button();
            this._btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.ForeColor = System.Drawing.Color.LightGray;
            this.lblName.Location = new System.Drawing.Point(20, 20);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(94, 15);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Category Name:";
            // 
            // _txtName
            // 
            this._txtName.BackColor = System.Drawing.Color.FromArgb(45, 45, 45);
            this._txtName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._txtName.ForeColor = System.Drawing.Color.White;
            this._txtName.Location = new System.Drawing.Point(20, 40);
            this._txtName.Name = "_txtName";
            this._txtName.Size = new System.Drawing.Size(290, 23);
            this._txtName.TabIndex = 1;
            // 
            // _btnOk
            // 
            this._btnOk.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            this._btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._btnOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnOk.ForeColor = System.Drawing.Color.White;
            this._btnOk.Location = new System.Drawing.Point(130, 75);
            this._btnOk.Name = "_btnOk";
            this._btnOk.Size = new System.Drawing.Size(80, 30);
            this._btnOk.TabIndex = 2;
            this._btnOk.Text = "OK";
            this._btnOk.UseVisualStyleBackColor = false;
            // 
            // _btnCancel
            // 
            this._btnCancel.BackColor = System.Drawing.Color.FromArgb(60, 60, 60);
            this._btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnCancel.ForeColor = System.Drawing.Color.White;
            this._btnCancel.Location = new System.Drawing.Point(220, 75);
            this._btnCancel.Name = "_btnCancel";
            this._btnCancel.Size = new System.Drawing.Size(80, 30);
            this._btnCancel.TabIndex = 3;
            this._btnCancel.Text = "Cancel";
            this._btnCancel.UseVisualStyleBackColor = false;
            // 
            // CategoryEditDialog
            // 
            this.AcceptButton = this._btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this.CancelButton = this._btnCancel;
            this.ClientSize = new System.Drawing.Size(334, 121);
            this.Controls.Add(this._btnCancel);
            this.Controls.Add(this._btnOk);
            this.Controls.Add(this._txtName);
            this.Controls.Add(this.lblName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CategoryEditDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox _txtName;
        private System.Windows.Forms.Button _btnOk;
        private System.Windows.Forms.Button _btnCancel;
    }
}
