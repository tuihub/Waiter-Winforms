namespace Waiter.Forms
{
    partial class ProgressDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            _progressBar = new ProgressBar();
            _lblStatus = new Label();
            _btnCancel = new Button();
            _btnHide = new Button();
            SuspendLayout();
            // 
            // _progressBar
            // 
            _progressBar.Location = new Point(20, 50);
            _progressBar.Name = "_progressBar";
            _progressBar.Size = new Size(340, 25);
            _progressBar.Style = ProgressBarStyle.Marquee;
            _progressBar.TabIndex = 0;
            // 
            // _lblStatus
            // 
            _lblStatus.ForeColor = Color.LightGray;
            _lblStatus.Location = new Point(20, 20);
            _lblStatus.Name = "_lblStatus";
            _lblStatus.Size = new Size(340, 20);
            _lblStatus.TabIndex = 1;
            _lblStatus.Text = "Starting...";
            // 
            // _btnCancel
            // 
            _btnCancel.BackColor = Color.FromArgb(60, 60, 60);
            _btnCancel.FlatStyle = FlatStyle.Flat;
            _btnCancel.ForeColor = Color.White;
            _btnCancel.Location = new Point(280, 90);
            _btnCancel.Name = "_btnCancel";
            _btnCancel.Size = new Size(80, 30);
            _btnCancel.TabIndex = 2;
            _btnCancel.Text = "Cancel";
            _btnCancel.UseVisualStyleBackColor = false;
            _btnCancel.Click += BtnCancel_Click;
            // 
            // _btnHide
            // 
            _btnHide.BackColor = Color.FromArgb(60, 60, 60);
            _btnHide.FlatStyle = FlatStyle.Flat;
            _btnHide.ForeColor = Color.White;
            _btnHide.Location = new Point(190, 90);
            _btnHide.Name = "_btnHide";
            _btnHide.Size = new Size(80, 30);
            _btnHide.TabIndex = 3;
            _btnHide.Text = "Hide";
            _btnHide.UseVisualStyleBackColor = false;
            _btnHide.Click += BtnHide_Click;
            // 
            // ProgressDialog
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(30, 30, 30);
            ClientSize = new Size(380, 135);
            Controls.Add(_btnHide);
            Controls.Add(_btnCancel);
            Controls.Add(_lblStatus);
            Controls.Add(_progressBar);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = true;
            Name = "ProgressDialog";
            ShowInTaskbar = true;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Progress";
            ResumeLayout(false);
        }

        #endregion

        private ProgressBar _progressBar;
        private Label _lblStatus;
        private Button _btnCancel;
        private Button _btnHide;
    }
}
