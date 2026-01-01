using Waiter.Services;
using Waiter.Helpers;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Waiter.Forms
{
    /// <summary>
    /// Form for viewing storage capacity and usage.
    /// </summary>
    public partial class StorageCapacityForm : Form
    {
        private readonly LibrarianClientService _clientService;

        // UI Components
        private Label _lblStorageInfo = null!;
        private ProgressBar _progressStorage = null!;
        private Label _lblPercentage = null!;
        private Button _btnRefresh = null!;
        private Button _btnClose = null!;

        public StorageCapacityForm(LibrarianClientService clientService)
        {
            _clientService = clientService;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Storage Capacity";
            this.Size = new Size(500, 250);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(27, 40, 56);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            CreateStorageOverview();
            CreateButtonPanel();

            this.Load += StorageCapacityForm_Load;
        }

        private void CreateStorageOverview()
        {
            var lblTitle = new Label
            {
                Text = "Storage Overview",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 20),
                AutoSize = true
            };

            _progressStorage = new ProgressBar
            {
                Location = new Point(20, 60),
                Size = new Size(440, 25),
                Style = ProgressBarStyle.Continuous,
                Maximum = 100,
                Value = 0
            };

            _lblPercentage = new Label
            {
                Text = "Loading...",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(380, 95),
                Size = new Size(80, 25),
                TextAlign = ContentAlignment.MiddleRight
            };

            _lblStorageInfo = new Label
            {
                Text = "Loading storage information...",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.LightGray,
                Location = new Point(20, 95),
                Size = new Size(350, 50)
            };

            this.Controls.AddRange(new Control[] { lblTitle, _progressStorage, _lblPercentage, _lblStorageInfo });
        }

        private void CreateButtonPanel()
        {
            var buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = Color.FromArgb(23, 29, 37)
            };

            _btnRefresh = new Button
            {
                Text = "Refresh",
                Location = new Point(20, 15),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            _btnRefresh.Click += async (s, e) => await LoadStorageDataAsync();

            _btnClose = new Button
            {
                Text = "Close",
                Location = new Point(this.Width - 130, 15),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.Right | AnchorStyles.Bottom
            };
            _btnClose.Click += (s, e) => this.Close();

            buttonPanel.Controls.AddRange(new Control[] { _btnRefresh, _btnClose });
            this.Controls.Add(buttonPanel);
        }

        private async void StorageCapacityForm_Load(object? sender, EventArgs e)
        {
            await LoadStorageDataAsync();
        }

        private async Task LoadStorageDataAsync()
        {
            _lblStorageInfo.Text = "Loading...";
            _lblPercentage.Text = "--%";
            _progressStorage.Value = 0;

            try
            {
                var response = await _clientService.GetStorageCapacityUsageAsync();
                if (response != null)
                {
                    // Show storage info - data available varies by server
                    _lblStorageInfo.Text = "Storage capacity information retrieved successfully.\n\nDetailed usage metrics depend on server configuration.";
                    _lblPercentage.Text = "Connected";
                    _progressStorage.Value = 0;
                    _progressStorage.Visible = false;
                }
                else
                {
                    _lblStorageInfo.Text = "Unable to load storage information.\n\nPlease try again later.";
                    _lblPercentage.Text = "N/A";
                }
            }
            catch (Exception ex)
            {
                _lblStorageInfo.Text = $"Error loading storage info:\n{ex.Message}";
                _lblPercentage.Text = "Error";
            }
        }
    }
}
