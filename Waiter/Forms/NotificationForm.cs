using Waiter.Services;
using Waiter.Helpers;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Waiter.Forms
{
    /// <summary>
    /// Form for managing notifications and notification settings.
    /// </summary>
    public partial class NotificationForm : Form
    {
        private readonly LibrarianClientService _clientService;

        // UI Components
        private ListView _lstNotifications = null!;
        private ListView _lstTargets = null!;
        private ListView _lstFlows = null!;
        private Button _btnMarkRead = null!;
        private Button _btnAddTarget = null!;
        private Button _btnAddFlow = null!;
        private Button _btnRefresh = null!;
        private Button _btnClose = null!;
        private TabControl _tabControl = null!;
        private StatusStrip _statusStrip = null!;
        private ToolStripStatusLabel _statusLabel = null!;

        public NotificationForm(LibrarianClientService clientService)
        {
            _clientService = clientService;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Notification Settings";
            this.Size = new Size(800, 550);
            this.MinimumSize = new Size(700, 450);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(27, 40, 56);

            CreateTabControl();
            CreateStatusStrip();
            CreateButtonPanel();

            this.Load += NotificationForm_Load;
        }

        private void CreateTabControl()
        {
            _tabControl = new TabControl
            {
                Dock = DockStyle.Fill
            };

            // System Notifications Tab
            var notificationsTab = new TabPage("System Notifications")
            {
                BackColor = Color.FromArgb(27, 40, 56)
            };
            CreateNotificationsTab(notificationsTab);
            _tabControl.TabPages.Add(notificationsTab);

            // Notify Targets Tab
            var targetsTab = new TabPage("Notify Targets")
            {
                BackColor = Color.FromArgb(27, 40, 56)
            };
            CreateTargetsTab(targetsTab);
            _tabControl.TabPages.Add(targetsTab);

            // Notify Flows Tab
            var flowsTab = new TabPage("Notify Flows")
            {
                BackColor = Color.FromArgb(27, 40, 56)
            };
            CreateFlowsTab(flowsTab);
            _tabControl.TabPages.Add(flowsTab);

            var tabPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10, 10, 10, 80)
            };
            tabPanel.Controls.Add(_tabControl);
            this.Controls.Add(tabPanel);
        }

        private void CreateNotificationsTab(TabPage tab)
        {
            var lblTitle = new Label
            {
                Text = "System Notifications",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 10),
                AutoSize = true
            };

            _lstNotifications = new ListView
            {
                Location = new Point(20, 45),
                Size = new Size(600, 320),
                View = View.Details,
                FullRowSelect = true,
                BackColor = Color.FromArgb(45, 60, 80),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            _lstNotifications.Columns.Add("Title", 200);
            _lstNotifications.Columns.Add("Content", 250);
            _lstNotifications.Columns.Add("Status", 100);

            _btnMarkRead = new Button
            {
                Text = "Mark Read",
                Location = new Point(630, 45),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            _btnMarkRead.Click += BtnMarkRead_Click;

            tab.Controls.AddRange(new Control[] { lblTitle, _lstNotifications, _btnMarkRead });
        }

        private void CreateTargetsTab(TabPage tab)
        {
            var lblTitle = new Label
            {
                Text = "Notification Targets",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 10),
                AutoSize = true
            };

            _lstTargets = new ListView
            {
                Location = new Point(20, 45),
                Size = new Size(600, 320),
                View = View.Details,
                FullRowSelect = true,
                BackColor = Color.FromArgb(45, 60, 80),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            _lstTargets.Columns.Add("Name", 200);
            _lstTargets.Columns.Add("Description", 250);
            _lstTargets.Columns.Add("Status", 100);

            _btnAddTarget = new Button
            {
                Text = "Add Target",
                Location = new Point(630, 45),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            _btnAddTarget.Click += BtnAddTarget_Click;

            tab.Controls.AddRange(new Control[] { lblTitle, _lstTargets, _btnAddTarget });
        }

        private void CreateFlowsTab(TabPage tab)
        {
            var lblTitle = new Label
            {
                Text = "Notification Flows",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 10),
                AutoSize = true
            };

            _lstFlows = new ListView
            {
                Location = new Point(20, 45),
                Size = new Size(600, 320),
                View = View.Details,
                FullRowSelect = true,
                BackColor = Color.FromArgb(45, 60, 80),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            _lstFlows.Columns.Add("Name", 200);
            _lstFlows.Columns.Add("Description", 250);
            _lstFlows.Columns.Add("Status", 100);

            _btnAddFlow = new Button
            {
                Text = "Add Flow",
                Location = new Point(630, 45),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            _btnAddFlow.Click += BtnAddFlow_Click;

            tab.Controls.AddRange(new Control[] { lblTitle, _lstFlows, _btnAddFlow });
        }

        private void CreateStatusStrip()
        {
            _statusStrip = new StatusStrip
            {
                BackColor = Color.FromArgb(23, 29, 37)
            };

            _statusLabel = new ToolStripStatusLabel
            {
                Text = "Ready",
                ForeColor = Color.LightGray,
                Spring = true,
                TextAlign = ContentAlignment.MiddleLeft
            };

            _statusStrip.Items.Add(_statusLabel);
            this.Controls.Add(_statusStrip);
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
            _btnRefresh.Click += async (s, e) =>
            {
                await LoadNotificationsAsync();
                await LoadTargetsAsync();
                await LoadFlowsAsync();
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

            buttonPanel.Controls.AddRange(new Control[] { _btnRefresh, _btnClose });
            this.Controls.Add(buttonPanel);
        }

        private async void NotificationForm_Load(object? sender, EventArgs e)
        {
            await LoadNotificationsAsync();
            await LoadTargetsAsync();
            await LoadFlowsAsync();
        }

        private async Task LoadNotificationsAsync()
        {
            _lstNotifications.Items.Clear();
            _statusLabel.Text = "Loading notifications...";

            try
            {
                var response = await _clientService.ListSystemNotificationsAsync(100);
                if (response != null)
                {
                    foreach (var notification in response.Notifications)
                    {
                        var item = new ListViewItem(notification.Title)
                        {
                            Tag = notification
                        };
                        item.SubItems.Add(notification.Content);
                        item.SubItems.Add(notification.Status.ToString());
                        _lstNotifications.Items.Add(item);
                    }
                    _statusLabel.Text = $"Loaded {response.Notifications.Count} notifications";
                }
            }
            catch (Exception ex)
            {
                _statusLabel.Text = $"Error: {ex.Message}";
            }
        }

        private async Task LoadTargetsAsync()
        {
            _lstTargets.Items.Clear();

            try
            {
                var response = await _clientService.ListNotifyTargetsAsync(100);
                if (response != null)
                {
                    foreach (var target in response.Targets)
                    {
                        var item = new ListViewItem(target.Name)
                        {
                            Tag = target
                        };
                        item.SubItems.Add(target.Description ?? "");
                        item.SubItems.Add(target.Status.ToString());
                        _lstTargets.Items.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading targets: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadFlowsAsync()
        {
            _lstFlows.Items.Clear();

            try
            {
                var response = await _clientService.ListNotifyFlowsAsync(100);
                if (response != null)
                {
                    foreach (var flow in response.Flows)
                    {
                        var item = new ListViewItem(flow.Name)
                        {
                            Tag = flow
                        };
                        item.SubItems.Add(flow.Description);
                        item.SubItems.Add(flow.Status.ToString());
                        _lstFlows.Items.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading flows: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnMarkRead_Click(object? sender, EventArgs e)
        {
            if (_lstNotifications.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select a notification to mark as read.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var notification = _lstNotifications.SelectedItems[0].Tag as SystemNotification;
            if (notification == null) return;

            try
            {
                var success = await _clientService.UpdateSystemNotificationAsync(notification.Id, SystemNotificationStatus.Read);
                if (success)
                {
                    await LoadNotificationsAsync();
                    _statusLabel.Text = "Notification marked as read";
                }
                else
                {
                    MessageBox.Show("Failed to update notification.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnAddTarget_Click(object? sender, EventArgs e)
        {
            using var inputForm = new Form
            {
                Text = "Add Notify Target",
                Size = new Size(400, 200),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.FromArgb(27, 40, 56)
            };

            var lblName = new Label { Text = "Name:", ForeColor = Color.White, Location = new Point(20, 20), AutoSize = true };
            var txtName = new TextBox { Location = new Point(20, 45), Size = new Size(340, 25), BackColor = Color.FromArgb(45, 60, 80), ForeColor = Color.White };
            var lblDesc = new Label { Text = "Description:", ForeColor = Color.White, Location = new Point(20, 75), AutoSize = true };
            var txtDesc = new TextBox { Location = new Point(20, 100), Size = new Size(340, 25), BackColor = Color.FromArgb(45, 60, 80), ForeColor = Color.White };

            var btnOk = new Button
            {
                Text = "Create",
                Location = new Point(180, 140),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.OK
            };

            var btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(270, 140),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.Cancel
            };

            inputForm.Controls.AddRange(new Control[] { lblName, txtName, lblDesc, txtDesc, btnOk, btnCancel });
            inputForm.AcceptButton = btnOk;
            inputForm.CancelButton = btnCancel;

            if (inputForm.ShowDialog(this) == DialogResult.OK && !string.IsNullOrWhiteSpace(txtName.Text))
            {
                try
                {
                    var target = new NotifyTarget
                    {
                        Name = txtName.Text.Trim(),
                        Description = txtDesc.Text.Trim(),
                        Status = NotifyTargetStatus.Active
                    };
                    var id = await _clientService.CreateNotifyTargetAsync(target);
                    if (id != null)
                    {
                        await LoadTargetsAsync();
                        _statusLabel.Text = "Notify target created";
                    }
                    else
                    {
                        MessageBox.Show("Failed to create notify target.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void BtnAddFlow_Click(object? sender, EventArgs e)
        {
            using var inputForm = new Form
            {
                Text = "Add Notify Flow",
                Size = new Size(400, 200),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.FromArgb(27, 40, 56)
            };

            var lblName = new Label { Text = "Name:", ForeColor = Color.White, Location = new Point(20, 20), AutoSize = true };
            var txtName = new TextBox { Location = new Point(20, 45), Size = new Size(340, 25), BackColor = Color.FromArgb(45, 60, 80), ForeColor = Color.White };
            var lblDesc = new Label { Text = "Description:", ForeColor = Color.White, Location = new Point(20, 75), AutoSize = true };
            var txtDesc = new TextBox { Location = new Point(20, 100), Size = new Size(340, 25), BackColor = Color.FromArgb(45, 60, 80), ForeColor = Color.White };

            var btnOk = new Button
            {
                Text = "Create",
                Location = new Point(180, 140),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.OK
            };

            var btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(270, 140),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.Cancel
            };

            inputForm.Controls.AddRange(new Control[] { lblName, txtName, lblDesc, txtDesc, btnOk, btnCancel });
            inputForm.AcceptButton = btnOk;
            inputForm.CancelButton = btnCancel;

            if (inputForm.ShowDialog(this) == DialogResult.OK && !string.IsNullOrWhiteSpace(txtName.Text))
            {
                try
                {
                    var flow = new NotifyFlow
                    {
                        Name = txtName.Text.Trim(),
                        Description = txtDesc.Text.Trim(),
                        Status = NotifyFlowStatus.Active
                    };
                    var id = await _clientService.CreateNotifyFlowAsync(flow);
                    if (id != null)
                    {
                        await LoadFlowsAsync();
                        _statusLabel.Text = "Notify flow created";
                    }
                    else
                    {
                        MessageBox.Show("Failed to create notify flow.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
