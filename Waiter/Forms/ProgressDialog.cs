namespace Waiter.Forms
{
    /// <summary>
    /// Dialog for showing progress of launch and upload operations.
    /// </summary>
    public partial class ProgressDialog : Form
    {
        private readonly CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        /// Gets the cancellation token for the operation.
        /// </summary>
        public CancellationToken CancellationToken => _cancellationTokenSource.Token;

        /// <summary>
        /// Gets whether the operation was cancelled.
        /// </summary>
        public bool IsCancelled => _cancellationTokenSource.IsCancellationRequested;

        public ProgressDialog(string title)
        {
            InitializeComponent();
            Text = title;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// Updates the status message. Thread-safe.
        /// </summary>
        /// <param name="message">Status message to display</param>
        public void UpdateStatus(string message)
        {
            if (InvokeRequired)
            {
                Invoke(() => UpdateStatus(message));
                return;
            }

            _lblStatus.Text = message;
        }

        /// <summary>
        /// Updates the progress bar. Thread-safe.
        /// </summary>
        /// <param name="percentage">Progress percentage (0-100), or -1 for indeterminate</param>
        public void UpdateProgress(int percentage)
        {
            if (InvokeRequired)
            {
                Invoke(() => UpdateProgress(percentage));
                return;
            }

            if (percentage < 0)
            {
                _progressBar.Style = ProgressBarStyle.Marquee;
            }
            else
            {
                _progressBar.Style = ProgressBarStyle.Continuous;
                _progressBar.Value = Math.Min(percentage, 100);
            }
        }

        /// <summary>
        /// Closes the dialog. Thread-safe.
        /// </summary>
        public void CloseDialog()
        {
            if (InvokeRequired)
            {
                Invoke(CloseDialog);
                return;
            }

            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            _cancellationTokenSource.Cancel();
            _btnCancel.Enabled = false;
            _lblStatus.Text = "Cancelling...";
        }

        private void BtnHide_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // If user tries to close while operation is running, cancel instead
            if (!_cancellationTokenSource.IsCancellationRequested && e.CloseReason == CloseReason.UserClosing)
            {
                _cancellationTokenSource.Cancel();
                _btnCancel.Enabled = false;
                _lblStatus.Text = "Cancelling...";
                e.Cancel = true; // Prevent close until operation completes
            }

            base.OnFormClosing(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _cancellationTokenSource?.Dispose();
                components?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
