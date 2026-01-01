using Waiter.Services;
using Waiter.Helpers;
using TuiHub.Protos.Librarian.Sephirah.V1.Sephirah;

namespace Waiter.Forms
{
    /// <summary>
    /// Main form with Steam-like layout for TuiHub client.
    /// </summary>
    public partial class MainForm : Form
    {
        private readonly LibrarianClientService _clientService;
        private readonly TokenService _tokenService;
        private readonly ConfigService _configService;
        private readonly BackgroundTaskService _taskService;

        // UI Components
        private MenuStrip _menuStrip = null!;
        private SplitContainer _mainSplitContainer = null!;
        private ListView _appListView = null!;
        private Panel _detailPanel = null!;
        private StatusStrip _statusStrip = null!;
        private ToolStripStatusLabel _statusLabel = null!;
        private ToolStripStatusLabel _userLabel = null!;
        private ToolStripStatusLabel _taskLabel = null!;

        // Detail panel components
        private Label _lblAppName = null!;
        private Label _lblAppDescription = null!;
        private Button _btnLaunch = null!;
        private Button _btnDownload = null!;
        private Button _btnSyncSave = null!;
        private PictureBox _appCoverImage = null!;

        private List<App> _apps = new();
        private App? _selectedApp;

        public MainForm(
            LibrarianClientService clientService,
            TokenService tokenService,
            ConfigService configService,
            BackgroundTaskService taskService)
        {
            _clientService = clientService;
            _tokenService = tokenService;
            _configService = configService;
            _taskService = taskService;

            InitializeComponent();

            _tokenService.TokensChanged += OnTokensChanged;
            _taskService.TaskAdded += OnTasksChanged;
            _taskService.TaskCompleted += OnTasksChanged;
        }

        private void InitializeComponent()
        {
            this.Text = "TuiHub Waiter";
            this.Size = new Size(1200, 700);
            this.MinimumSize = new Size(800, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(27, 40, 56); // Steam-like dark blue

            CreateMenuStrip();
            CreateMainLayout();
            CreateStatusStrip();

            this.Load += MainForm_Load;
            this.FormClosing += MainForm_FormClosing;
        }

        private void CreateMenuStrip()
        {
            _menuStrip = new MenuStrip
            {
                BackColor = Color.FromArgb(23, 29, 37),
                ForeColor = Color.White
            };

            // File Menu
            var fileMenu = new ToolStripMenuItem("File");
            fileMenu.DropDownItems.Add("Settings", null, (s, e) => OpenSettings());
            fileMenu.DropDownItems.Add(new ToolStripSeparator());
            fileMenu.DropDownItems.Add("Exit", null, (s, e) => Application.Exit());
            _menuStrip.Items.Add(fileMenu);

            // View Menu
            var viewMenu = new ToolStripMenuItem("View");
            viewMenu.DropDownItems.Add("Refresh Apps", null, async (s, e) => await LoadAppsAsync());
            viewMenu.DropDownItems.Add("Background Tasks", null, (s, e) => OpenBackgroundTasks());
            _menuStrip.Items.Add(viewMenu);

            // Apps Menu
            var appsMenu = new ToolStripMenuItem("Apps");
            appsMenu.DropDownItems.Add("Store", null, (s, e) => OpenStore());
            appsMenu.DropDownItems.Add("Categories", null, (s, e) => OpenCategories());
            appsMenu.DropDownItems.Add(new ToolStripSeparator());
            appsMenu.DropDownItems.Add("Add App...", null, (s, e) => OpenAddApp());
            _menuStrip.Items.Add(appsMenu);

            // Account Menu
            var accountMenu = new ToolStripMenuItem("Account");
            accountMenu.DropDownItems.Add("Login", null, (s, e) => ShowLogin());
            accountMenu.DropDownItems.Add("Logout", null, (s, e) => Logout());
            _menuStrip.Items.Add(accountMenu);

            // Help Menu
            var helpMenu = new ToolStripMenuItem("Help");
            helpMenu.DropDownItems.Add("About", null, (s, e) => ShowAbout());
            _menuStrip.Items.Add(helpMenu);

            this.MainMenuStrip = _menuStrip;
            this.Controls.Add(_menuStrip);
        }

        private void CreateMainLayout()
        {
            _mainSplitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                SplitterDistance = 300,
                BackColor = Color.FromArgb(27, 40, 56),
                Panel1MinSize = 200,
                Panel2MinSize = 400
            };
            _mainSplitContainer.Panel1.BackColor = Color.FromArgb(27, 40, 56);
            _mainSplitContainer.Panel2.BackColor = Color.FromArgb(27, 40, 56);

            // Left Panel - App List
            CreateAppListPanel();

            // Right Panel - App Details
            CreateDetailPanel();

            this.Controls.Add(_mainSplitContainer);
        }

        private void CreateAppListPanel()
        {
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.FromArgb(23, 29, 37)
            };

            var lblLibrary = new Label
            {
                Text = "LIBRARY",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(15, 15),
                AutoSize = true
            };
            headerPanel.Controls.Add(lblLibrary);

            _appListView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.LargeIcon,
                BackColor = Color.FromArgb(27, 40, 56),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.None,
                LargeImageList = new ImageList { ImageSize = new Size(64, 64), ColorDepth = ColorDepth.Depth32Bit }
            };
            _appListView.SelectedIndexChanged += AppListView_SelectedIndexChanged;
            _appListView.DoubleClick += AppListView_DoubleClick;

            _mainSplitContainer.Panel1.Controls.Add(_appListView);
            _mainSplitContainer.Panel1.Controls.Add(headerPanel);
        }

        private void CreateDetailPanel()
        {
            _detailPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(27, 40, 56)
            };

            // App Cover Image
            _appCoverImage = new PictureBox
            {
                Location = new Point(20, 20),
                Size = new Size(200, 280),
                BackColor = Color.FromArgb(45, 60, 80),
                SizeMode = PictureBoxSizeMode.Zoom
            };
            _detailPanel.Controls.Add(_appCoverImage);

            // App Name
            _lblAppName = new Label
            {
                Text = "Select an app",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(240, 20),
                Size = new Size(500, 40),
                AutoEllipsis = true
            };
            _detailPanel.Controls.Add(_lblAppName);

            // App Description
            _lblAppDescription = new Label
            {
                Text = "Select an app from your library to see details",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.LightGray,
                Location = new Point(240, 70),
                Size = new Size(500, 100)
            };
            _detailPanel.Controls.Add(_lblAppDescription);

            // Action Buttons
            _btnLaunch = new Button
            {
                Text = "LAUNCH",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(240, 180),
                Size = new Size(120, 40),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = false
            };
            _btnLaunch.Click += BtnLaunch_Click;
            _detailPanel.Controls.Add(_btnLaunch);

            _btnDownload = new Button
            {
                Text = "Download",
                Location = new Point(370, 180),
                Size = new Size(100, 40),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = false
            };
            _btnDownload.Click += BtnDownload_Click;
            _detailPanel.Controls.Add(_btnDownload);

            _btnSyncSave = new Button
            {
                Text = "Sync Save",
                Location = new Point(480, 180),
                Size = new Size(100, 40),
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = false
            };
            _btnSyncSave.Click += BtnSyncSave_Click;
            _detailPanel.Controls.Add(_btnSyncSave);

            _mainSplitContainer.Panel2.Controls.Add(_detailPanel);
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

            _taskLabel = new ToolStripStatusLabel
            {
                Text = "Tasks: 0",
                ForeColor = Color.LightGray,
                BorderSides = ToolStripStatusLabelBorderSides.Left,
                BorderStyle = Border3DStyle.Etched
            };
            _taskLabel.Click += (s, e) => OpenBackgroundTasks();

            _userLabel = new ToolStripStatusLabel
            {
                Text = "Not logged in",
                ForeColor = Color.LightGray,
                BorderSides = ToolStripStatusLabelBorderSides.Left,
                BorderStyle = Border3DStyle.Etched
            };

            _statusStrip.Items.Add(_statusLabel);
            _statusStrip.Items.Add(_taskLabel);
            _statusStrip.Items.Add(_userLabel);

            this.Controls.Add(_statusStrip);
        }

        private async void MainForm_Load(object? sender, EventArgs e)
        {
            UpdateLoginStatus();

            if (!_tokenService.IsLoggedIn)
            {
                ShowLogin();
            }

            if (_tokenService.IsLoggedIn)
            {
                await LoadAppsAsync();
                await LoadUserInfoAsync();
            }
        }

        private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            _tokenService.TokensChanged -= OnTokensChanged;
            _taskService.TaskAdded -= OnTasksChanged;
            _taskService.TaskCompleted -= OnTasksChanged;
        }

        private void OnTokensChanged(object? sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(() => OnTokensChanged(sender, e));
                return;
            }
            UpdateLoginStatus();
        }

        private void OnTasksChanged(object? sender, TaskEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(() => OnTasksChanged(sender, e));
                return;
            }
            UpdateTaskCount();
        }

        private void UpdateLoginStatus()
        {
            _userLabel.Text = _tokenService.IsLoggedIn ? "Logged in" : "Not logged in";
        }

        private void UpdateTaskCount()
        {
            var activeCount = _taskService.GetActiveTasks().Count();
            _taskLabel.Text = $"Tasks: {activeCount}";
        }

        private async Task LoadAppsAsync()
        {
            _statusLabel.Text = "Loading apps...";
            _appListView.Items.Clear();

            try
            {
                var response = await _clientService.ListAppsAsync(100);
                if (response != null)
                {
                    _apps = response.Apps.ToList();
                    foreach (var app in _apps)
                    {
                        var item = new ListViewItem(app.Name)
                        {
                            Tag = app,
                            ImageIndex = 0
                        };
                        _appListView.Items.Add(item);
                    }
                    _statusLabel.Text = $"Loaded {_apps.Count} apps";
                }
                else
                {
                    _statusLabel.Text = "Failed to load apps";
                }
            }
            catch (Exception ex)
            {
                _statusLabel.Text = $"Error: {ex.Message}";
            }
        }

        private async Task LoadUserInfoAsync()
        {
            try
            {
                var user = await _clientService.GetCurrentUserAsync();
                if (user != null)
                {
                    _userLabel.Text = $"Logged in as: {user.Username}";
                }
            }
            catch
            {
                // Ignore errors
            }
        }

        private void AppListView_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_appListView.SelectedItems.Count > 0 && _appListView.SelectedItems[0].Tag is App app)
            {
                _selectedApp = app;
                UpdateDetailPanel();
            }
        }

        private void AppListView_DoubleClick(object? sender, EventArgs e)
        {
            if (_selectedApp != null)
            {
                BtnLaunch_Click(sender, e);
            }
        }

        private void UpdateDetailPanel()
        {
            if (_selectedApp != null)
            {
                _lblAppName.Text = _selectedApp.Name;
                _lblAppDescription.Text = string.IsNullOrEmpty(_selectedApp.Description)
                    ? "No description available"
                    : _selectedApp.Description;

                _btnLaunch.Enabled = true;
                _btnDownload.Enabled = true;
                _btnSyncSave.Enabled = true;
            }
            else
            {
                _lblAppName.Text = "Select an app";
                _lblAppDescription.Text = "Select an app from your library to see details";

                _btnLaunch.Enabled = false;
                _btnDownload.Enabled = false;
                _btnSyncSave.Enabled = false;
            }
        }

        private void BtnLaunch_Click(object? sender, EventArgs e)
        {
            if (_selectedApp != null)
            {
                MessageBox.Show(
                    $"Launch functionality for '{_selectedApp.Name}' is not implemented yet.\nThis is a placeholder for future implementation.",
                    "Launch App",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private void BtnDownload_Click(object? sender, EventArgs e)
        {
            if (_selectedApp != null)
            {
                // Create a download task (stub)
                var taskId = _taskService.CreateDownloadTask(
                    _selectedApp.Name,
                    "https://example.com/download",
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), _selectedApp.Name));

                _taskService.StartTask(taskId);

                // Simulate some progress (stub implementation with proper exception handling)
                _ = SimulateDownloadAsync(taskId);

                _statusLabel.Text = $"Started download for '{_selectedApp.Name}'";
            }
        }

        private async Task SimulateDownloadAsync(string taskId)
        {
            try
            {
                for (int i = 0; i <= 100; i += 10)
                {
                    await Task.Delay(500).ConfigureAwait(false);
                    _taskService.UpdateTaskProgress(taskId, i, $"Downloading... {i}%");
                }
                _taskService.CompleteTask(taskId);
            }
            catch (Exception ex)
            {
                _taskService.FailTask(taskId, ex.Message);
            }
        }

        private void BtnSyncSave_Click(object? sender, EventArgs e)
        {
            if (_selectedApp != null)
            {
                // Create a sync save task (stub)
                var taskId = _taskService.CreateSyncSaveTask(
                    _selectedApp.Name,
                    (long)_selectedApp.Id.Id,
                    "upload");

                _taskService.StartTask(taskId);

                // Simulate completion (stub implementation with proper exception handling)
                _ = SimulateSyncSaveAsync(taskId);

                _statusLabel.Text = $"Started save sync for '{_selectedApp.Name}'";
            }
        }

        private async Task SimulateSyncSaveAsync(string taskId)
        {
            try
            {
                await Task.Delay(2000).ConfigureAwait(false);
                _taskService.UpdateTaskProgress(taskId, 100, "Sync complete");
                _taskService.CompleteTask(taskId);
            }
            catch (Exception ex)
            {
                _taskService.FailTask(taskId, ex.Message);
            }
        }

        private void ShowLogin()
        {
            using var loginForm = new LoginForm(_clientService, _tokenService, _configService);
            WindowHelper.ShowCenteredDialog(loginForm, this);

            if (loginForm.LoginSuccessful)
            {
                _ = LoadAppsAsync();
                _ = LoadUserInfoAsync();
            }
        }

        private void Logout()
        {
            var result = MessageBox.Show(
                "Are you sure you want to logout?",
                "Confirm Logout",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                _clientService.Logout();
                _apps.Clear();
                _appListView.Items.Clear();
                _selectedApp = null;
                UpdateDetailPanel();
                _statusLabel.Text = "Logged out";
            }
        }

        private void OpenSettings()
        {
            using var settingsForm = new SettingsForm(_configService, _tokenService);
            WindowHelper.ShowCenteredDialog(settingsForm, this);

            if (settingsForm.DialogResult == DialogResult.OK && !_tokenService.IsLoggedIn)
            {
                // User logged out via settings
                _apps.Clear();
                _appListView.Items.Clear();
                _selectedApp = null;
                UpdateDetailPanel();
                ShowLogin();
            }
        }

        private void OpenBackgroundTasks()
        {
            using var tasksForm = new BackgroundTasksForm(_taskService);
            WindowHelper.ShowCenteredDialog(tasksForm, this);
        }

        private void OpenStore()
        {
            MessageBox.Show(
                "Store functionality is not fully implemented yet.\nThis is a placeholder for the app store feature.",
                "Store",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void OpenCategories()
        {
            using var categoryForm = new AppCategoryForm(_clientService);
            WindowHelper.ShowCenteredDialog(categoryForm, this);
        }

        private void OpenAddApp()
        {
            MessageBox.Show(
                "Add App functionality is not fully implemented yet.\nThis is a placeholder for adding new apps.",
                "Add App",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void ShowAbout()
        {
            MessageBox.Show(
                "TuiHub Waiter\n\nA WinForms client for TuiHub\n\nVersion: 0.1.0\n\nUsing TuiHub.Protos for server communication.",
                "About TuiHub Waiter",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }
}
