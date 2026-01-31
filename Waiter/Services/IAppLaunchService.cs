using Waiter.Data.Models;

namespace Waiter.Services
{
    /// <summary>
    /// Interface for orchestrating app launch, runtime tracking, and post-processing.
    /// </summary>
    public interface IAppLaunchService
    {
        /// <summary>
        /// Validates that an app package has valid launch configuration.
        /// Checks executable path and working directory exist.
        /// </summary>
        /// <param name="appPackageId">App package to validate</param>
        /// <returns>Validation result with error messages if invalid</returns>
        Task<LaunchValidationResult> ValidateLaunchConfigurationAsync(long appPackageId);

        /// <summary>
        /// Launches an application and tracks its runtime until exit.
        /// </summary>
        /// <param name="appPackageId">App package to launch</param>
        /// <param name="progress">Progress callback for UI updates</param>
        /// <param name="cancellationToken">Cancellation token for abort</param>
        /// <returns>Runtime session with results</returns>
        Task<RuntimeSession> LaunchAndTrackAsync(
            long appPackageId,
            IProgress<LaunchProgress>? progress,
            CancellationToken cancellationToken);

        /// <summary>
        /// Uploads runtime data and save files after app exits.
        /// </summary>
        /// <param name="sessionId">Runtime session to upload</param>
        /// <param name="progress">Progress callback for UI updates</param>
        /// <param name="cancellationToken">Cancellation token</param>
        Task UploadSessionDataAsync(
            long sessionId,
            IProgress<UploadProgress>? progress,
            CancellationToken cancellationToken);

        /// <summary>
        /// Checks if target app is already running.
        /// </summary>
        /// <param name="appPackageId">App package to check</param>
        /// <returns>Running process info if found, null otherwise</returns>
        Task<RunningProcessInfo?> CheckIfRunningAsync(long appPackageId);

        /// <summary>
        /// Retries failed uploads from cache.
        /// </summary>
        /// <param name="cachedUploadId">Cached upload record to retry</param>
        /// <param name="cancellationToken">Cancellation token</param>
        Task RetryUploadAsync(long cachedUploadId, CancellationToken cancellationToken);

        /// <summary>
        /// Cleans up expired cached uploads (>30 days old).
        /// </summary>
        Task CleanupExpiredCacheAsync();
    }

    /// <summary>
    /// Result of launch configuration validation.
    /// </summary>
    public record LaunchValidationResult(
        bool IsValid,
        List<string> Errors);

    /// <summary>
    /// Progress update during app launch and tracking.
    /// </summary>
    public record LaunchProgress(
        LaunchPhase Phase,
        string Message,
        int? ProgressPercentage = null);

    /// <summary>
    /// Phases of app launch process.
    /// </summary>
    public enum LaunchPhase
    {
        Validating,
        Starting,
        WaitingForProcess,
        Tracking,
        Complete
    }

    /// <summary>
    /// Progress update during upload.
    /// </summary>
    public record UploadProgress(
        UploadPhase Phase,
        string Message,
        int? ProgressPercentage = null);

    /// <summary>
    /// Phases of upload process.
    /// </summary>
    public enum UploadPhase
    {
        ReportingRuntime,
        CreatingArchive,
        CalculatingHash,
        UploadingSave,
        Complete
    }
}
