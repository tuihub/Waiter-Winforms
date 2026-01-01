using Waiter.Services;
using Waiter.Helpers;

namespace Waiter.Forms
{
    /// <summary>
    /// Form for user profile and session management.
    /// </summary>
    public partial class UserProfileForm : Form
    {
        private readonly LibrarianClientService _clientService;
        private readonly TokenService _tokenService;

        // UI Components
        private Label _lblUsername = null!;
        private Label _lblUserId = null!;
        private Label _lblStatus = null!;
        private ListView _lstSessions = null!;
        private Button _btnDeleteSession = null!;
        private Button _btnRefresh = null!;
        private Button _btnClose = null!;

        public UserProfileForm(LibrarianClientService clientService, TokenService tokenService)
        {
            _clientService = clientService;
            _tokenService = tokenService;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "User Profile";
            this.Size = new Size(600, 500);
            this.MinimumSize = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(27, 40, 56);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            CreateProfileSection();
            CreateSessionsSection();
            CreateButtonPanel();

            this.Load += UserProfileForm_Load;
        }

        private void CreateProfileSection()
        {
            var lblTitle = new Label
            {
                Text = "User Information",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 20),
                AutoSize = true
            };

            _lblUsername = new Label
            {
                Text = "Username: Loading...",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.LightGray,
                Location = new Point(20, 55),
                Size = new Size(400, 25)
            };

            _lblUserId = new Label
            {
                Text = "User ID: Loading...",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.LightGray,
                Location = new Point(20, 85),
                Size = new Size(400, 25)
            };

            _lblStatus = new Label
            {
                Text = "Status: Loading...",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.LightGray,
                Location = new Point(20, 115),
                Size = new Size(400, 25)
            };

            this.Controls.AddRange(new Control[] {
                lblTitle, _lblUsername, _lblUserId, _lblStatus
            });
        }

        private void CreateSessionsSection()
        {
            var lblSessions = new Label
            {
                Text = "Active Sessions",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 160),
                AutoSize = true
            };

            _lstSessions = new ListView
            {
                Location = new Point(20, 190),
                Size = new Size(440, 200),
                View = View.Details,
                FullRowSelect = true,
                BackColor = Color.FromArgb(45, 60, 80),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            _lstSessions.Columns.Add("Device", 200);
            _lstSessions.Columns.Add("Session ID", 200);

            _btnDeleteSession = new Button
            {
                Text = "End Session",
                Location = new Point(470, 190),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(200, 50, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            _btnDeleteSession.Click += BtnDeleteSession_Click;

            _btnRefresh = new Button
            {
                Text = "Refresh",
                Location = new Point(470, 230),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            _btnRefresh.Click += async (s, e) =>
            {
                await LoadUserInfoAsync();
                await LoadSessionsAsync();
            };

            this.Controls.AddRange(new Control[] { lblSessions, _lstSessions, _btnDeleteSession, _btnRefresh });
        }

        private void CreateButtonPanel()
        {
            var buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = Color.FromArgb(23, 29, 37)
            };

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

            buttonPanel.Controls.Add(_btnClose);
            this.Controls.Add(buttonPanel);
        }

        private async void UserProfileForm_Load(object? sender, EventArgs e)
        {
            await LoadUserInfoAsync();
            await LoadSessionsAsync();
        }

        private async Task LoadUserInfoAsync()
        {
            try
            {
                var user = await _clientService.GetCurrentUserAsync();
                if (user != null)
                {
                    _lblUsername.Text = $"Username: {user.Username}";
                    _lblUserId.Text = $"User ID: {user.Id?.Id}";
                    _lblStatus.Text = $"Status: {user.Status}";
                }
                else
                {
                    _lblUsername.Text = "Username: Error loading";
                    _lblUserId.Text = "User ID: N/A";
                    _lblStatus.Text = "Status: N/A";
                }
            }
            catch (Exception ex)
            {
                _lblUsername.Text = $"Error: {ex.Message}";
            }
        }

        private async Task LoadSessionsAsync()
        {
            _lstSessions.Items.Clear();
            try
            {
                var response = await _clientService.ListUserSessionsAsync();
                if (response != null)
                {
                    foreach (var session in response.Sessions)
                    {
                        var item = new ListViewItem(session.DeviceInfo?.DeviceName ?? "Unknown Device")
                        {
                            Tag = session
                        };
                        item.SubItems.Add(session.Id?.Id.ToString() ?? "N/A");
                        _lstSessions.Items.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading sessions: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnDeleteSession_Click(object? sender, EventArgs e)
        {
            if (_lstSessions.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select a session to end.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var session = _lstSessions.SelectedItems[0].Tag as TuiHub.Protos.Librarian.Sephirah.V1.UserSession;
            if (session == null) return;

            var result = MessageBox.Show(
                "Are you sure you want to end this session?",
                "Confirm",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    var success = await _clientService.DeleteUserSessionAsync(session.Id);
                    if (success)
                    {
                        await LoadSessionsAsync();
                        MessageBox.Show("Session ended successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Failed to end session.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
