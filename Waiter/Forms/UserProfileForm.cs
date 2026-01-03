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

        public UserProfileForm(LibrarianClientService clientService, TokenService tokenService)
        {
            _clientService = clientService;
            _tokenService = tokenService;
            InitializeComponent();
        }

        private void BtnClose_Click(object? sender, EventArgs e)
        {
            this.Close();
        }

        private async void BtnRefresh_Click(object? sender, EventArgs e)
        {
            await LoadUserInfoAsync();
            await LoadSessionsAsync();
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
