using System.ComponentModel.DataAnnotations;

namespace Waiter.Data.Models
{
    /// <summary>
    /// Stores local configuration for launching an app package.
    /// </summary>
    public class AppPackageLaunchSettings
    {
        /// <summary>
        /// Primary key.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Foreign key to AppPackage. One-to-one relationship.
        /// </summary>
        public long AppPackageId { get; set; }

        /// <summary>
        /// Path to the executable file. Must exist on disk before launch.
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string ExecutablePath { get; set; } = string.Empty;

        /// <summary>
        /// Working directory for the process. Must exist on disk before launch.
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string WorkingDirectory { get; set; } = string.Empty;

        /// <summary>
        /// How to monitor the process lifecycle.
        /// DirectProcess: Track the launched process directly.
        /// ProcessName: Poll for a process with specific name.
        /// </summary>
        public MonitoringMode MonitoringMode { get; set; } = MonitoringMode.DirectProcess;

        /// <summary>
        /// Process name to monitor when MonitoringMode = ProcessName.
        /// Required when using ProcessName mode.
        /// </summary>
        [MaxLength(100)]
        public string? ProcessName { get; set; }

        /// <summary>
        /// Whether to use shell execute for process start.
        /// </summary>
        public bool UseShellExecute { get; set; } = false;

        /// <summary>
        /// Timeout in seconds for process to appear (ProcessName mode).
        /// Range: 5-300 seconds. Default: 30 seconds.
        /// </summary>
        [Range(5, 300)]
        public int LaunchTimeout { get; set; } = 30;

        /// <summary>
        /// Optional directory path for save data backup.
        /// </summary>
        [MaxLength(500)]
        public string? SaveDataPath { get; set; }
    }
}
