using Waiter.Services;

namespace Waiter.Forms
{
    public partial class BackgroundTasksForm : Form
    {
        private readonly BackgroundTaskService _taskService;

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

        private void BtnClose_Click(object? sender, EventArgs e)
        {
            this.Close();
        }

        private void BackgroundTasksForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            _taskService.TaskAdded -= OnTaskChanged;
            _taskService.TaskUpdated -= OnTaskChanged;
            _taskService.TaskCompleted -= OnTaskChanged;
            _taskService.TaskFailed -= OnTaskChanged;
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
