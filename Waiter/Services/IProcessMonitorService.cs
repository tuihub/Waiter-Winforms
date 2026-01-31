using System.Diagnostics;

namespace Waiter.Services
{
    /// <summary>
    /// Interface for monitoring process lifecycle and calculating runtime.
    /// </summary>
    public interface IProcessMonitorService
    {
        /// <summary>
        /// Tracks a launched process directly until it exits.
        /// Used when MonitoringMode = DirectProcess.
        /// </summary>
        /// <param name="process">Process to monitor</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Process result with timing and exit code</returns>
        Task<ProcessResult> TrackProcessAsync(Process process, CancellationToken cancellationToken);

        /// <summary>
        /// Waits for a process with specific name to appear, then tracks it.
        /// Used when MonitoringMode = ProcessName.
        /// </summary>
        /// <param name="processName">Process name to monitor (without .exe extension)</param>
        /// <param name="timeout">Maximum time to wait for process to appear</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Process result with timing and exit code</returns>
        Task<ProcessResult> TrackProcessByNameAsync(
            string processName,
            TimeSpan timeout,
            CancellationToken cancellationToken);

        /// <summary>
        /// Checks if a process with given name is currently running.
        /// </summary>
        /// <param name="processName">Process name to check</param>
        /// <returns>Process info if running, null otherwise</returns>
        RunningProcessInfo? FindRunningProcess(string processName);
    }

    /// <summary>
    /// Result of tracking a process.
    /// </summary>
    public record ProcessResult(
        DateTime StartTime,
        DateTime EndTime,
        TimeSpan Duration,
        int ExitCode)
    {
        /// <summary>
        /// Whether the process exited normally (exit code 0).
        /// </summary>
        public bool IsNormalExit => ExitCode == 0;

        /// <summary>
        /// Whether the process exited abnormally (non-zero exit code).
        /// </summary>
        public bool IsAbnormalExit => ExitCode != 0;
    }

    /// <summary>
    /// Information about a running process.
    /// </summary>
    public record RunningProcessInfo(
        int ProcessId,
        string ProcessName,
        DateTime StartTime);
}
