namespace Waiter.Data.Models
{
    /// <summary>
    /// Defines how the system monitors process lifecycle.
    /// </summary>
    public enum MonitoringMode
    {
        /// <summary>
        /// Monitor the launched process directly (default, simpler).
        /// Track the process returned by Process.Start().
        /// </summary>
        DirectProcess = 0,

        /// <summary>
        /// Monitor by process name (for multi-process apps with launchers).
        /// Poll for process matching AppPackageLaunchSettings.ProcessName.
        /// </summary>
        ProcessName = 1
    }
}
