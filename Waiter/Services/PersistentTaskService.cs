using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Waiter.Data;
using Waiter.Data.Models;

namespace Waiter.Services
{
    #region Exception Types

    /// <summary>
    /// Thrown when attempting to create a task that already exists.
    /// </summary>
    public class DuplicateTaskException : Exception
    {
        public string TaskKey { get; }
        public string ExistingTaskId { get; }

        public DuplicateTaskException(string taskKey, string existingTaskId, string message)
            : base(message)
        {
            TaskKey = taskKey;
            ExistingTaskId = existingTaskId;
        }
    }

    #endregion

    #region Event Args

    /// <summary>
    /// Event arguments for persistent task operations.
    /// </summary>
    public class PersistentTaskEventArgs : EventArgs
    {
        public PersistentTask Task { get; }
        public PersistentTaskOperation Operation { get; }

        public PersistentTaskEventArgs(PersistentTask task, PersistentTaskOperation operation)
        {
            Task = task;
            Operation = operation;
        }
    }

    /// <summary>
    /// Types of persistent task operations.
    /// </summary>
    public enum PersistentTaskOperation
    {
        Created,
        Updated,
        Completed,
        Failed,
        Retried,
        Deleted
    }

    #endregion

    #region Interface

    /// <summary>
    /// Service for persisting background tasks to the database for retry capability.
    /// </summary>
    public interface IPersistentTaskService
    {
        /// <summary>
        /// Initializes the service, marks interrupted tasks, and loads persisted tasks.
        /// Should be called during application startup.
        /// </summary>
        /// <returns>List of tasks that were loaded and need attention (failed/interrupted).</returns>
        Task<List<BackgroundTask>> InitializeAsync();

        /// <summary>
        /// Persists a new task to the database.
        /// </summary>
        /// <param name="task">The background task to persist.</param>
        /// <param name="parameters">The task parameters for retry.</param>
        /// <exception cref="DuplicateTaskException">Thrown when a task with the same key already exists.</exception>
        Task PersistTaskAsync(BackgroundTask task, TaskParametersBase parameters);

        /// <summary>
        /// Updates the persisted state of a task.
        /// </summary>
        /// <param name="task">The task with updated state.</param>
        Task UpdateTaskAsync(BackgroundTask task);

        /// <summary>
        /// Marks a task as completed and updates the database.
        /// </summary>
        /// <param name="taskId">The ID of the task to complete.</param>
        Task CompleteTaskAsync(string taskId);

        /// <summary>
        /// Marks a task as failed and updates the database.
        /// </summary>
        /// <param name="taskId">The ID of the task to fail.</param>
        /// <param name="errorMessage">The error message describing the failure.</param>
        Task FailTaskAsync(string taskId, string errorMessage);

        /// <summary>
        /// Retries a failed or interrupted task.
        /// </summary>
        /// <param name="taskId">The ID of the task to retry.</param>
        /// <returns>The recreated BackgroundTask ready for execution.</returns>
        /// <exception cref="InvalidOperationException">Thrown when task cannot be retried.</exception>
        Task<BackgroundTask> RetryTaskAsync(string taskId);

        /// <summary>
        /// Checks if a task with the given key already exists and is active.
        /// </summary>
        /// <param name="taskKey">The unique task key.</param>
        /// <returns>True if a duplicate active task exists.</returns>
        Task<bool> IsDuplicateTaskAsync(string taskKey);

        /// <summary>
        /// Removes all completed and cancelled tasks from the database.
        /// </summary>
        /// <returns>Number of tasks removed.</returns>
        Task<int> ClearCompletedTasksAsync();

        /// <summary>
        /// Removes all failed tasks from the database.
        /// </summary>
        /// <returns>Number of tasks removed.</returns>
        Task<int> ClearFailedTasksAsync();

        /// <summary>
        /// Gets all persisted tasks (for debugging/testing).
        /// </summary>
        Task<List<PersistentTask>> GetAllPersistedTasksAsync();

        /// <summary>
        /// Event raised when a task is persisted or updated.
        /// </summary>
        event EventHandler<PersistentTaskEventArgs>? TaskPersisted;
    }

    #endregion

    #region Implementation

    /// <summary>
    /// Implementation of IPersistentTaskService for persisting background tasks to SQLite.
    /// </summary>
    public class PersistentTaskService : IPersistentTaskService
    {
        private readonly WaiterDbContext _context;
        private readonly BackgroundTaskService _backgroundTaskService;
        private readonly ILogger<PersistentTaskService>? _logger;
        private readonly object _dbLock = new();
        private DateTime _lastUpdateTime = DateTime.MinValue;
        private readonly TimeSpan _updateThrottle = TimeSpan.FromMilliseconds(500);

        public event EventHandler<PersistentTaskEventArgs>? TaskPersisted;

        public PersistentTaskService(
            WaiterDbContext context,
            BackgroundTaskService backgroundTaskService,
            ILogger<PersistentTaskService>? logger = null)
        {
            _context = context;
            _backgroundTaskService = backgroundTaskService;
            _logger = logger;

            // Subscribe to BackgroundTaskService events for automatic persistence
            _backgroundTaskService.TaskAdded += OnBackgroundTaskAdded;
            _backgroundTaskService.TaskUpdated += OnBackgroundTaskUpdated;
            _backgroundTaskService.TaskCompleted += OnBackgroundTaskCompleted;
            _backgroundTaskService.TaskFailed += OnBackgroundTaskFailed;
        }

        #region Initialization

        /// <inheritdoc />
        public async Task<List<BackgroundTask>> InitializeAsync()
        {
            try
            {
                _logger?.LogInformation("Initializing PersistentTaskService...");

                // Mark any tasks that were "Running" as "Interrupted"
                await MarkInterruptedTasksAsync();

                // Load all non-completed tasks
                var persistedTasks = await _context.PersistentTasks
                    .Where(t => t.Status != nameof(TaskStatus.Completed) && t.Status != nameof(TaskStatus.Cancelled))
                    .OrderByDescending(t => t.CreatedAt)
                    .ToListAsync();

                var backgroundTasks = new List<BackgroundTask>();

                foreach (var pt in persistedTasks)
                {
                    var task = ConvertToBackgroundTask(pt);
                    if (task != null)
                    {
                        backgroundTasks.Add(task);
                    }
                }

                _logger?.LogInformation("Loaded {Count} persisted tasks", backgroundTasks.Count);
                return backgroundTasks;
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Failed to initialize PersistentTaskService, continuing with in-memory only");
                return new List<BackgroundTask>();
            }
        }

        private async Task MarkInterruptedTasksAsync()
        {
            try
            {
                var runningTasks = await _context.PersistentTasks
                    .Where(t => t.Status == nameof(TaskStatus.Running))
                    .ToListAsync();

                foreach (var task in runningTasks)
                {
                    task.Status = nameof(TaskStatus.Interrupted);
                    task.StatusMessage = "Task was interrupted by application shutdown";
                    task.UpdatedAt = DateTime.UtcNow;
                }

                if (runningTasks.Count > 0)
                {
                    await _context.SaveChangesAsync();
                    _logger?.LogInformation("Marked {Count} tasks as Interrupted", runningTasks.Count);
                }
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Failed to mark interrupted tasks");
            }
        }

        #endregion

        #region Core Persistence

        /// <inheritdoc />
        public async Task PersistTaskAsync(BackgroundTask task, TaskParametersBase parameters)
        {
            try
            {
                var taskKey = parameters.GenerateTaskKey();

                // Check for duplicates
                if (await IsDuplicateTaskAsync(taskKey))
                {
                    var existing = await _context.PersistentTasks
                        .FirstOrDefaultAsync(t => t.TaskKey == taskKey &&
                            t.Status != nameof(TaskStatus.Completed) &&
                            t.Status != nameof(TaskStatus.Cancelled));

                    throw new DuplicateTaskException(
                        taskKey,
                        existing?.TaskId ?? "unknown",
                        $"A task with the same target already exists: {existing?.Name ?? taskKey}");
                }

                var persistentTask = new PersistentTask
                {
                    TaskId = task.Id,
                    TaskKey = taskKey,
                    Name = task.Name,
                    TaskType = task.Type.ToString(),
                    Status = task.Status.ToString(),
                    StatusMessage = task.StatusMessage,
                    Progress = task.Progress,
                    ParametersJson = JsonSerializer.Serialize(parameters, parameters.GetType()),
                    RetryCount = 0,
                    CreatedAt = DateTime.UtcNow,
                    StartedAt = task.StartTime,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.PersistentTasks.Add(persistentTask);
                await _context.SaveChangesAsync();

                _logger?.LogInformation("Persisted task: {TaskId} - {Name}", task.Id, task.Name);
                TaskPersisted?.Invoke(this, new PersistentTaskEventArgs(persistentTask, PersistentTaskOperation.Created));
            }
            catch (DuplicateTaskException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Failed to persist task: {TaskId}", task.Id);
                // Graceful degradation - don't throw, allow task to continue without persistence
            }
        }

        /// <inheritdoc />
        public async Task<bool> IsDuplicateTaskAsync(string taskKey)
        {
            try
            {
                return await _context.PersistentTasks
                    .AnyAsync(t => t.TaskKey == taskKey &&
                        t.Status != nameof(TaskStatus.Completed) &&
                        t.Status != nameof(TaskStatus.Cancelled));
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Failed to check for duplicate task: {TaskKey}", taskKey);
                return false;
            }
        }

        /// <inheritdoc />
        public async Task<List<PersistentTask>> GetAllPersistedTasksAsync()
        {
            try
            {
                return await _context.PersistentTasks
                    .OrderByDescending(t => t.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Failed to get all persisted tasks");
                return new List<PersistentTask>();
            }
        }

        #endregion

        #region Status Updates

        /// <inheritdoc />
        public async Task UpdateTaskAsync(BackgroundTask task)
        {
            try
            {
                var persistedTask = await _context.PersistentTasks
                    .FirstOrDefaultAsync(t => t.TaskId == task.Id);

                if (persistedTask == null)
                {
                    _logger?.LogDebug("Task not found for update: {TaskId}", task.Id);
                    return;
                }

                persistedTask.Status = task.Status.ToString();
                persistedTask.StatusMessage = task.StatusMessage;
                persistedTask.Progress = task.Progress;
                persistedTask.UpdatedAt = DateTime.UtcNow;

                if (task.Status == TaskStatus.Running && persistedTask.StartedAt == null)
                {
                    persistedTask.StartedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
                TaskPersisted?.Invoke(this, new PersistentTaskEventArgs(persistedTask, PersistentTaskOperation.Updated));
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Failed to update task: {TaskId}", task.Id);
            }
        }

        /// <inheritdoc />
        public async Task CompleteTaskAsync(string taskId)
        {
            try
            {
                var persistedTask = await _context.PersistentTasks
                    .FirstOrDefaultAsync(t => t.TaskId == taskId);

                if (persistedTask == null)
                {
                    _logger?.LogDebug("Task not found for completion: {TaskId}", taskId);
                    return;
                }

                persistedTask.Status = nameof(TaskStatus.Completed);
                persistedTask.Progress = 100;
                persistedTask.CompletedAt = DateTime.UtcNow;
                persistedTask.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger?.LogInformation("Completed task: {TaskId}", taskId);
                TaskPersisted?.Invoke(this, new PersistentTaskEventArgs(persistedTask, PersistentTaskOperation.Completed));
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Failed to complete task: {TaskId}", taskId);
            }
        }

        /// <inheritdoc />
        public async Task FailTaskAsync(string taskId, string errorMessage)
        {
            try
            {
                var persistedTask = await _context.PersistentTasks
                    .FirstOrDefaultAsync(t => t.TaskId == taskId);

                if (persistedTask == null)
                {
                    _logger?.LogDebug("Task not found for failure: {TaskId}", taskId);
                    return;
                }

                persistedTask.Status = nameof(TaskStatus.Failed);
                persistedTask.StatusMessage = errorMessage;
                persistedTask.CompletedAt = DateTime.UtcNow;
                persistedTask.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger?.LogInformation("Failed task: {TaskId} - {Error}", taskId, errorMessage);
                TaskPersisted?.Invoke(this, new PersistentTaskEventArgs(persistedTask, PersistentTaskOperation.Failed));
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Failed to mark task as failed: {TaskId}", taskId);
            }
        }

        #endregion

        #region Retry

        /// <inheritdoc />
        public async Task<BackgroundTask> RetryTaskAsync(string taskId)
        {
            var persistedTask = await _context.PersistentTasks
                .FirstOrDefaultAsync(t => t.TaskId == taskId);

            if (persistedTask == null)
            {
                throw new InvalidOperationException($"Task not found: {taskId}");
            }

            if (persistedTask.Status != nameof(TaskStatus.Failed) &&
                persistedTask.Status != nameof(TaskStatus.Interrupted))
            {
                throw new InvalidOperationException($"Task cannot be retried (status: {persistedTask.Status})");
            }

            // Deserialize parameters
            var parameters = JsonSerializer.Deserialize<TaskParametersBase>(persistedTask.ParametersJson);
            if (parameters == null)
            {
                throw new InvalidOperationException($"Failed to deserialize task parameters for: {taskId}");
            }

            // Create new background task based on type
            BackgroundTask newTask;
            string newTaskId;

            if (parameters is DownloadTaskParameters downloadParams)
            {
                newTaskId = _backgroundTaskService.CreateDownloadTask(
                    downloadParams.AppName,
                    downloadParams.DownloadUrl,
                    downloadParams.DestinationPath);
                newTask = _backgroundTaskService.GetTask(newTaskId)!;
            }
            else if (parameters is SyncSaveTaskParameters syncParams)
            {
                newTaskId = _backgroundTaskService.CreateSyncSaveTask(
                    syncParams.AppName,
                    syncParams.AppId,
                    syncParams.SyncDirection);
                newTask = _backgroundTaskService.GetTask(newTaskId)!;
            }
            else
            {
                throw new InvalidOperationException($"Unknown task parameter type: {parameters.GetType().Name}");
            }

            // Update the persisted task with new ID and reset status
            persistedTask.TaskId = newTaskId;
            persistedTask.Status = nameof(TaskStatus.Pending);
            persistedTask.StatusMessage = string.Empty;
            persistedTask.Progress = 0;
            persistedTask.RetryCount++;
            persistedTask.StartedAt = null;
            persistedTask.CompletedAt = null;
            persistedTask.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger?.LogInformation("Retried task: {OldTaskId} -> {NewTaskId} (retry #{RetryCount})",
                taskId, newTaskId, persistedTask.RetryCount);
            TaskPersisted?.Invoke(this, new PersistentTaskEventArgs(persistedTask, PersistentTaskOperation.Retried));

            return newTask;
        }

        #endregion

        #region Clear Operations

        /// <inheritdoc />
        public async Task<int> ClearCompletedTasksAsync()
        {
            try
            {
                var toRemove = await _context.PersistentTasks
                    .Where(t => t.Status == nameof(TaskStatus.Completed) || t.Status == nameof(TaskStatus.Cancelled))
                    .ToListAsync();

                _context.PersistentTasks.RemoveRange(toRemove);
                await _context.SaveChangesAsync();

                _logger?.LogInformation("Cleared {Count} completed/cancelled tasks", toRemove.Count);
                return toRemove.Count;
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Failed to clear completed tasks");
                return 0;
            }
        }

        /// <inheritdoc />
        public async Task<int> ClearFailedTasksAsync()
        {
            try
            {
                var toRemove = await _context.PersistentTasks
                    .Where(t => t.Status == nameof(TaskStatus.Failed))
                    .ToListAsync();

                _context.PersistentTasks.RemoveRange(toRemove);
                await _context.SaveChangesAsync();

                _logger?.LogInformation("Cleared {Count} failed tasks", toRemove.Count);
                return toRemove.Count;
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Failed to clear failed tasks");
                return 0;
            }
        }

        #endregion

        #region Event Handlers

        private async void OnBackgroundTaskAdded(object? sender, TaskEventArgs e)
        {
            // Note: Tasks are persisted explicitly when created with parameters
            // This handler is for tasks that might be created without persistence
            _logger?.LogDebug("Task added: {TaskId}", e.Task.Id);
        }

        private async void OnBackgroundTaskUpdated(object? sender, TaskEventArgs e)
        {
            // Throttle updates to avoid excessive database writes
            var now = DateTime.UtcNow;
            if (now - _lastUpdateTime < _updateThrottle)
            {
                return;
            }
            _lastUpdateTime = now;

            await UpdateTaskAsync(e.Task);
        }

        private async void OnBackgroundTaskCompleted(object? sender, TaskEventArgs e)
        {
            await CompleteTaskAsync(e.Task.Id);
        }

        private async void OnBackgroundTaskFailed(object? sender, TaskEventArgs e)
        {
            await FailTaskAsync(e.Task.Id, e.Task.StatusMessage);
        }

        #endregion

        #region Helper Methods

        private BackgroundTask? ConvertToBackgroundTask(PersistentTask pt)
        {
            try
            {
                var status = Enum.TryParse<TaskStatus>(pt.Status, out var s) ? s : TaskStatus.Failed;
                var taskType = Enum.TryParse<BackgroundTaskType>(pt.TaskType, out var t) ? t : BackgroundTaskType.Other;

                // Deserialize the parameters to get details
                object? details = null;
                try
                {
                    var parameters = JsonSerializer.Deserialize<TaskParametersBase>(pt.ParametersJson);
                    if (parameters is DownloadTaskParameters dp)
                    {
                        details = new DownloadTaskDetails
                        {
                            AppName = dp.AppName,
                            DownloadUrl = dp.DownloadUrl,
                            DestinationPath = dp.DestinationPath
                        };
                    }
                    else if (parameters is SyncSaveTaskParameters sp)
                    {
                        details = new SyncSaveTaskDetails
                        {
                            AppName = sp.AppName,
                            AppId = sp.AppId,
                            SyncDirection = sp.SyncDirection
                        };
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogWarning(ex, "Failed to deserialize parameters for task: {TaskId}", pt.TaskId);
                }

                return new BackgroundTask
                {
                    Id = pt.TaskId,
                    Name = pt.Name,
                    Type = taskType,
                    Status = status,
                    Progress = pt.Progress,
                    StatusMessage = pt.StatusMessage ?? string.Empty,
                    StartTime = pt.StartedAt,
                    EndTime = pt.CompletedAt,
                    Details = details
                };
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Failed to convert PersistentTask to BackgroundTask: {TaskId}", pt.TaskId);
                return null;
            }
        }

        #endregion
    }

    #endregion
}
