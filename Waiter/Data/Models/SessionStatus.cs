namespace Waiter.Data.Models
{
    /// <summary>
    /// Status of a runtime session lifecycle.
    /// </summary>
    public enum SessionStatus
    {
        /// <summary>
        /// Process is running, tracking runtime.
        /// </summary>
        Tracking = 0,

        /// <summary>
        /// App exited normally, uploading data.
        /// </summary>
        Processing = 1,

        /// <summary>
        /// Successfully uploaded runtime and save data.
        /// </summary>
        Completed = 2,

        /// <summary>
        /// Upload failed, data cached locally.
        /// </summary>
        Failed = 3,

        /// <summary>
        /// User declined upload after abnormal exit.
        /// </summary>
        Skipped = 4,

        /// <summary>
        /// Awaiting user confirmation after abnormal exit.
        /// </summary>
        Abnormal = 5
    }
}
