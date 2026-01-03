using Waiter.Services;
using Waiter.Helpers;

namespace Waiter.Forms
{
    /// <summary>
    /// Form for feed management and viewing feed items.
    /// </summary>
    public partial class FeedForm : Form
    {
        private readonly LibrarianClientService _clientService;

        public FeedForm(LibrarianClientService clientService)
        {
            _clientService = clientService;
            InitializeComponent();
        }

        private void BtnClose_Click(object? sender, EventArgs e)
        {
            this.Close();
        }

        private async void BtnRefresh_Click(object? sender, EventArgs e)
        {
            await LoadFeedConfigsAsync();
            await LoadFeedItemsAsync();
        }

        private async void FeedForm_Load(object? sender, EventArgs e)
        {
            await LoadFeedConfigsAsync();
            await LoadFeedItemsAsync();
        }

        private async Task LoadFeedConfigsAsync()
        {
            _lstFeedConfigs.Items.Clear();
            _statusLabel.Text = "Loading feed sources...";

            try
            {
                var response = await _clientService.ListFeedConfigsAsync(100);
                if (response != null)
                {
                    int count = 0;
                    foreach (var feedWithConfig in response.FeedsWithConfig)
                    {
                        var config = feedWithConfig.Config;
                        var item = new ListViewItem(config.Name)
                        {
                            Tag = config
                        };
                        item.SubItems.Add(config.Status.ToString());
                        _lstFeedConfigs.Items.Add(item);
                        count++;
                    }
                    _statusLabel.Text = $"Loaded {count} feed sources";
                }
            }
            catch (Exception ex)
            {
                _statusLabel.Text = $"Error: {ex.Message}";
            }
        }

        private async Task LoadFeedItemsAsync()
        {
            _lstFeedItems.Items.Clear();
            _statusLabel.Text = "Loading feed items...";

            try
            {
                var response = await _clientService.ListFeedItemsAsync(pageSize: 100);
                if (response != null)
                {
                    int count = 0;
                    foreach (var itemDigest in response.Items)
                    {
                        var listItem = new ListViewItem(itemDigest.Title)
                        {
                            Tag = itemDigest
                        };
                        listItem.SubItems.Add(itemDigest.PublishedParsedTime?.ToDateTime().ToString("g") ?? "N/A");
                        _lstFeedItems.Items.Add(listItem);
                        count++;
                    }
                    _statusLabel.Text = $"Loaded {count} feed items";
                }
            }
            catch (Exception ex)
            {
                _statusLabel.Text = $"Error: {ex.Message}";
            }
        }

        private void LstFeedItems_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_lstFeedItems.SelectedItems.Count > 0 && _lstFeedItems.SelectedItems[0].Tag is TuiHub.Protos.Librarian.Sephirah.V1.FeedItemDigest item)
            {
                _txtFeedContent.Clear();
                _txtFeedContent.SelectionFont = new Font("Segoe UI", 14, FontStyle.Bold);
                _txtFeedContent.AppendText(item.Title + "\n\n");
                _txtFeedContent.SelectionFont = new Font("Segoe UI", 10);
                _txtFeedContent.AppendText($"Published: {item.PublishedParsedTime?.ToDateTime().ToString("f") ?? "Unknown"}\n");
                _txtFeedContent.AppendText($"Source: {item.FeedConfigName}\n\n");
                _txtFeedContent.AppendText("Select an item to view its content.");
            }
        }

        private async void BtnAddConfig_Click(object? sender, EventArgs e)
        {
            using var inputForm = new Form
            {
                Text = "Add Feed Source",
                Size = new Size(450, 200),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.FromArgb(27, 40, 56)
            };

            var lblName = new Label { Text = "Name:", ForeColor = Color.White, Location = new Point(20, 20), AutoSize = true };
            var txtName = new TextBox { Location = new Point(20, 45), Size = new Size(390, 25), BackColor = Color.FromArgb(45, 60, 80), ForeColor = Color.White };
            var lblUrl = new Label { Text = "Feed URL:", ForeColor = Color.White, Location = new Point(20, 75), AutoSize = true };
            var txtUrl = new TextBox { Location = new Point(20, 100), Size = new Size(390, 25), BackColor = Color.FromArgb(45, 60, 80), ForeColor = Color.White };

            var btnOk = new Button
            {
                Text = "Create",
                Location = new Point(230, 140),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.OK
            };

            var btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(320, 140),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.Cancel
            };

            inputForm.Controls.AddRange(new Control[] { lblName, txtName, lblUrl, txtUrl, btnOk, btnCancel });
            inputForm.AcceptButton = btnOk;
            inputForm.CancelButton = btnCancel;

            if (inputForm.ShowDialog(this) == DialogResult.OK && !string.IsNullOrWhiteSpace(txtName.Text))
            {
                try
                {
                    var config = new TuiHub.Protos.Librarian.Sephirah.V1.FeedConfig
                    {
                        Name = txtName.Text.Trim(),
                        Status = TuiHub.Protos.Librarian.Sephirah.V1.FeedConfigStatus.Active
                    };
                    var id = await _clientService.CreateFeedConfigAsync(config);
                    if (id != null)
                    {
                        await LoadFeedConfigsAsync();
                        _statusLabel.Text = "Feed source created";
                    }
                    else
                    {
                        MessageBox.Show("Failed to create feed source.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
