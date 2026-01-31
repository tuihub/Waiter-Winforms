# Service Contracts: App Launch with Runtime Tracking

**Phase**: 1 (Design & Contracts)  
**Date**: 2026-01-31  
**Status**: Complete

This document defines the internal service interfaces for app launch and runtime tracking, following Constitution Principle I (Service-Oriented Architecture).

---

## Service Architecture

```
┌─────────────────┐
│  AppDetailForm  │ (UI Layer)
└────────┬────────┘
         │ depends on
         ▼
┌──────────────────┐
│ AppLaunchService │ (Orchestration)
└────────┬─────────┘
         │ depends on
         ├─────────────────────┬──────────────────┬─────────────────┐
         ▼                     ▼                  ▼                 ▼
┌──────────────────┐  ┌──────────────────┐  ┌──────────────┐  ┌─────────────┐
│ ProcessMonitor   │  │ LibrarianClient  │  │ Background   │  │ Database    │
│ Service          │  │ Service          │  │ TaskService  │  │ Service     │
└──────────────────┘  └──────────────────┘  └──────────────┘  └─────────────┘
```

---

## 1. IAppLaunchService

**Purpose**: Orchestrate app launch, runtime tracking, and post-processing.

**Responsibility**: Primary service for FR-001 through FR-021. Coordinates between process monitoring, API communication, and database persistence.

### Interface Definition

```csharp
public interface IAppLaunchService
{
    /// <summary>
    /// Validates that an app package has valid launch configuration.
    /// Implements FR-016: Check executable path and working directory exist.
    /// </summary>
    /// <param name="appPackageId">App package to validate</param>
    /// <returns>Validation result with error messages if invalid</returns>
    Task<LaunchValidationResult> ValidateLaunchConfigurationAsync(long appPackageId);
    
    /// <summary>
    /// Launches an application and tracks its runtime until exit.
    /// Implements FR-001, FR-002, FR-003, FR-004, FR-005.
    /// </summary>
    /// <param name="appPackageId">App package to launch</param>
    /// <param name="progress">Progress callback for UI updates</param>
    /// <param name="cancellationToken">Cancellation token for abort</param>
    /// <returns>Runtime session with results</returns>
    Task<RuntimeSession> LaunchAndTrackAsync(
        long appPackageId, 
        IProgress<LaunchProgress> progress, 
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Uploads runtime data and save files after app exits.
    /// Implements FR-006, FR-007, FR-008, FR-012, FR-018.
    /// </summary>
    /// <param name="sessionId">Runtime session to upload</param>
    /// <param name="progress">Progress callback for UI updates</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task UploadSessionDataAsync(
        long sessionId, 
        IProgress<UploadProgress> progress, 
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Checks if target app is already running.
    /// Implements FR-020.
    /// </summary>
    /// <param name="appPackageId">App package to check</param>
    /// <returns>Running process info if found, null otherwise</returns>
    Task<RunningProcessInfo?> CheckIfRunningAsync(long appPackageId);
    
    /// <summary>
    /// Retries failed uploads from cache.
    /// Implements FR-018 manual retry.
    /// </summary>
    /// <param name="cachedUploadId">Cached upload record to retry</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task RetryUploadAsync(long cachedUploadId, CancellationToken cancellationToken);
    
    /// <summary>
    /// Cleans up expired cached uploads (>30 days old).
    /// Called automatically on app startup or manually.
    /// </summary>
    Task CleanupExpiredCacheAsync();
}
```

### Supporting Types

```csharp
public record LaunchValidationResult(
    bool IsValid,
    List<string> Errors);

public record LaunchProgress(
    LaunchPhase Phase,
    string Message,
    int? ProgressPercentage = null);

public enum LaunchPhase
{
    Validating,
    Starting,
    WaitingForProcess,
    Tracking,
    Complete
}

public record UploadProgress(
    UploadPhase Phase,
    string Message,
    int? ProgressPercentage = null);

public enum UploadPhase
{
    ReportingRuntime,
    CreatingArchive,
    CalculatingHash,
    UploadingSave,
    Complete
}

public record RunningProcessInfo(
    int ProcessId,
    string ProcessName,
    DateTime StartTime);
```

---

## 2. IProcessMonitorService

**Purpose**: Monitor process lifecycle and calculate runtime.

**Responsibility**: Implements FR-004 (two monitoring modes), FR-005 (track start/end/duration), FR-011 (launch timeout).

### Interface Definition

```csharp
public interface IProcessMonitorService
{
    /// <summary>
    /// Tracks a launched process directly until it exits.
    /// Used when MonitoringMode = DirectProcess.
    /// </summary>
    /// <param name="process">Process to monitor</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Process result with timing and exit code</returns>
    Task<ProcessResult> TrackProcessAsync(
        Process process, 
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Waits for a process with specific name to appear, then tracks it.
    /// Used when MonitoringMode = ProcessName.
    /// Implements FR-004 process listen mode.
    /// </summary>
    /// <param name="processName">Process name to monitor (e.g., "game.exe")</param>
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
```

### Supporting Types

```csharp
public record ProcessResult(
    DateTime StartTime,
    DateTime EndTime,
    TimeSpan Duration,
    int ExitCode)
{
    public bool IsNormalExit => ExitCode == 0;
    public bool IsAbnormalExit => ExitCode != 0;
}
```

---

## 3. ISaveDataService

**Purpose**: Create and manage compressed save data archives.

**Responsibility**: Implements FR-012 (create archives), supports chunked upload from research decisions.

### Interface Definition

```csharp
public interface ISaveDataService
{
    /// <summary>
    /// Creates a compressed ZIP archive of save data directory.
    /// </summary>
    /// <param name="savePath">Directory containing save files</param>
    /// <param name="outputFileName">Output archive name (optional, auto-generated if null)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>File info for created archive</returns>
    Task<FileInfo> CreateSaveArchiveAsync(
        string savePath, 
        string? outputFileName = null, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Calculates SHA256 hash of a file.
    /// Used for file integrity verification per API contract.
    /// </summary>
    /// <param name="file">File to hash</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>SHA256 hash as hex string</returns>
    Task<string> CalculateSHA256Async(
        FileInfo file, 
        CancellationToken cancellationToken);
}
```

---

## 4. Extended LibrarianClientService

**Purpose**: Add runtime and save upload methods to existing gRPC client service.

**Responsibility**: Implements API contracts defined in [api-contracts.md](../contracts/api-contracts.md).

### New Methods

```csharp
public partial class LibrarianClientService
{
    /// <summary>
    /// Reports runtime statistics to server.
    /// Calls BatchCreateAppRunTime RPC.
    /// </summary>
    /// <param name="session">Runtime session to report</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task ReportRuntimeAsync(RuntimeSession session, CancellationToken cancellationToken);
    
    /// <summary>
    /// Gets upload token for save file (step 1 of upload).
    /// Calls UploadAppSaveFile RPC.
    /// </summary>
    /// <param name="appId">App package ID</param>
    /// <param name="saveFile">Archive file info</param>
    /// <param name="sha256Hash">File SHA256 hash</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Upload token (presigned URL)</returns>
    Task<string> GetSaveFileUploadTokenAsync(
        long appId, 
        FileInfo saveFile, 
        string sha256Hash, 
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Uploads save file data using presigned URL (step 2 of upload).
    /// Uses HTTP PUT per Constitution III exception.
    /// </summary>
    /// <param name="uploadToken">Presigned URL from GetSaveFileUploadTokenAsync</param>
    /// <param name="saveFile">File to upload</param>
    /// <param name="progress">Upload progress callback</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task UploadSaveFileDataAsync(
        string uploadToken, 
        FileInfo saveFile, 
        IProgress<int> progress, 
        CancellationToken cancellationToken);
}
```

---

## 5. Extended DatabaseService

**Purpose**: Add query methods for new entities.

**Responsibility**: Database operations for AppPackageLaunchSettings, RuntimeSession, CachedUpload.

### New Methods

```csharp
public partial class DatabaseService
{
    // AppPackageLaunchSettings queries
    Task<AppPackageLaunchSettings?> GetLaunchSettingsAsync(long appPackageId);
    Task SaveLaunchSettingsAsync(AppPackageLaunchSettings settings);
    
    // RuntimeSession queries
    Task<RuntimeSession> CreateRuntimeSessionAsync(RuntimeSession session);
    Task UpdateRuntimeSessionAsync(RuntimeSession session);
    Task<RuntimeSession?> GetRuntimeSessionAsync(long sessionId);
    Task<List<RuntimeSession>> GetRecentSessionsAsync(long appPackageId, int count = 10);
    Task<TimeSpan> GetTotalRuntimeAsync(long appPackageId);
    
    // CachedUpload queries
    Task<CachedUpload> CreateCachedUploadAsync(CachedUpload upload);
    Task<List<CachedUpload>> GetPendingUploadsAsync();
    Task DeleteCachedUploadAsync(long uploadId);
    Task<List<CachedUpload>> GetExpiredCachedUploadsAsync();
}
```

---

## Dependency Injection Registration

All new services registered in `Program.cs`:

```csharp
// Register new services
services.AddSingleton<IAppLaunchService, AppLaunchService>();
services.AddSingleton<IProcessMonitorService, ProcessMonitorService>();
services.AddSingleton<ISaveDataService, SaveDataService>();

// Existing services extended (no new registrations needed)
// - LibrarianClientService (already registered)
// - DatabaseService (already registered)
// - BackgroundTaskService (already registered)
```

---

## Service Interaction Flow

### Launch and Track Workflow

```csharp
// In AppDetailForm.cs (UI Layer)
private async void LaunchButton_Click(object sender, EventArgs e)
{
    var cts = new CancellationTokenSource();
    var progress = new Progress<LaunchProgress>(p => UpdateProgressDialog(p));
    
    try
    {
        // Validate configuration
        var validation = await _appLaunchService.ValidateLaunchConfigurationAsync(_appPackageId);
        if (!validation.IsValid)
        {
            MessageBox.Show(string.Join("\n", validation.Errors), "Cannot Launch");
            return;
        }
        
        // Check if already running
        var runningProcess = await _appLaunchService.CheckIfRunningAsync(_appPackageId);
        if (runningProcess != null)
        {
            var result = MessageBox.Show(
                $"App is already running (PID: {runningProcess.ProcessId}). Launch anyway?",
                "Already Running",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);
            if (result != DialogResult.Yes) return;
        }
        
        // Launch and track
        var session = await _appLaunchService.LaunchAndTrackAsync(_appPackageId, progress, cts.Token);
        
        // Upload data
        await _appLaunchService.UploadSessionDataAsync(session.Id, uploadProgress, cts.Token);
        
        MessageBox.Show($"Session complete. Runtime: {session.Duration}", "Success");
    }
    catch (OperationCanceledException)
    {
        MessageBox.Show("Launch cancelled by user.");
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Launch failed");
        MessageBox.Show($"Launch failed: {ex.Message}", "Error");
    }
}
```

### AppLaunchService Implementation (Orchestration)

```csharp
public async Task<RuntimeSession> LaunchAndTrackAsync(
    long appPackageId, 
    IProgress<LaunchProgress> progress, 
    CancellationToken ct)
{
    // 1. Load settings
    progress.Report(new LaunchProgress(LaunchPhase.Validating, "Loading configuration..."));
    var settings = await _database.GetLaunchSettingsAsync(appPackageId);
    
    // 2. Create runtime session record
    var session = new RuntimeSession
    {
        AppPackageId = appPackageId,
        DeviceId = _config.CurrentDeviceId,
        StartTime = DateTime.UtcNow,
        Status = SessionStatus.Tracking
    };
    session = await _database.CreateRuntimeSessionAsync(session);
    
    // 3. Start process
    progress.Report(new LaunchProgress(LaunchPhase.Starting, "Starting application..."));
    var startInfo = new ProcessStartInfo
    {
        FileName = settings.ExecutablePath,
        WorkingDirectory = settings.WorkingDirectory,
        UseShellExecute = settings.UseShellExecute
    };
    var process = Process.Start(startInfo);
    
    // 4. Monitor process
    progress.Report(new LaunchProgress(LaunchPhase.Tracking, "Monitoring runtime..."));
    ProcessResult result;
    
    if (settings.MonitoringMode == MonitoringMode.DirectProcess)
    {
        result = await _processMonitor.TrackProcessAsync(process, ct);
    }
    else
    {
        var timeout = TimeSpan.FromSeconds(settings.LaunchTimeout);
        result = await _processMonitor.TrackProcessByNameAsync(
            settings.ProcessName, timeout, ct);
    }
    
    // 5. Update session
    session.EndTime = result.EndTime;
    session.ExitCode = result.ExitCode;
    session.Status = result.IsAbnormalExit ? SessionStatus.Abnormal : SessionStatus.Processing;
    await _database.UpdateRuntimeSessionAsync(session);
    
    progress.Report(new LaunchProgress(LaunchPhase.Complete, "Complete"));
    return session;
}
```

---

## Testing Strategy

### Unit Tests (Optional, Encouraged)

```csharp
[Fact]
public async Task LaunchAndTrackAsync_WithValidConfig_CreatesSession()
{
    // Arrange
    var mockProcessMonitor = new Mock<IProcessMonitorService>();
    mockProcessMonitor
        .Setup(x => x.TrackProcessAsync(It.IsAny<Process>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(new ProcessResult(DateTime.UtcNow, DateTime.UtcNow.AddMinutes(5), TimeSpan.FromMinutes(5), 0));
    
    var service = new AppLaunchService(mockProcessMonitor.Object, ...);
    
    // Act
    var session = await service.LaunchAndTrackAsync(123, null, CancellationToken.None);
    
    // Assert
    Assert.NotNull(session);
    Assert.Equal(123, session.AppPackageId);
    Assert.Equal(0, session.ExitCode);
}
```

### Manual Testing Checklist

See Phase 2 output (tasks.md) for complete manual testing scenarios per user stories in spec.md.

---

## Summary

| Service | Responsibility | New/Extended |
|---------|---------------|--------------|
| IAppLaunchService | Launch orchestration, upload coordination | NEW |
| IProcessMonitorService | Process lifecycle tracking | NEW |
| ISaveDataService | Archive creation, hash calculation | NEW |
| LibrarianClientService | gRPC API calls for runtime/save upload | EXTENDED |
| DatabaseService | Query methods for new entities | EXTENDED |

All services follow Constitution Principle I (service-oriented), registered via DI, and are testable in isolation.
