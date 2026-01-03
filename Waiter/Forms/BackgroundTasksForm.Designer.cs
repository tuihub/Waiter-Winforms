namespace Waiter.Forms
{
    partial class BackgroundTasksForm
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
            this._listViewTasks = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this._lblStatus = new System.Windows.Forms.Label();
            this._btnClear = new System.Windows.Forms.Button();
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
            this.lblTitle.Size = new System.Drawing.Size(193, 30);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Background Tasks";
            // 
            // _listViewTasks
            // 
            this._listViewTasks.BackColor = System.Drawing.Color.FromArgb(45, 45, 45);
            this._listViewTasks.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5});
            this._listViewTasks.ForeColor = System.Drawing.Color.White;
            this._listViewTasks.FullRowSelect = true;
            this._listViewTasks.GridLines = true;
            this._listViewTasks.Location = new System.Drawing.Point(20, 55);
            this._listViewTasks.Name = "_listViewTasks";
            this._listViewTasks.Size = new System.Drawing.Size(640, 250);
            this._listViewTasks.TabIndex = 1;
            this._listViewTasks.UseCompatibleStateImageBehavior = false;
            this._listViewTasks.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 200;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Type";
            this.columnHeader2.Width = 80;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Status";
            this.columnHeader3.Width = 80;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Progress";
            this.columnHeader4.Width = 80;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Message";
            this.columnHeader5.Width = 180;
            // 
            // _lblStatus
            // 
            this._lblStatus.ForeColor = System.Drawing.Color.LightGray;
            this._lblStatus.Location = new System.Drawing.Point(20, 315);
            this._lblStatus.Name = "_lblStatus";
            this._lblStatus.Size = new System.Drawing.Size(400, 20);
            this._lblStatus.TabIndex = 2;
            // 
            // _btnClear
            // 
            this._btnClear.BackColor = System.Drawing.Color.FromArgb(60, 60, 60);
            this._btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnClear.ForeColor = System.Drawing.Color.White;
            this._btnClear.Location = new System.Drawing.Point(450, 315);
            this._btnClear.Name = "_btnClear";
            this._btnClear.Size = new System.Drawing.Size(100, 30);
            this._btnClear.TabIndex = 3;
            this._btnClear.Text = "Clear Completed";
            this._btnClear.UseVisualStyleBackColor = false;
            this._btnClear.Click += new System.EventHandler(this.BtnClear_Click);
            // 
            // _btnClose
            // 
            this._btnClose.BackColor = System.Drawing.Color.FromArgb(60, 60, 60);
            this._btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnClose.ForeColor = System.Drawing.Color.White;
            this._btnClose.Location = new System.Drawing.Point(560, 315);
            this._btnClose.Name = "_btnClose";
            this._btnClose.Size = new System.Drawing.Size(100, 30);
            this._btnClose.TabIndex = 4;
            this._btnClose.Text = "Close";
            this._btnClose.UseVisualStyleBackColor = false;
            this._btnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // BackgroundTasksForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this.ClientSize = new System.Drawing.Size(684, 361);
            this.Controls.Add(this._btnClose);
            this.Controls.Add(this._btnClear);
            this.Controls.Add(this._lblStatus);
            this.Controls.Add(this._listViewTasks);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "BackgroundTasksForm";
            this.Text = "Background Tasks";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BackgroundTasksForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.ListView _listViewTasks;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.Label _lblStatus;
        private System.Windows.Forms.Button _btnClear;
        private System.Windows.Forms.Button _btnClose;
    }
}
