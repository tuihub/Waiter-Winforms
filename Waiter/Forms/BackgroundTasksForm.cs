using Waiter.Services;

namespace Waiter.Forms
{
    public partial class BackgroundTasksForm : Form
    {
        private readonly BackgroundTaskService _taskService;

        private ListView _listViewTasks = null!;
        private Button _btnClear = null!;
        private Button _btnClose = null!;
        private Label _lblStatus = null!;

        public BackgroundTasksForm(BackgroundTaskService taskService)
        {
            _taskService = taskService;
            InitializeComponent();
            LoadTasks();

            // Subscribe to task events
            _taskService.TaskAdded += OnTaskChanged;
            _taskService.TaskUpdated += OnTaskChanged;
            _taskService.TaskCompleted += OnTaskChanged;
            _taskService.TaskFailed += OnTaskChanged;
        }

        private void InitializeComponent()
        {
            this.Text = "Background Tasks";
            this.Size = new Size(700, 400);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(30, 30, 30);

            // Title
            var lblTitle = new Label
            {
                Text = "Background Tasks",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 15),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);

            // ListView
            _listViewTasks = new ListView
            {
                Location = new Point(20, 55),
                Size = new Size(640, 250),
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                BackColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.White
            };
            _listViewTasks.Columns.Add("Name", 200);
            _listViewTasks.Columns.Add("Type", 80);
            _listViewTasks.Columns.Add("Status", 80);
            _listViewTasks.Columns.Add("Progress", 80);
            _listViewTasks.Columns.Add("Message", 180);
            this.Controls.Add(_listViewTasks);

            // Status Label
            _lblStatus = new Label
            {
                Text = "",
                ForeColor = Color.LightGray,
                Location = new Point(20, 315),
                Size = new Size(400, 20)
            };
            this.Controls.Add(_lblStatus);

            // Buttons
            _btnClear = new Button
            {
                Text = "Clear Completed",
                Location = new Point(450, 315),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            _btnClear.Click += BtnClear_Click;
            this.Controls.Add(_btnClear);

            _btnClose = new Button
            {
                Text = "Close",
                Location = new Point(560, 315),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            _btnClose.Click += (s, e) => this.Close();
            this.Controls.Add(_btnClose);

            this.FormClosing += (s, e) =>
            {
                _taskService.TaskAdded -= OnTaskChanged;
                _taskService.TaskUpdated -= OnTaskChanged;
                _taskService.TaskCompleted -= OnTaskChanged;
                _taskService.TaskFailed -= OnTaskChanged;
            };
        }

        private void LoadTasks()
        {
            _listViewTasks.Items.Clear();

            foreach (var task in _taskService.GetAllTasks())
            {
                AddTaskToListView(task);
            }

            UpdateStatus();
        }

        private void AddTaskToListView(BackgroundTask task)
        {
            var existingItem = _listViewTasks.Items.Cast<ListViewItem>()
                .FirstOrDefault(i => i.Tag is string id && id == task.Id);

            if (existingItem != null)
            {
                UpdateListViewItem(existingItem, task);
            }
            else
            {
                var item = new ListViewItem(task.Name);
                item.SubItems.Add(task.Type.ToString());
                item.SubItems.Add(task.Status.ToString());
                item.SubItems.Add($"{task.Progress:F0}%");
                item.SubItems.Add(task.StatusMessage);
                item.Tag = task.Id;
                SetItemColor(item, task.Status);
                _listViewTasks.Items.Add(item);
            }
        }

        private void UpdateListViewItem(ListViewItem item, BackgroundTask task)
        {
            item.SubItems[0].Text = task.Name;
            item.SubItems[1].Text = task.Type.ToString();
            item.SubItems[2].Text = task.Status.ToString();
            item.SubItems[3].Text = $"{task.Progress:F0}%";
            item.SubItems[4].Text = task.StatusMessage;
            SetItemColor(item, task.Status);
        }

        private void SetItemColor(ListViewItem item, Services.TaskStatus status)
        {
            item.ForeColor = status switch
            {
                Services.TaskStatus.Running => Color.LightBlue,
                Services.TaskStatus.Completed => Color.LimeGreen,
                Services.TaskStatus.Failed => Color.OrangeRed,
                Services.TaskStatus.Cancelled => Color.Gray,
                _ => Color.White
            };
        }

        private void OnTaskChanged(object? sender, TaskEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(() => OnTaskChanged(sender, e));
                return;
            }

            AddTaskToListView(e.Task);
            UpdateStatus();
        }

        private void UpdateStatus()
        {
            var activeTasks = _taskService.GetActiveTasks().Count();
            _lblStatus.Text = $"Active tasks: {activeTasks}";
        }

        private void BtnClear_Click(object? sender, EventArgs e)
        {
            var completedTasks = _taskService.GetAllTasks()
                .Where(t => t.Status == Services.TaskStatus.Completed ||
                           t.Status == Services.TaskStatus.Failed ||
                           t.Status == Services.TaskStatus.Cancelled)
                .ToList();

            foreach (var task in completedTasks)
            {
                _taskService.RemoveTask(task.Id);
            }

            LoadTasks();
        }
    }
}
