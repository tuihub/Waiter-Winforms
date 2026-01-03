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

        public StoreForm(LibrarianClientService clientService)
        {
            _clientService = clientService;
            InitializeComponent();
        }

        private void BtnClose_Click(object? sender, EventArgs e)
        {
            this.Close();
        }

        private void TxtSearch_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                _ = SearchAppsAsync();
                e.Handled = true;
            }
        }

        private async void BtnSearch_Click(object? sender, EventArgs e)
        {
            await SearchAppsAsync();
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
