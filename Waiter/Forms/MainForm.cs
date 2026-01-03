using Waiter.Services;
using Waiter.Helpers;
using TuiHub.Protos.Librarian.Sephirah.V1;

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

        private List<App> _apps = new();
        private App? _selectedApp;
        private ImageList? _appImageList;

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

            // Setup ImageList for app list
            _appImageList = new ImageList { ImageSize = new Size(64, 64), ColorDepth = ColorDepth.Depth32Bit };
            _appListView.LargeImageList = _appImageList;

            _tokenService.TokensChanged += OnTokensChanged;
            _taskService.TaskAdded += OnTasksChanged;
            _taskService.TaskCompleted += OnTasksChanged;
        }

        // Menu event handlers
        private void SettingsToolStripMenuItem_Click(object? sender, EventArgs e) => OpenSettings();
        private void StorageCapacityToolStripMenuItem_Click(object? sender, EventArgs e) => OpenStorageCapacity();
        private void ExitToolStripMenuItem_Click(object? sender, EventArgs e) => Application.Exit();
        private async void RefreshAppsToolStripMenuItem_Click(object? sender, EventArgs e) => await LoadAppsAsync();
        private void BackgroundTasksToolStripMenuItem_Click(object? sender, EventArgs e) => OpenBackgroundTasks();
        private void FeedManagerToolStripMenuItem_Click(object? sender, EventArgs e) => OpenFeedManager();
        private void NotificationsToolStripMenuItem_Click(object? sender, EventArgs e) => OpenNotifications();
        private void StoreToolStripMenuItem_Click(object? sender, EventArgs e) => OpenStore();
        private void CategoriesToolStripMenuItem_Click(object? sender, EventArgs e) => OpenCategories();
        private void AddAppToolStripMenuItem_Click(object? sender, EventArgs e) => OpenAddApp();
        private void ProfileToolStripMenuItem_Click(object? sender, EventArgs e) => OpenProfile();
        private void LoginToolStripMenuItem_Click(object? sender, EventArgs e) => ShowLogin();
        private void LogoutToolStripMenuItem_Click(object? sender, EventArgs e) => Logout();
        private void AboutToolStripMenuItem_Click(object? sender, EventArgs e) => ShowAbout();

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
                OpenAppDetails();
            }
        }

        private void OpenAppDetails()
        {
            if (_selectedApp == null) return;

            using var appDetailForm = new AppDetailForm(_clientService, _selectedApp);
            if (appDetailForm.ShowDialog(this) == DialogResult.OK)
            {
                if (appDetailForm.IsDeleted || appDetailForm.ResultApp != null)
                {
                    _ = LoadAppsAsync();
                }
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
            using var storeForm = new StoreForm(_clientService);
            WindowHelper.ShowCenteredDialog(storeForm, this);
        }

        private void OpenCategories()
        {
            using var categoryForm = new AppCategoryForm(_clientService);
            WindowHelper.ShowCenteredDialog(categoryForm, this);
        }

        private void OpenAddApp()
        {
            using var appDetailForm = new AppDetailForm(_clientService);
            if (appDetailForm.ShowDialog(this) == DialogResult.OK && appDetailForm.ResultApp != null)
            {
                _ = LoadAppsAsync();
            }
        }

        private void OpenProfile()
        {
            using var profileForm = new UserProfileForm(_clientService, _tokenService);
            WindowHelper.ShowCenteredDialog(profileForm, this);
        }

        private void OpenFeedManager()
        {
            using var feedForm = new FeedForm(_clientService);
            WindowHelper.ShowCenteredDialog(feedForm, this);
        }

        private void OpenNotifications()
        {
            using var notificationForm = new NotificationForm(_clientService);
            WindowHelper.ShowCenteredDialog(notificationForm, this);
        }

        private void OpenStorageCapacity()
        {
            using var storageForm = new StorageCapacityForm(_clientService);
            WindowHelper.ShowCenteredDialog(storageForm, this);
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
