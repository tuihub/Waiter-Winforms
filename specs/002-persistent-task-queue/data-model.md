# Data Model: Persistent Task Queue

**Feature Branch**: `002-persistent-task-queue`  
**Date**: 2026-01-31  
**Status**: Complete

---

## Entity: PersistentTask

The `PersistentTask` entity stores all information needed to persist and retry background tasks.

### Schema Definition

```csharp
namespace Waiter.Data.Models
{
    /// <summary>
    /// Represents a background task that is persisted to the database for retry capability.
    /// </summary>
    public class PersistentTask
    {
        /// <summary>
        /// Primary key, auto-incremented.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Unique identifier matching BackgroundTask.Id.
        /// Format: "task_{counter}_{timestamp}"
        /// </summary>
        [Required]
        [MaxLength(64)]
        public string TaskId { get; set; } = string.Empty;

        /// <summary>
        /// Unique key for duplicate detection.
        /// Format: "{taskType}:{targetIdentifier}"
        /// </summary>
        [Required]
        [MaxLength(512)]
        public string TaskKey { get; set; } = string.Empty;

        /// <summary>
        /// Display name for the task (e.g., "Download: MyApp").
        /// </summary>
        [Required]
        [MaxLength(256)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Type of task: Download, SyncSave, Other.
        /// Stored as string for flexibility.
        /// </summary>
        [Required]
        [MaxLength(32)]
        public string TaskType { get; set; } = string.Empty;

        /// <summary>
        /// Current status of the task.
        /// Values: Pending, Running, Completed, Failed, Cancelled, Interrupted
        /// </summary>
        [Required]
        [MaxLength(32)]
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Last status message or error description.
        /// </summary>
        [MaxLength(1024)]
        public string? StatusMessage { get; set; }

        /// <summary>
        /// Current progress percentage (0-100).
        /// </summary>
        public double Progress { get; set; }

        /// <summary>
        /// JSON-serialized task parameters for retry.
        /// Contains all data needed to recreate the task.
        /// </summary>
        [Required]
        public string ParametersJson { get; set; } = "{}";

        /// <summary>
        /// Number of times this task has been retried.
        /// </summary>
        public int RetryCount { get; set; }

        /// <summary>
        /// When the task was originally created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// When the task was last started/retried.
        /// </summary>
        public DateTime? StartedAt { get; set; }

        /// <summary>
        /// When the task completed, failed, or was cancelled.
        /// </summary>
        public DateTime? CompletedAt { get; set; }

        /// <summary>
        /// Last time the task record was updated.
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
```

### EF Core Configuration

```csharp
namespace Waiter.Data.Configurations
{
    public class PersistentTaskConfiguration : IEntityTypeConfiguration<PersistentTask>
    {
        public void Configure(EntityTypeBuilder<PersistentTask> builder)
        {
            builder.ToTable("PersistentTasks");

            builder.HasKey(e => e.Id);

            // Unique constraint on TaskId
            builder.HasIndex(e => e.TaskId)
                .IsUnique();

            // Unique constraint on TaskKey (prevents duplicates)
            builder.HasIndex(e => e.TaskKey)
                .IsUnique();

            // Index for querying by status
            builder.HasIndex(e => e.Status);

            // Index for querying non-completed tasks on startup
            builder.HasIndex(e => new { e.Status, e.CreatedAt });
        }
    }
}
```

### WaiterDbContext Changes

Add to `WaiterDbContext.cs`:

```csharp
// Add to DbSet declarations
public DbSet<PersistentTask> PersistentTasks { get; set; }

// Add to OnModelCreating
modelBuilder.ApplyConfiguration(new PersistentTaskConfiguration());
```

---

## Task Parameters Serialization

### Base Class with Polymorphism

```csharp
namespace Waiter.Services
{
    /// <summary>
    /// Base class for task parameters with JSON polymorphism support.
    /// </summary>
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
    [JsonDerivedType(typeof(DownloadTaskParameters), "download")]
    [JsonDerivedType(typeof(SyncSaveTaskParameters), "syncsave")]
    public abstract class TaskParametersBase
    {
        /// <summary>
        /// Generate a unique key for duplicate detection.
        /// </summary>
        public abstract string GenerateTaskKey();
    }

    public class DownloadTaskParameters : TaskParametersBase
    {
        public string AppName { get; set; } = string.Empty;
        public string DownloadUrl { get; set; } = string.Empty;
        public string DestinationPath { get; set; } = string.Empty;

        public override string GenerateTaskKey()
            => $"download:{DownloadUrl}:{DestinationPath}";
    }

    public class SyncSaveTaskParameters : TaskParametersBase
    {
        public string AppName { get; set; } = string.Empty;
        public long AppId { get; set; }
        public string SyncDirection { get; set; } = string.Empty;

        public override string GenerateTaskKey()
            => $"syncsave:{AppId}:{SyncDirection}";
    }
}
```

### Serialization Example

```csharp
// Serialization
var parameters = new DownloadTaskParameters
{
    AppName = "MyGame",
    DownloadUrl = "https://example.com/game.zip",
    DestinationPath = "C:\\Games\\MyGame"
};
string json = JsonSerializer.Serialize<TaskParametersBase>(parameters);
// Result: {"$type":"download","AppName":"MyGame","DownloadUrl":"https://example.com/game.zip","DestinationPath":"C:\\Games\\MyGame"}

// Deserialization
var restored = JsonSerializer.Deserialize<TaskParametersBase>(json);
// restored is DownloadTaskParameters
```

---

## Status Enum Extension

Add `Interrupted` status to existing enum in `BackgroundTaskService.cs`:

```csharp
public enum TaskStatus
{
    Pending,
    Running,
    Completed,
    Failed,
    Cancelled,
    Interrupted  // NEW: Task was running when app closed unexpectedly
}
```

---

## Database Queries

### Load Retryable Tasks on Startup

```csharp
// Load all tasks that need attention (not completed/cancelled)
var retryableTasks = await _context.PersistentTasks
    .Where(t => t.Status != "Completed" && t.Status != "Cancelled")
    .OrderByDescending(t => t.CreatedAt)
    .ToListAsync();
```

### Check for Duplicate Task

```csharp
// Check if task with same key already exists and is not completed
var existingTask = await _context.PersistentTasks
    .FirstOrDefaultAsync(t => t.TaskKey == taskKey 
        && t.Status != "Completed" 
        && t.Status != "Cancelled");

if (existingTask != null)
{
    throw new DuplicateTaskException($"Task already exists: {existingTask.Name}");
}
```

### Mark Interrupted Tasks on Startup

```csharp
// Find tasks that were running when app closed
var runningTasks = await _context.PersistentTasks
    .Where(t => t.Status == "Running")
    .ToListAsync();

foreach (var task in runningTasks)
{
    task.Status = "Interrupted";
    task.StatusMessage = "Task was interrupted by application shutdown";
    task.UpdatedAt = DateTime.UtcNow;
}

await _context.SaveChangesAsync();
```

### Clear Completed Tasks

```csharp
// Remove all completed and cancelled tasks
var toRemove = await _context.PersistentTasks
    .Where(t => t.Status == "Completed" || t.Status == "Cancelled")
    .ToListAsync();

_context.PersistentTasks.RemoveRange(toRemove);
await _context.SaveChangesAsync();
```

---

## Relationships

The `PersistentTask` entity is standalone and does not have foreign key relationships to other entities. It stores all necessary information (including serialized parameters) to be self-contained.

**Design Decision**: Task parameters are stored as JSON rather than separate related entities because:
1. Different task types have different parameter shapes
2. Parameters are read-only after creation (no need for relational updates)
3. Simpler schema with single table
4. Easier migration when task types change

---

## Validation Rules

| Field | Validation |
|-------|------------|
| TaskId | Required, unique, max 64 chars |
| TaskKey | Required, unique, max 512 chars |
| Name | Required, max 256 chars |
| TaskType | Required, one of: Download, SyncSave, Other |
| Status | Required, one of: Pending, Running, Completed, Failed, Cancelled, Interrupted |
| ParametersJson | Required, valid JSON |
| Progress | 0-100 |
| RetryCount | >= 0 |

---

## State Transitions

```text
Valid Transitions:
├── Pending → Running (task started)
├── Running → Completed (success)
├── Running → Failed (error)
├── Running → Cancelled (user cancelled)
├── Running → Interrupted (app shutdown detection on restart)
├── Failed → Pending (retry requested)
├── Interrupted → Pending (retry requested)
└── Any → (deleted) (clear completed/failed)

Invalid Transitions:
├── Completed → Any (terminal state)
├── Cancelled → Any (terminal state, unless explicitly deleted)
└── Pending/Running → Failed/Interrupted (must go through Running first)
```
