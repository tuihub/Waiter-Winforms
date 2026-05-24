using Waiter.Services;
using Waiter.Data.Models;

namespace Waiter.Forms
{
    public partial class BackgroundTasksForm : Form
    {
        private readonly BackgroundTaskService _taskService;
        private readonly IPersistentTaskService _persistentTaskService;

        public BackgroundTasksForm(BackgroundTaskService taskService, IPersistentTaskService persistentTaskService)
        {
            _taskService = taskService;
            _persistentTaskService = persistentTaskService;
            InitializeComponent();
            LoadTasks();

            // Subscribe to task events
            _taskService.TaskAdded += OnTaskChanged;
            _taskService.TaskUpdated += OnTaskChanged;
            _taskService.TaskCompleted += OnTaskChanged;
            _taskService.TaskFailed += OnTaskChanged;

            // Update retry button state initially
            UpdateRetryButtonState();
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
                item.SubItems.Add(task.StartTime?.ToString("g") ?? "-");
                item.SubItems.Add(task.EndTime?.ToString("g") ?? "-");
                item.SubItems.Add("-"); // Retry count - will be updated from persisted task
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
            item.SubItems[5].Text = task.StartTime?.ToString("g") ?? "-";
            item.SubItems[6].Text = task.EndTime?.ToString("g") ?? "-";
            // item.SubItems[7] is retry count, updated separately
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
                Services.TaskStatus.Interrupted => Color.Orange,
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

        private void ListViewTasks_SelectedIndexChanged(object? sender, EventArgs e)
        {
            UpdateRetryButtonState();
        }

        private void UpdateRetryButtonState()
        {
            var selectedTask = GetSelectedTask();
            _btnRetry.Enabled = selectedTask != null &&
                (selectedTask.Status == Services.TaskStatus.Failed ||
                 selectedTask.Status == Services.TaskStatus.Interrupted);
        }

        private BackgroundTask? GetSelectedTask()
        {
            if (_listViewTasks.SelectedItems.Count == 0)
                return null;

            var taskId = _listViewTasks.SelectedItems[0].Tag as string;
            if (string.IsNullOrEmpty(taskId))
                return null;

            return _taskService.GetTask(taskId);
        }

        private async void BtnRetry_Click(object? sender, EventArgs e)
        {
            var selectedTask = GetSelectedTask();
            if (selectedTask == null)
                return;

            try
            {
                _btnRetry.Enabled = false;
                var retriedTask = await _persistentTaskService.RetryTaskAsync(selectedTask.Id);

                // Remove old item and add new one
                var oldItem = _listViewTasks.SelectedItems[0];
                _listViewTasks.Items.Remove(oldItem);
                AddTaskToListView(retriedTask);

                UpdateStatus();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show($"Cannot retry task: {ex.Message}", "Retry Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to retry task: {ex.Message}", "Retry Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                UpdateRetryButtonState();
            }
        }

        private async void BtnClear_Click(object? sender, EventArgs e)
        {
            var completedTasks = _taskService.GetAllTasks()
                .Where(t => t.Status == Services.TaskStatus.Completed ||
                           t.Status == Services.TaskStatus.Failed ||
                           t.Status == Services.TaskStatus.Cancelled)
                .ToList();

            if (completedTasks.Count == 0)
            {
                MessageBox.Show("No completed, failed, or cancelled tasks to clear.", "Clear Tasks",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var result = MessageBox.Show(
                $"Are you sure you want to clear {completedTasks.Count} completed/failed/cancelled tasks?\n\nThis will also remove them from the persistent task history.",
                "Clear Tasks",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            // Clear from persistent storage
            await _persistentTaskService.ClearCompletedTasksAsync();
            await _persistentTaskService.ClearFailedTasksAsync();

            // Clear from in-memory service
            foreach (var task in completedTasks)
            {
                _taskService.RemoveTask(task.Id);
            }

            LoadTasks();
        }
    }
}
