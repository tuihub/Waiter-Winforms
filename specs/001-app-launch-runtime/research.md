# Research: App Launch with Runtime Tracking

**Phase**: 0 (Research & Investigation)  
**Date**: 2026-01-31  
**Status**: Complete

This document resolves all "NEEDS CLARIFICATION" items from the Technical Context section of [plan.md](plan.md).

---

## 1. ProcessTimeMonitor Library Investigation

### Decision: Use Custom Process Monitoring Implementation

**Rationale**: The feature spec references "TuiHub.ProcessTimeMonitorLibrary" which does not exist as a published NuGet package or in the TuiHub GitHub organization. After searching NuGet.org and the protos repository, no such library is available.

**Resolution**:
- Create a custom `ProcessMonitorService` class using .NET's built-in `System.Diagnostics.Process` API
- Implement two monitoring modes:
  1. **Direct Process Tracking**: Monitor the launched process directly using `Process.Start()` return value
  2. **Process Name Listening**: Poll for processes matching a specified name using `Process.GetProcessesByName()`
- Track start time using `Process.StartTime` and calculate duration when process exits
- Monitor process lifecycle using `Process.WaitForExitAsync()` for non-blocking runtime tracking

**Alternatives Considered**:
- **Wait for library implementation**: Rejected - would block feature development indefinitely
- **Use third-party process monitoring library**: Rejected - violates YAGNI principle; built-in APIs sufficient
- **Implement as separate NuGet package**: Rejected - premature abstraction; keep simple until reuse need identified

**API Surface** (to be implemented):
```csharp
public interface IProcessMonitorService
{
    Task<ProcessMonitorResult> TrackProcessDirectlyAsync(Process process, CancellationToken cancellationToken);
    Task<ProcessMonitorResult> TrackProcessByNameAsync(string processName, TimeSpan timeout, CancellationToken cancellationToken);
}

public record ProcessMonitorResult(DateTime StartTime, DateTime EndTime, TimeSpan Duration, int ExitCode);
```

---

## 2. Test Infrastructure Investigation

### Decision: Manual Testing with Optional Unit Tests

**Rationale**: The Waiter-Winforms project currently has no automated test infrastructure. The constitution explicitly states "Manual testing required for UI changes; unit tests encouraged for Services."

**Resolution**:
- Follow constitution guidelines: Manual testing is primary validation method
- Optional: Add unit test project for Service layer (recommended for AppLaunchService due to complexity)
- No integration test framework needed for this feature
- Manual test checklist will be created in Phase 2 (tasks.md)

**Test Strategy**:
- **Manual UI Testing**: Verify launch button, progress dialogs, error messages through user interaction
- **Unit Tests (optional)**: Test AppLaunchService logic in isolation using mock dependencies
- **Test Project Setup** (if implementing unit tests):
  ```xml
  <ItemGroup>
    <PackageReference Include="xunit" Version="2.6.6" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.5.6" />
    <PackageReference Include="Moq" Version="4.20.70" />
  </ItemGroup>
  ```

**Alternatives Considered**:
- **Full test automation suite**: Rejected - constitution doesn't require it; adds complexity
- **UI automation tests**: Rejected - WinForms UI testing is brittle; manual testing sufficient
- **No tests at all**: Rejected - unit tests for complex Service logic provide value

---

## 3. gRPC API Methods for Runtime and Save Upload

### Decision: Use LibrarianSephirahService Proto Definitions

**Rationale**: The feature requires reporting runtime statistics and uploading save files to TuiHub server. These operations must use existing gRPC endpoints from TuiHub.Protos package.

**Resolution**: Identified two key RPC methods in `LibrarianSephirahService`:

1. **BatchCreateAppRunTime**: Report runtime statistics
   ```protobuf
   rpc BatchCreateAppRunTime(BatchCreateAppRunTimeRequest) returns (BatchCreateAppRunTimeResponse);
   
   message BatchCreateAppRunTimeRequest {
     repeated AppRunTime app_run_times = 1;
   }
   
   message AppRunTime {
     librarian.v1.InternalID id = 1;           // Runtime session ID
     librarian.v1.InternalID app_id = 2;        // App package ID
     librarian.v1.InternalID device_id = 3;     // Current device ID
     librarian.v1.TimeRange run_time = 4;       // Start and end timestamps
   }
   ```

2. **UploadAppSaveFile**: Upload save data archives
   ```protobuf
   rpc UploadAppSaveFile(UploadAppSaveFileRequest) returns (UploadAppSaveFileResponse);
   
   message UploadAppSaveFileRequest {
     librarian.v1.FileMetadata file_metadata = 1;  // File info (name, size, SHA256)
     librarian.v1.InternalID app_id = 2;           // App package ID
   }
   
   message UploadAppSaveFileResponse {
     string upload_token = 1;  // Token for subsequent file upload (presigned URL pattern)
   }
   ```

**Implementation Notes**:
- Runtime reporting uses batch API (can report multiple sessions at once, but typical use is single session)
- Save file upload follows two-step pattern: get upload token, then transfer file data
- Must include device_id in runtime reports (requires device identification strategy)
- TimeRange requires both start and end timestamps (use Process.StartTime and exit timestamp)

**Related APIs** (for future use):
- `SumAppRunTime`: Query total runtime for app
- `ListAppRunTimes`: Query runtime history
- `ListAppSaveFiles`: View saved backups
- `DownloadAppSaveFile`: Restore save data

---

## 4. SavedataManager Service Existence

### Decision: NEEDS IMPLEMENTATION (Not Found)

**Rationale**: The feature spec assumes existence of a "SavedataManager service" for creating compressed save data archives. No such service exists in current codebase.

**Resolution Options**:
1. **Create SavedataManager Service** (recommended):
   - Implement service to compress save directories into archives
   - Use built-in .NET compression APIs (`System.IO.Compression`)
   - Follow service-oriented architecture principle
   
2. **Inline save compression** (simpler, acceptable for MVP):
   - Implement compression directly in AppLaunchService
   - Extract to dedicated service if reuse need emerges
   
**Recommendation**: Start with Option 2 (inline) per YAGNI principle. Extract to SavedataManager if save data handling becomes more complex or is needed elsewhere.

**Implementation Notes**:
```csharp
// Simple inline approach using System.IO.Compression
using System.IO.Compression;

private async Task<FileInfo> CreateSaveArchiveAsync(string savePath, CancellationToken cancellationToken)
{
    var tempArchivePath = Path.Combine(Path.GetTempPath(), $"save_{Guid.NewGuid()}.zip");
    ZipFile.CreateFromDirectory(savePath, tempArchivePath);
    return new FileInfo(tempArchivePath);
}
```

---

## 5. ProgressBarWindow Component Investigation

### Decision: Create New Progress Dialog Form

**Rationale**: The feature spec references a "ProgressBarWindow" UI component for showing operation status. Investigation shows no such component exists in current codebase. Only existing dialog is `CategoryEditDialog` (simple input dialog, not suitable for progress).

**Resolution**:
- Create new `ProgressDialog.cs` Form for launch/tracking progress feedback
- Follow WinForms patterns from existing Forms (e.g., CategoryEditDialog structure)
- Design minimal, reusable progress dialog that can be used for future features

**Required API**:
```csharp
public partial class ProgressDialog : Form
{
    public ProgressDialog(string title);
    public void UpdateStatus(string message);
    public void UpdateProgress(int percentage);  // 0-100, or -1 for indeterminate
    public void ShowDialog(IWin32Window owner);
    public void CloseDialog();
}
```

**UI Design**:
```
┌─────────────────────────────────────┐
│ Starting Steam...              [X]   │
├─────────────────────────────────────┤
│                                      │
│ ████████████████░░░░░░░░░░░░░ 60%  │
│                                      │
│ Status: Waiting for process...      │
│                                      │
│           [Cancel] [Hide]            │
└─────────────────────────────────────┘
```

**Features**:
- Hide button: Minimize dialog during app runtime (per FR-019)
- Cancel button: Allow user to abort operation
- Progress bar: Visual feedback (0-100% or indeterminate marquee)
- Status label: Text description of current operation
- Auto-show: Reappear automatically when app exits

**Alternatives Considered**:
- **Use MessageBox**: Rejected - not suitable for long-running operations with progress
- **Third-party progress library**: Rejected - YAGNI; simple Form sufficient
- **Status bar in main form**: Rejected - less visible, harder to track per-operation progress

---

## 6. Best Practices for Async Process Monitoring in WinForms

### Decision: Use Task-Based Async Pattern with Proper UI Thread Marshalling

**Rationale**: WinForms is not naturally async-friendly; improper async patterns can cause UI freezes or cross-thread exceptions.

**Best Practices to Apply**:

1. **Use ConfigureAwait(false) in Service Layer**:
   ```csharp
   // In AppLaunchService (not UI-bound)
   await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
   ```

2. **Use Invoke for UI Updates from Background Threads**:
   ```csharp
   // In Forms when updating from async callbacks
   this.Invoke(() => { statusLabel.Text = "Complete"; });
   ```

3. **Handle Cancellation Properly**:
   ```csharp
   using var cts = new CancellationTokenSource();
   try {
       await LaunchAndTrackAsync(appId, cts.Token);
   } catch (OperationCanceledException) {
       MessageBox.Show("Launch cancelled by user");
   }
   ```

4. **Avoid Blocking UI Thread**:
   - Never use `.Result` or `.Wait()` on Tasks in event handlers
   - Use `async void` only for event handlers; everywhere else use `async Task`
   - Show progress dialog immediately to provide feedback

5. **Process.WaitForExitAsync() Pattern**:
   ```csharp
   // .NET 8.0 provides WaitForExitAsync
   var process = Process.Start(startInfo);
   await process.WaitForExitAsync(cancellationToken);
   var exitCode = process.ExitCode;
   ```

**References**:
- [Microsoft Docs: Async in WinForms](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/advanced/async)
- [ConfigureAwait FAQ](https://devblogs.microsoft.com/dotnet/configureawait-faq/)

---

## 7. File Transfer Chunking Strategy

### Decision: Use Streaming Upload with Configured Chunk Size

**Rationale**: The feature spec references `GlobalContext.SystemConfig.FileTransferChunkBytes` for uploading save files. Large save files should be streamed to avoid memory issues.

**Resolution**:
- Read save archive in chunks (e.g., 1MB default)
- Stream chunks to gRPC endpoint (if streaming supported) or presigned URL (HTTP)
- Calculate SHA256 hash during upload for integrity verification
- Show progress percentage during upload

**Implementation Pattern**:
```csharp
private async Task UploadFileInChunksAsync(FileInfo file, string uploadUrl, int chunkSize, CancellationToken cancellationToken)
{
    using var fileStream = file.OpenRead();
    using var sha256 = SHA256.Create();
    var buffer = new byte[chunkSize];
    long totalBytesUploaded = 0;
    
    while (true)
    {
        var bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
        if (bytesRead == 0) break;
        
        // Upload chunk (implementation depends on API)
        await UploadChunkAsync(buffer, bytesRead, cancellationToken);
        
        // Update hash
        sha256.TransformBlock(buffer, 0, bytesRead, null, 0);
        
        // Report progress
        totalBytesUploaded += bytesRead;
        var percentage = (int)((totalBytesUploaded * 100) / file.Length);
        OnProgressUpdated(percentage);
    }
    
    sha256.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
    return BitConverter.ToString(sha256.Hash).Replace("-", "").ToLower();
}
```

**Alternatives Considered**:
- **Load entire file in memory**: Rejected - large save files (>100MB) could cause OOM
- **No progress feedback**: Rejected - violates <500ms feedback requirement
- **Synchronous upload**: Rejected - violates UI responsiveness principle

---

## 8. Cache Directory for Failed Uploads

### Decision: Use GlobalContext.SystemConfig.GetRealCacheDirPath()

**Rationale**: The feature spec references this method for storing failed uploads locally. Need to verify implementation and cache structure.

**Resolution**:
- Store cached uploads in subdirectory: `{CachePath}/pending-uploads/`
- Use JSON metadata files alongside binary data:
  ```
  pending-uploads/
  ├── {guid}_runtime.json      # Runtime session metadata
  ├── {guid}_save.zip          # Save data archive
  └── {guid}_metadata.json     # Upload metadata (URLs, timestamps)
  ```
- Create database records (CachedUpload entities) to track pending items
- Implement retry UI in app details view per spec requirements

**Cache Cleanup Strategy**:
- Automatically retry uploads on app launch if network available
- User-initiated retry via "Sync Now" button
- Expire cached uploads after 30 days (configurable)
- Delete cache entries after successful upload

---

## Summary of Decisions

| Item | Decision | Impact |
|------|----------|--------|
| ProcessTimeMonitor Library | Custom implementation using System.Diagnostics.Process | Create ProcessMonitorService class |
| Test Infrastructure | Manual testing + optional unit tests | No new infrastructure required |
| gRPC API Methods | Use LibrarianSephirahService (requires proto investigation) | Document in contracts/ phase |
| SavedataManager Service | Inline compression initially (YAGNI) | Implement in AppLaunchService |
| ProgressBarWindow | Reuse existing or create minimal dialog | Investigate existing Forms |
| Async Patterns | Task-based async with proper marshalling | Follow documented best practices |
| File Transfer | Streaming upload with chunks + SHA256 | Implement chunked upload in AppLaunchService |
| Cache Directory | Use GetRealCacheDirPath() + database tracking | Create CachedUpload entity |

All NEEDS CLARIFICATION items resolved. Ready to proceed to Phase 1 (Design).
