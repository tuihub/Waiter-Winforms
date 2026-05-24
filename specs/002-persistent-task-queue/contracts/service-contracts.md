# Service Contracts: Persistent Task Queue

**Feature Branch**: `002-persistent-task-queue`  
**Date**: 2026-01-31  
**Status**: Complete

---

## IPersistentTaskService Interface

```csharp
namespace Waiter.Services
{
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
}
```

---

## Event Arguments

```csharp
namespace Waiter.Services
{
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

    public enum PersistentTaskOperation
    {
        Created,
        Updated,
        Completed,
        Failed,
        Retried,
        Deleted
    }
}
```

---

## Exception Types

```csharp
namespace Waiter.Services
{
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
}
```

---

## PersistentTaskService Implementation Contract

```csharp
namespace Waiter.Services
{
    public class PersistentTaskService : IPersistentTaskService
    {
        private readonly WaiterDbContext _context;
        private readonly BackgroundTaskService _backgroundTaskService;
        private readonly ILogger<PersistentTaskService>? _logger;

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

        // ... implementation methods
    }
}
```

---

## DI Registration

Add to `Program.cs` or service configuration:

```csharp
// Register PersistentTaskService
services.AddSingleton<IPersistentTaskService, PersistentTaskService>();

// Or if using scoped DbContext:
services.AddScoped<IPersistentTaskService, PersistentTaskService>();
```

---

## Integration Points

### 1. BackgroundTaskService Extensions

The existing `BackgroundTaskService` needs minor extensions to support persistence:

```csharp
// Add to BackgroundTaskService
public class BackgroundTaskService
{
    // ... existing code ...

    /// <summary>
    /// Adds a task that was loaded from persistence (doesn't trigger TaskAdded).
    /// </summary>
    public void RestoreTask(BackgroundTask task)
    {
        _tasks[task.Id] = task;
        // Note: Does NOT raise TaskAdded event to avoid re-persisting
    }

    /// <summary>
    /// Gets a task by ID.
    /// </summary>
    public BackgroundTask? GetTask(string taskId)
    {
        _tasks.TryGetValue(taskId, out var task);
        return task;
    }
}
```

### 2. BackgroundTasksForm Extensions

```csharp
public partial class BackgroundTasksForm : Form
{
    private readonly BackgroundTaskService _taskService;
    private readonly IPersistentTaskService _persistentTaskService;

    public BackgroundTasksForm(
        BackgroundTaskService taskService,
        IPersistentTaskService persistentTaskService)
    {
        _taskService = taskService;
        _persistentTaskService = persistentTaskService;
        InitializeComponent();
        
        // ... existing initialization ...
    }

    private async void BtnRetry_Click(object? sender, EventArgs e)
    {
        var selectedTask = GetSelectedTask();
        if (selectedTask == null) return;

        try
        {
            var retriedTask = await _persistentTaskService.RetryTaskAsync(selectedTask.Id);
            // Task is now running, UI will update via events
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to retry task: {ex.Message}", "Retry Failed",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void UpdateRetryButtonState()
    {
        var selectedTask = GetSelectedTask();
        _btnRetry.Enabled = selectedTask != null && 
            (selectedTask.Status == TaskStatus.Failed || 
             selectedTask.Status == TaskStatus.Interrupted);
    }
}
```

### 3. Application Startup Integration

```csharp
// In Program.cs or MainForm initialization
public static async Task InitializeServicesAsync(IServiceProvider services)
{
    var persistentTaskService = services.GetRequiredService<IPersistentTaskService>();
    var backgroundTaskService = services.GetRequiredService<BackgroundTaskService>();

    // Initialize persistence and load saved tasks
    var loadedTasks = await persistentTaskService.InitializeAsync();

    // Restore tasks to in-memory service
    foreach (var task in loadedTasks)
    {
        backgroundTaskService.RestoreTask(task);
    }
}
```

---

## Error Handling Contract

| Operation | Error Condition | Behavior |
|-----------|-----------------|----------|
| PersistTaskAsync | Duplicate task key | Throw `DuplicateTaskException` |
| PersistTaskAsync | Database error | Log warning, continue (graceful degradation) |
| RetryTaskAsync | Task not found | Throw `InvalidOperationException` |
| RetryTaskAsync | Task not retryable | Throw `InvalidOperationException` |
| InitializeAsync | Database corrupted | Log warning, return empty list |
| UpdateTaskAsync | Task not persisted | Create new persisted task |

---

## Threading Model

| Method | Thread Safety | Notes |
|--------|---------------|-------|
| InitializeAsync | Call from UI thread | Async, won't block |
| PersistTaskAsync | Thread-safe | Uses DbContext locking |
| UpdateTaskAsync | Thread-safe | Uses DbContext locking |
| RetryTaskAsync | Call from UI thread | Returns task for execution |
| Event handlers | Marshal to UI thread | Use Invoke for UI updates |
