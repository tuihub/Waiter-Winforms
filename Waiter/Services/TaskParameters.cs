using System.Text.Json.Serialization;

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

    /// <summary>
    /// Parameters for download tasks.
    /// </summary>
    public class DownloadTaskParameters : TaskParametersBase
    {
        public string AppName { get; set; } = string.Empty;
        public string DownloadUrl { get; set; } = string.Empty;
        public string DestinationPath { get; set; } = string.Empty;

        public override string GenerateTaskKey()
            => $"download:{DownloadUrl}:{DestinationPath}";
    }

    /// <summary>
    /// Parameters for save data sync tasks.
    /// </summary>
    public class SyncSaveTaskParameters : TaskParametersBase
    {
        public string AppName { get; set; } = string.Empty;
        public long AppId { get; set; }
        public string SyncDirection { get; set; } = string.Empty;

        public override string GenerateTaskKey()
            => $"syncsave:{AppId}:{SyncDirection}";
    }
}
