using System.ComponentModel.DataAnnotations;

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
