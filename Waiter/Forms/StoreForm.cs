using Waiter.Services;
using Waiter.Helpers;

namespace Waiter.Forms
{
    /// <summary>
    /// Store form for browsing and acquiring store apps.
    /// </summary>
    public partial class StoreForm : Form
    {
        private readonly LibrarianClientService _clientService;

        // UI Components
        private TextBox _txtSearch = null!;
        private Button _btnSearch = null!;
        private ListView _lstStoreApps = null!;
        private Panel _detailPanel = null!;
        private Label _lblAppName = null!;
        private Label _lblAppDescription = null!;
        private Button _btnAcquire = null!;
        private Button _btnClose = null!;
        private StatusStrip _statusStrip = null!;
        private ToolStripStatusLabel _statusLabel = null!;

        public StoreForm(LibrarianClientService clientService)
        {
            _clientService = clientService;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "TuiHub Store";
            this.Size = new Size(900, 550);
            this.MinimumSize = new Size(700, 450);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(27, 40, 56);

            CreateSearchPanel();
            CreateMainLayout();
            CreateStatusStrip();
        }

        private void CreateSearchPanel()
        {
            var searchPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(23, 29, 37),
                Padding = new Padding(10)
            };

            var lblSearch = new Label
            {
                Text = "Search Store:",
                ForeColor = Color.White,
                Location = new Point(10, 18),
                AutoSize = true
            };

            _txtSearch = new TextBox
            {
                Location = new Point(100, 15),
                Size = new Size(300, 25),
                BackColor = Color.FromArgb(45, 60, 80),
                ForeColor = Color.White
            };
            _txtSearch.KeyPress += (s, e) =>
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    _ = SearchAppsAsync();
                    e.Handled = true;
                }
            };

            _btnSearch = new Button
            {
                Text = "Search",
                Location = new Point(410, 13),
                Size = new Size(80, 28),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            _btnSearch.Click += async (s, e) => await SearchAppsAsync();

            searchPanel.Controls.AddRange(new Control[] { lblSearch, _txtSearch, _btnSearch });
            this.Controls.Add(searchPanel);
        }

        private void CreateMainLayout()
        {
            var splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                SplitterDistance = 400,
                BackColor = Color.FromArgb(27, 40, 56),
                Panel1MinSize = 200,
                Panel2MinSize = 250
            };
            splitContainer.Panel1.BackColor = Color.FromArgb(27, 40, 56);
            splitContainer.Panel2.BackColor = Color.FromArgb(27, 40, 56);

            // Left Panel - Store App List
            _lstStoreApps = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                BackColor = Color.FromArgb(27, 40, 56),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.None
            };
            _lstStoreApps.Columns.Add("App Name", 350);
            _lstStoreApps.SelectedIndexChanged += LstStoreApps_SelectedIndexChanged;
            splitContainer.Panel1.Controls.Add(_lstStoreApps);

            // Right Panel - App Details
            CreateDetailPanel();
            splitContainer.Panel2.Controls.Add(_detailPanel);

            this.Controls.Add(splitContainer);
        }

        private void CreateDetailPanel()
        {
            _detailPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(27, 40, 56),
                Padding = new Padding(20)
            };

            _lblAppName = new Label
            {
                Text = "Select an app",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 20),
                Size = new Size(350, 30),
                AutoEllipsis = true
            };

            _lblAppDescription = new Label
            {
                Text = "Select an app from the store to see details and acquire it.",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.LightGray,
                Location = new Point(20, 60),
                Size = new Size(350, 120)
            };

            _btnAcquire = new Button
            {
                Text = "Acquire App",
                Location = new Point(20, 200),
                Size = new Size(130, 35),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = false
            };
            _btnAcquire.Click += BtnAcquire_Click;

            _btnClose = new Button
            {
                Text = "Close",
                Location = new Point(160, 200),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            _btnClose.Click += (s, e) => this.Close();

            _detailPanel.Controls.AddRange(new Control[] {
                _lblAppName, _lblAppDescription, _btnAcquire, _btnClose
            });
        }

        private void CreateStatusStrip()
        {
            _statusStrip = new StatusStrip
            {
                BackColor = Color.FromArgb(23, 29, 37)
            };

            _statusLabel = new ToolStripStatusLabel
            {
                Text = "Enter a search term to find apps in the store",
                ForeColor = Color.LightGray,
                Spring = true,
                TextAlign = ContentAlignment.MiddleLeft
            };

            _statusStrip.Items.Add(_statusLabel);
            this.Controls.Add(_statusStrip);
        }

        private async Task SearchAppsAsync()
        {
            var searchTerm = _txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(searchTerm))
            {
                MessageBox.Show("Please enter a search term.", "Search", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _statusLabel.Text = "Searching...";
            _lstStoreApps.Items.Clear();
            _btnAcquire.Enabled = false;
            _lblAppName.Text = "Select an app";
            _lblAppDescription.Text = "Select an app from the store to see details and acquire it.";

            try
            {
                var response = await _clientService.SearchStoreAppsAsync(searchTerm, 50);
                if (response != null)
                {
                    int count = 0;
                    foreach (var appInfo in response.AppInfos)
                    {
                        var item = new ListViewItem(appInfo.Name ?? "Unknown App")
                        {
                            Tag = appInfo.Id
                        };
                        _lstStoreApps.Items.Add(item);
                        count++;
                    }
                    _statusLabel.Text = $"Found {count} apps";
                }
                else
                {
                    _statusLabel.Text = "Search failed";
                }
            }
            catch (Exception ex)
            {
                _statusLabel.Text = $"Error: {ex.Message}";
            }
        }

        private void LstStoreApps_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_lstStoreApps.SelectedItems.Count > 0)
            {
                var item = _lstStoreApps.SelectedItems[0];
                _lblAppName.Text = item.Text;
                _lblAppDescription.Text = "Click 'Acquire App' to add this app to your library.";
                _btnAcquire.Enabled = item.Tag != null;
            }
        }

        private async void BtnAcquire_Click(object? sender, EventArgs e)
        {
            if (_lstStoreApps.SelectedItems.Count == 0) return;

            var selectedItem = _lstStoreApps.SelectedItems[0];
            var appId = selectedItem.Tag as TuiHub.Protos.Librarian.V1.InternalID;
            if (appId == null) return;

            var result = MessageBox.Show(
                $"Do you want to acquire '{selectedItem.Text}'?",
                "Confirm Acquisition",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                _statusLabel.Text = "Acquiring app...";
                try
                {
                    var newAppId = await _clientService.AcquireStoreAppAsync(appId);
                    if (newAppId != null)
                    {
                        MessageBox.Show(
                            $"Successfully acquired '{selectedItem.Text}'!",
                            "Success",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        _statusLabel.Text = "App acquired successfully";
                    }
                    else
                    {
                        MessageBox.Show("Failed to acquire the app.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        _statusLabel.Text = "Acquisition failed";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _statusLabel.Text = $"Error: {ex.Message}";
                }
            }
        }
    }
}
