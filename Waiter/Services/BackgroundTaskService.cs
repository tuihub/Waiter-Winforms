using System.Collections.Concurrent;

namespace Waiter.Services
{
    /// <summary>
    /// Background task manager for handling long-running operations like downloads and save file syncs.
    /// </summary>
    public class BackgroundTaskService
    {
        private readonly ConcurrentDictionary<string, BackgroundTask> _tasks = new();
        private int _taskIdCounter = 0;

        public event EventHandler<TaskEventArgs>? TaskAdded;
        public event EventHandler<TaskEventArgs>? TaskUpdated;
        public event EventHandler<TaskEventArgs>? TaskCompleted;
        public event EventHandler<TaskEventArgs>? TaskFailed;

        public IEnumerable<BackgroundTask> GetAllTasks() => _tasks.Values;

        public IEnumerable<BackgroundTask> GetActiveTasks() =>
            _tasks.Values.Where(t => t.Status == TaskStatus.Running || t.Status == TaskStatus.Pending);

        public string CreateDownloadTask(string appName, string downloadUrl, string destinationPath)
        {
            var taskId = GenerateTaskId();
            var task = new BackgroundTask
            {
                Id = taskId,
                Name = $"Download: {appName}",
                Type = BackgroundTaskType.Download,
                Status = TaskStatus.Pending,
                Progress = 0,
                Details = new DownloadTaskDetails
                {
                    AppName = appName,
                    DownloadUrl = downloadUrl,
                    DestinationPath = destinationPath
                }
            };

            _tasks[taskId] = task;
            TaskAdded?.Invoke(this, new TaskEventArgs(task));
            return taskId;
        }

        public string CreateSyncSaveTask(string appName, long appId, string syncDirection)
        {
            var taskId = GenerateTaskId();
            var task = new BackgroundTask
            {
                Id = taskId,
                Name = $"Sync Save: {appName} ({syncDirection})",
                Type = BackgroundTaskType.SyncSave,
                Status = TaskStatus.Pending,
                Progress = 0,
                Details = new SyncSaveTaskDetails
                {
                    AppName = appName,
                    AppId = appId,
                    SyncDirection = syncDirection
                }
            };

            _tasks[taskId] = task;
            TaskAdded?.Invoke(this, new TaskEventArgs(task));
            return taskId;
        }

        public void StartTask(string taskId)
        {
            if (_tasks.TryGetValue(taskId, out var task))
            {
                task.Status = TaskStatus.Running;
                task.StartTime = DateTime.Now;
                TaskUpdated?.Invoke(this, new TaskEventArgs(task));
            }
        }

        public void UpdateTaskProgress(string taskId, double progress, string? message = null)
        {
            if (_tasks.TryGetValue(taskId, out var task))
            {
                task.Progress = progress;
                if (message != null)
                {
                    task.StatusMessage = message;
                }
                TaskUpdated?.Invoke(this, new TaskEventArgs(task));
            }
        }

        public void CompleteTask(string taskId)
        {
            if (_tasks.TryGetValue(taskId, out var task))
            {
                task.Status = TaskStatus.Completed;
                task.Progress = 100;
                task.EndTime = DateTime.Now;
                TaskCompleted?.Invoke(this, new TaskEventArgs(task));
            }
        }

        public void FailTask(string taskId, string errorMessage)
        {
            if (_tasks.TryGetValue(taskId, out var task))
            {
                task.Status = TaskStatus.Failed;
                task.StatusMessage = errorMessage;
                task.EndTime = DateTime.Now;
                TaskFailed?.Invoke(this, new TaskEventArgs(task));
            }
        }

        public void CancelTask(string taskId)
        {
            if (_tasks.TryGetValue(taskId, out var task))
            {
                task.Status = TaskStatus.Cancelled;
                task.EndTime = DateTime.Now;
                TaskUpdated?.Invoke(this, new TaskEventArgs(task));
            }
        }

        public bool RemoveTask(string taskId)
        {
            return _tasks.TryRemove(taskId, out _);
        }

        private string GenerateTaskId()
        {
            return $"task_{Interlocked.Increment(ref _taskIdCounter)}_{DateTime.Now:yyyyMMddHHmmss}";
        }
    }

    public class BackgroundTask
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public BackgroundTaskType Type { get; set; }
        public TaskStatus Status { get; set; }
        public double Progress { get; set; }
        public string StatusMessage { get; set; } = string.Empty;
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public object? Details { get; set; }
    }

    public enum BackgroundTaskType
    {
        Download,
        SyncSave,
        Other
    }

    public enum TaskStatus
    {
        Pending,
        Running,
        Completed,
        Failed,
        Cancelled
    }

    public class DownloadTaskDetails
    {
        public string AppName { get; set; } = string.Empty;
        public string DownloadUrl { get; set; } = string.Empty;
        public string DestinationPath { get; set; } = string.Empty;
        public long TotalBytes { get; set; }
        public long DownloadedBytes { get; set; }
    }

    public class SyncSaveTaskDetails
    {
        public string AppName { get; set; } = string.Empty;
        public long AppId { get; set; }
        public string SyncDirection { get; set; } = string.Empty; // "upload" or "download"
    }

    public class TaskEventArgs : EventArgs
    {
        public BackgroundTask Task { get; }

        public TaskEventArgs(BackgroundTask task)
        {
            Task = task;
        }
    }
}
