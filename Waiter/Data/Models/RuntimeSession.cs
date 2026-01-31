namespace Waiter.Data.Models
{
    /// <summary>
    /// Records a single execution session of an application.
    /// </summary>
    public class RuntimeSession
    {
        /// <summary>
        /// Primary key.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Foreign key to AppPackage.
        /// </summary>
        public long AppPackageId { get; set; }

        /// <summary>
        /// Current device identifier (TuiHub device ID).
        /// </summary>
        public long DeviceId { get; set; }

        /// <summary>
        /// UTC timestamp when the session started.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// UTC timestamp when the session ended. Null if still running.
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// Process exit code. 0 = normal, non-zero = error.
        /// </summary>
        public int? ExitCode { get; set; }

        /// <summary>
        /// Current status of the session.
        /// </summary>
        public SessionStatus Status { get; set; } = SessionStatus.Tracking;

        /// <summary>
        /// Whether upload has been attempted (success or failure).
        /// </summary>
        public bool UploadAttempted { get; set; } = false;

        /// <summary>
        /// UTC timestamp of successful upload.
        /// </summary>
        public DateTime? UploadedAt { get; set; }

        // Computed properties (ignored by EF Core)

        /// <summary>
        /// Duration of the session. Null if session hasn't ended.
        /// </summary>
        public TimeSpan? Duration => EndTime.HasValue ? EndTime.Value - StartTime : null;

        /// <summary>
        /// Whether the process is currently running.
        /// </summary>
        public bool IsRunning => Status == SessionStatus.Tracking && !EndTime.HasValue;

        /// <summary>
        /// Whether the process exited abnormally (non-zero exit code).
        /// </summary>
        public bool IsAbnormalExit => ExitCode.HasValue && ExitCode.Value != 0;
    }
}
