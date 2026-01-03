namespace Waiter.Forms
{
    partial class AppCategoryForm
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
            this._listViewCategories = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this._btnAdd = new System.Windows.Forms.Button();
            this._btnEdit = new System.Windows.Forms.Button();
            this._btnDelete = new System.Windows.Forms.Button();
            this._btnRefresh = new System.Windows.Forms.Button();
            this._lblStatus = new System.Windows.Forms.Label();
            this._btnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(20, 15);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(169, 30);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "App Categories";
            // 
            // _listViewCategories
            // 
            this._listViewCategories.BackColor = System.Drawing.Color.FromArgb(45, 45, 45);
            this._listViewCategories.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this._listViewCategories.ForeColor = System.Drawing.Color.White;
            this._listViewCategories.FullRowSelect = true;
            this._listViewCategories.GridLines = true;
            this._listViewCategories.Location = new System.Drawing.Point(20, 55);
            this._listViewCategories.Name = "_listViewCategories";
            this._listViewCategories.Size = new System.Drawing.Size(440, 300);
            this._listViewCategories.TabIndex = 1;
            this._listViewCategories.UseCompatibleStateImageBehavior = false;
            this._listViewCategories.View = System.Windows.Forms.View.Details;
            this._listViewCategories.SelectedIndexChanged += new System.EventHandler(this.ListView_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "ID";
            this.columnHeader1.Width = 80;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Name";
            this.columnHeader2.Width = 200;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Apps Count";
            this.columnHeader3.Width = 80;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Version";
            this.columnHeader4.Width = 60;
            // 
            // _btnAdd
            // 
            this._btnAdd.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            this._btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnAdd.ForeColor = System.Drawing.Color.White;
            this._btnAdd.Location = new System.Drawing.Point(480, 55);
            this._btnAdd.Name = "_btnAdd";
            this._btnAdd.Size = new System.Drawing.Size(90, 30);
            this._btnAdd.TabIndex = 2;
            this._btnAdd.Text = "Add";
            this._btnAdd.UseVisualStyleBackColor = false;
            this._btnAdd.Click += new System.EventHandler(this.BtnAdd_Click);
            // 
            // _btnEdit
            // 
            this._btnEdit.BackColor = System.Drawing.Color.FromArgb(60, 60, 60);
            this._btnEdit.Enabled = false;
            this._btnEdit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnEdit.ForeColor = System.Drawing.Color.White;
            this._btnEdit.Location = new System.Drawing.Point(480, 95);
            this._btnEdit.Name = "_btnEdit";
            this._btnEdit.Size = new System.Drawing.Size(90, 30);
            this._btnEdit.TabIndex = 3;
            this._btnEdit.Text = "Edit";
            this._btnEdit.UseVisualStyleBackColor = false;
            this._btnEdit.Click += new System.EventHandler(this.BtnEdit_Click);
            // 
            // _btnDelete
            // 
            this._btnDelete.BackColor = System.Drawing.Color.FromArgb(180, 50, 50);
            this._btnDelete.Enabled = false;
            this._btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnDelete.ForeColor = System.Drawing.Color.White;
            this._btnDelete.Location = new System.Drawing.Point(480, 135);
            this._btnDelete.Name = "_btnDelete";
            this._btnDelete.Size = new System.Drawing.Size(90, 30);
            this._btnDelete.TabIndex = 4;
            this._btnDelete.Text = "Delete";
            this._btnDelete.UseVisualStyleBackColor = false;
            this._btnDelete.Click += new System.EventHandler(this.BtnDelete_Click);
            // 
            // _btnRefresh
            // 
            this._btnRefresh.BackColor = System.Drawing.Color.FromArgb(60, 60, 60);
            this._btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnRefresh.ForeColor = System.Drawing.Color.White;
            this._btnRefresh.Location = new System.Drawing.Point(480, 185);
            this._btnRefresh.Name = "_btnRefresh";
            this._btnRefresh.Size = new System.Drawing.Size(90, 30);
            this._btnRefresh.TabIndex = 5;
            this._btnRefresh.Text = "Refresh";
            this._btnRefresh.UseVisualStyleBackColor = false;
            this._btnRefresh.Click += new System.EventHandler(this.BtnRefresh_Click);
            // 
            // _lblStatus
            // 
            this._lblStatus.ForeColor = System.Drawing.Color.LightGray;
            this._lblStatus.Location = new System.Drawing.Point(20, 365);
            this._lblStatus.Name = "_lblStatus";
            this._lblStatus.Size = new System.Drawing.Size(440, 20);
            this._lblStatus.TabIndex = 6;
            // 
            // _btnClose
            // 
            this._btnClose.BackColor = System.Drawing.Color.FromArgb(60, 60, 60);
            this._btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnClose.ForeColor = System.Drawing.Color.White;
            this._btnClose.Location = new System.Drawing.Point(480, 370);
            this._btnClose.Name = "_btnClose";
            this._btnClose.Size = new System.Drawing.Size(90, 30);
            this._btnClose.TabIndex = 7;
            this._btnClose.Text = "Close";
            this._btnClose.UseVisualStyleBackColor = false;
            this._btnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // AppCategoryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this.ClientSize = new System.Drawing.Size(584, 411);
            this.Controls.Add(this._btnClose);
            this.Controls.Add(this._lblStatus);
            this.Controls.Add(this._btnRefresh);
            this.Controls.Add(this._btnDelete);
            this.Controls.Add(this._btnEdit);
            this.Controls.Add(this._btnAdd);
            this.Controls.Add(this._listViewCategories);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "AppCategoryForm";
            this.Text = "App Category Management";
            this.Load += new System.EventHandler(this.AppCategoryForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.ListView _listViewCategories;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.Button _btnAdd;
        private System.Windows.Forms.Button _btnEdit;
        private System.Windows.Forms.Button _btnDelete;
        private System.Windows.Forms.Button _btnRefresh;
        private System.Windows.Forms.Label _lblStatus;
        private System.Windows.Forms.Button _btnClose;
    }
}
