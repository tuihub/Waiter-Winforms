namespace Waiter.Data.Models
{
    /// <summary>
    /// Type of cached upload for retry.
    /// </summary>
    public enum UploadType
    {
        /// <summary>
        /// Runtime statistics (BatchCreateAppRunTime API).
        /// </summary>
        RuntimeData = 0,

        /// <summary>
        /// Save data archive (UploadAppSaveFile API).
        /// </summary>
        SaveFile = 1
    }
}
