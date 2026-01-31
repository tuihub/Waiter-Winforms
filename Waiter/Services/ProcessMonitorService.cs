using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Waiter.Services
{
    /// <summary>
    /// Service for monitoring process lifecycle and calculating runtime.
    /// </summary>
    public class ProcessMonitorService : IProcessMonitorService
    {
        private readonly ILogger<ProcessMonitorService> _logger;
        private const int ProcessPollIntervalMs = 500;

        public ProcessMonitorService(ILogger<ProcessMonitorService> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<ProcessResult> TrackProcessAsync(Process process, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting to track process {ProcessName} (PID: {ProcessId})",
                process.ProcessName, process.Id);

            DateTime startTime;
            try
            {
                startTime = process.StartTime;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not get process start time, using current time");
                startTime = DateTime.Now;
            }

            await process.WaitForExitAsync(cancellationToken);

            var endTime = DateTime.Now;
            var exitCode = process.ExitCode;
            var duration = endTime - startTime;

            _logger.LogInformation("Process {ProcessName} exited with code {ExitCode}, duration: {Duration}",
                process.ProcessName, exitCode, duration);

            return new ProcessResult(startTime, endTime, duration, exitCode);
        }

        /// <inheritdoc />
        public async Task<ProcessResult> TrackProcessByNameAsync(
            string processName,
            TimeSpan timeout,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Waiting for process '{ProcessName}' to appear (timeout: {Timeout})",
                processName, timeout);

            // Remove .exe extension if present
            var cleanProcessName = Path.GetFileNameWithoutExtension(processName);

            var startWait = DateTime.UtcNow;
            Process? targetProcess = null;

            // Poll for process to appear
            while (DateTime.UtcNow - startWait < timeout)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var processes = Process.GetProcessesByName(cleanProcessName);
                if (processes.Length > 0)
                {
                    targetProcess = processes[0];
                    _logger.LogInformation("Found process '{ProcessName}' (PID: {ProcessId})",
                        cleanProcessName, targetProcess.Id);

                    // Dispose other process handles
                    for (int i = 1; i < processes.Length; i++)
                    {
                        processes[i].Dispose();
                    }
                    break;
                }

                await Task.Delay(ProcessPollIntervalMs, cancellationToken);
            }

            if (targetProcess == null)
            {
                _logger.LogWarning("Process '{ProcessName}' did not appear within {Timeout}", cleanProcessName, timeout);
                throw new TimeoutException($"Process '{cleanProcessName}' did not appear within {timeout.TotalSeconds} seconds");
            }

            // Track the found process
            return await TrackProcessAsync(targetProcess, cancellationToken);
        }

        /// <inheritdoc />
        public RunningProcessInfo? FindRunningProcess(string processName)
        {
            var cleanProcessName = Path.GetFileNameWithoutExtension(processName);
            var processes = Process.GetProcessesByName(cleanProcessName);

            if (processes.Length == 0)
            {
                return null;
            }

            try
            {
                var process = processes[0];
                var info = new RunningProcessInfo(
                    process.Id,
                    process.ProcessName,
                    process.StartTime);

                // Dispose all process handles
                foreach (var p in processes)
                {
                    p.Dispose();
                }

                return info;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error getting process info for '{ProcessName}'", cleanProcessName);

                // Dispose all process handles
                foreach (var p in processes)
                {
                    p.Dispose();
                }

                return null;
            }
        }
    }
}
