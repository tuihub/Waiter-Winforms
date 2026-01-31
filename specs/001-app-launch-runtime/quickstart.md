# Quick Start: App Launch with Runtime Tracking

**Phase**: 1 (Design & Contracts)  
**Date**: 2026-01-31  
**For**: Developers implementing the feature

This guide provides a step-by-step roadmap for implementing the app launch and runtime tracking feature.

---

## Prerequisites

- Waiter-Winforms solution open in Visual Studio 2022
- .NET 8.0 SDK installed
- TuiHub.Protos NuGet package v0.6.2+ (already referenced)
- SQLite database operational (ApplicationDbContext working)
- Feature branch `001-app-launch-runtime` checked out

---

## Implementation Sequence

### Phase 1: Database Layer (1-2 hours)

#### 1.1 Create Entity Classes

**Location**: `Waiter/Data/Models/`

**Files to create**:
- `AppPackageLaunchSettings.cs`
- `RuntimeSession.cs`
- `CachedUpload.cs`
- `MonitoringMode.cs` (enum)
- `SessionStatus.cs` (enum)
- `UploadType.cs` (enum)

**Reference**: See [data-model.md](data-model.md) for complete entity definitions

**Verification**:
```bash
dotnet build
# Should compile without errors
```

---

#### 1.2 Add EF Core Configurations

**Location**: `Waiter/Data/Configurations/` (create if not exists)

**Files to create**:
- `AppPackageLaunchSettingsConfiguration.cs`
- `RuntimeSessionConfiguration.cs`
- `CachedUploadConfiguration.cs`

**Update**: `Waiter/Data/WaiterDbContext.cs`
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);
    
    // Apply new configurations
    modelBuilder.ApplyConfiguration(new AppPackageLaunchSettingsConfiguration());
    modelBuilder.ApplyConfiguration(new RuntimeSessionConfiguration());
    modelBuilder.ApplyConfiguration(new CachedUploadConfiguration());
}
```

---

#### 1.3 Create EF Migration

**Commands**:
```bash
cd Waiter
dotnet ef migrations add AddAppLaunchTables
dotnet ef database update
```

**Verification**:
- Check `Waiter/Migrations/` for new migration file
- Open SQLite database and verify tables created:
  - `AppPackageLaunchSettings`
  - `RuntimeSessions`
  - `CachedUploads`

---

#### 1.4 Extend DatabaseService

**Location**: `Waiter/Data/DatabaseService.cs`

**Add methods** from [service-contracts.md](contracts/service-contracts.md):
```csharp
// AppPackageLaunchSettings queries
public async Task<AppPackageLaunchSettings?> GetLaunchSettingsAsync(long appPackageId)
{
    return await _context.AppPackageLaunchSettings
        .FirstOrDefaultAsync(x => x.AppPackageId == appPackageId);
}

public async Task SaveLaunchSettingsAsync(AppPackageLaunchSettings settings)
{
    if (settings.Id == 0)
        _context.AppPackageLaunchSettings.Add(settings);
    else
        _context.AppPackageLaunchSettings.Update(settings);
    
    await _context.SaveChangesAsync();
}

// RuntimeSession queries
// ... (add remaining methods from contract)
```

**Verification**:
```bash
dotnet build
# No compilation errors
```

---

### Phase 2: Service Layer (3-4 hours)

#### 2.1 Create ProcessMonitorService

**Location**: `Waiter/Services/ProcessMonitorService.cs`

**Implementation**:
```csharp
public class ProcessMonitorService : IProcessMonitorService
{
    private readonly ILogger<ProcessMonitorService> _logger;
    
    public ProcessMonitorService(ILogger<ProcessMonitorService> logger)
    {
        _logger = logger;
    }
    
    public async Task<ProcessResult> TrackProcessAsync(
        Process process, 
        CancellationToken ct)
    {
        var startTime = process.StartTime;
        await process.WaitForExitAsync(ct);
        var endTime = DateTime.Now;
        var exitCode = process.ExitCode;
        
        return new ProcessResult(
            startTime, 
            endTime, 
            endTime - startTime, 
            exitCode);
    }
    
    public async Task<ProcessResult> TrackProcessByNameAsync(
        string processName, 
        TimeSpan timeout, 
        CancellationToken ct)
    {
        // Implementation: Poll for process appearance
        // See research.md for pattern
    }
    
    public RunningProcessInfo? FindRunningProcess(string processName)
    {
        var processes = Process.GetProcessesByName(
            Path.GetFileNameWithoutExtension(processName));
        
        if (processes.Length == 0) return null;
        
        var process = processes[0];
        return new RunningProcessInfo(
            process.Id, 
            process.ProcessName, 
            process.StartTime);
    }
}
```

**Interface**: Create `IProcessMonitorService.cs` in same directory (extract interface)

---

#### 2.2 Create SaveDataService

**Location**: `Waiter/Services/SaveDataService.cs`

**Implementation**:
```csharp
public class SaveDataService : ISaveDataService
{
    public async Task<FileInfo> CreateSaveArchiveAsync(
        string savePath, 
        string? outputFileName = null, 
        CancellationToken ct = default)
    {
        outputFileName ??= $"save_{DateTime.UtcNow:yyyyMMdd_HHmmss}.zip";
        var tempPath = Path.Combine(Path.GetTempPath(), outputFileName);
        
        // Use System.IO.Compression
        await Task.Run(() => 
            ZipFile.CreateFromDirectory(savePath, tempPath), ct);
        
        return new FileInfo(tempPath);
    }
    
    public async Task<string> CalculateSHA256Async(
        FileInfo file, 
        CancellationToken ct)
    {
        using var sha256 = SHA256.Create();
        using var stream = file.OpenRead();
        
        var hashBytes = await sha256.ComputeHashAsync(stream, ct);
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }
}
```

---

#### 2.3 Extend LibrarianClientService

**Location**: `Waiter/Services/LibrarianClientService.cs`

**Add methods** from [api-contracts.md](contracts/api-contracts.md):
```csharp
public async Task ReportRuntimeAsync(
    RuntimeSession session, 
    CancellationToken ct)
{
    var request = new BatchCreateAppRunTimeRequest();
    request.AppRunTimes.Add(new AppRunTime
    {
        Id = new InternalID { Id = 0 },
        AppId = new InternalID { Id = session.AppPackageId },
        DeviceId = new InternalID { Id = session.DeviceId },
        RunTime = new TimeRange
        {
            StartTime = Timestamp.FromDateTime(session.StartTime.ToUniversalTime()),
            EndTime = Timestamp.FromDateTime(session.EndTime.Value.ToUniversalTime())
        }
    });
    
    await _client.BatchCreateAppRunTimeAsync(request, cancellationToken: ct);
}

// ... (add remaining methods)
```

---

#### 2.4 Create AppLaunchService

**Location**: `Waiter/Services/AppLaunchService.cs`

**Implementation**: Main orchestration logic from [service-contracts.md](contracts/service-contracts.md)

**Key methods**:
- `ValidateLaunchConfigurationAsync()` - Check paths exist (FR-016)
- `LaunchAndTrackAsync()` - Main launch workflow
- `UploadSessionDataAsync()` - Post-processing and upload
- `CheckIfRunningAsync()` - Duplicate detection (FR-020)
- `RetryUploadAsync()` - Manual retry for cached uploads

---

#### 2.5 Register Services in DI

**Location**: `Waiter/Program.cs`

**Add registrations**:
```csharp
// In ConfigureServices or Main method
services.AddSingleton<IProcessMonitorService, ProcessMonitorService>();
services.AddSingleton<ISaveDataService, SaveDataService>();
services.AddSingleton<IAppLaunchService, AppLaunchService>();
```

**Verification**:
```bash
dotnet build
dotnet run
# App should start without DI errors
```

---

### Phase 3: UI Layer (2-3 hours)

#### 3.1 Create ProgressDialog Form

**Location**: `Waiter/Forms/ProgressDialog.cs`

**Steps**:
1. Right-click `Forms` folder → Add → Form (Windows Forms)
2. Name: `ProgressDialog.cs`
3. Design UI per [research.md](research.md) mockup:
   - ProgressBar control
   - Label for status text
   - Hide and Cancel buttons

**Code**:
```csharp
public partial class ProgressDialog : Form
{
    public ProgressDialog(string title)
    {
        InitializeComponent();
        this.Text = title;
        progressBar.Style = ProgressBarStyle.Marquee; // Indeterminate by default
    }
    
    public void UpdateStatus(string message)
    {
        if (InvokeRequired)
        {
            Invoke(() => UpdateStatus(message));
            return;
        }
        statusLabel.Text = message;
    }
    
    public void UpdateProgress(int percentage)
    {
        if (InvokeRequired)
        {
            Invoke(() => UpdateProgress(percentage));
            return;
        }
        
        if (percentage < 0)
        {
            progressBar.Style = ProgressBarStyle.Marquee;
        }
        else
        {
            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.Value = Math.Clamp(percentage, 0, 100);
        }
    }
}
```

---

#### 3.2 Add Launch Button to AppDetailForm

**Location**: `Waiter/Forms/AppDetailForm.cs` (assumed to exist)

**Designer**:
1. Open `AppDetailForm.Designer.cs`
2. Add Button control: `btnLaunch` ("Launch App")
3. Wire up click event

**Code**:
```csharp
private readonly IAppLaunchService _appLaunchService;

public AppDetailForm(
    long appPackageId, 
    IAppLaunchService appLaunchService)
{
    InitializeComponent();
    _appPackageId = appPackageId;
    _appLaunchService = appLaunchService;
}

private async void btnLaunch_Click(object sender, EventArgs e)
{
    // Validation
    var validation = await _appLaunchService.ValidateLaunchConfigurationAsync(_appPackageId);
    if (!validation.IsValid)
    {
        MessageBox.Show(
            string.Join("\n", validation.Errors), 
            "Cannot Launch", 
            MessageBoxButtons.OK, 
            MessageBoxIcon.Warning);
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
    
    // Show progress dialog
    using var progressDialog = new ProgressDialog("Launching App");
    var cts = new CancellationTokenSource();
    
    var progress = new Progress<LaunchProgress>(p =>
    {
        progressDialog.UpdateStatus(p.Message);
        if (p.ProgressPercentage.HasValue)
            progressDialog.UpdateProgress(p.ProgressPercentage.Value);
    });
    
    progressDialog.Show(this);
    
    try
    {
        // Launch and track
        var session = await _appLaunchService.LaunchAndTrackAsync(
            _appPackageId, progress, cts.Token);
        
        // Upload data
        var uploadProgress = new Progress<UploadProgress>(p =>
        {
            progressDialog.UpdateStatus(p.Message);
            if (p.ProgressPercentage.HasValue)
                progressDialog.UpdateProgress(p.ProgressPercentage.Value);
        });
        
        await _appLaunchService.UploadSessionDataAsync(
            session.Id, uploadProgress, cts.Token);
        
        progressDialog.Close();
        MessageBox.Show(
            $"Session complete!\nRuntime: {session.Duration:hh\\:mm\\:ss}", 
            "Success", 
            MessageBoxButtons.OK, 
            MessageBoxIcon.Information);
    }
    catch (OperationCanceledException)
    {
        MessageBox.Show("Launch cancelled by user.");
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Launch failed");
        MessageBox.Show(
            $"Launch failed: {ex.Message}", 
            "Error", 
            MessageBoxButtons.OK, 
            MessageBoxIcon.Error);
    }
    finally
    {
        progressDialog.Close();
    }
}
```

---

### Phase 4: Testing (2 hours)

#### 4.1 Configure Test App Package

1. Open Waiter app
2. Navigate to app package details (or create test package)
3. Configure launch settings:
   - Executable path: Point to real app (e.g., `C:\Windows\System32\notepad.exe`)
   - Working directory: `C:\Windows\System32`
   - Monitoring mode: DirectProcess
   - Launch timeout: 30 seconds

4. Save settings

---

#### 4.2 Manual Test Scenarios

**Test 1: Basic Launch and Track** (User Story 1)
- Click "Launch App" button
- Verify progress dialog appears with "Starting..." message
- App launches successfully
- Use app for 30+ seconds
- Close app normally
- Verify progress dialog shows "Uploading..."
- Verify success message with runtime displayed
- Check database: RuntimeSession record created with correct duration

**Test 2: Invalid Configuration** (User Story 2, FR-016)
- Set executable path to non-existent file
- Click "Launch App"
- Verify error message: "Executable not found: [path]"
- Launch button should be disabled or validation prevents launch

**Test 3: Abnormal Exit** (User Story 4, FR-008)
- Launch test app
- Forcefully kill process (Task Manager)
- Verify prompt: "App exited abnormally. Upload save data anyway?"
- Test both "Yes" and "No" options
- Verify RuntimeSession.Status reflects choice (Completed vs Skipped)

**Test 4: Network Failure** (FR-018)
- Disconnect network
- Launch and complete app session
- Verify error message: "Upload failed. Data cached locally."
- Check database: CachedUpload record created
- Reconnect network
- Click "Sync Now" button (manual retry)
- Verify upload succeeds and cache entry deleted

**Test 5: Already Running** (User Story 3, FR-020)
- Launch notepad.exe via Waiter
- While running, click "Launch App" again
- Verify warning dialog: "App is already running (PID: XXXX). Launch anyway?"
- Test both "Yes" (allow duplicate) and "No" (cancel)

---

#### 4.3 Edge Cases

- Launch timeout: Set short timeout (5s), use slow-starting app
- Process listen mode: Test multi-process launcher (e.g., Steam game)
- Special characters in paths: Unicode, spaces, symbols
- Long-running sessions: Verify >1 hour sessions tracked correctly

---

### Phase 5: Documentation & Cleanup (30 minutes)

#### 5.1 Update README

**Location**: `README.md`

**Add section**:
```markdown
## Features

### App Launch & Runtime Tracking

Launch installed applications directly from Waiter with automatic runtime tracking:
- One-click launch from app details view
- Automatic session duration tracking
- Runtime statistics uploaded to TuiHub server
- Save data backup after each session
- Network failure handling with offline cache

See [specs/001-app-launch-runtime/](specs/001-app-launch-runtime/) for details.
```

---

#### 5.2 Add XML Documentation

Ensure all public APIs have XML doc comments:
```csharp
/// <summary>
/// Launches an application and tracks its runtime until exit.
/// </summary>
/// <param name="appPackageId">ID of the app package to launch</param>
/// <param name="progress">Progress callback for UI updates</param>
/// <param name="cancellationToken">Cancellation token to abort launch</param>
/// <returns>Completed runtime session with duration and exit code</returns>
/// <exception cref="InvalidOperationException">Thrown if launch configuration is invalid</exception>
```

---

#### 5.3 Commit Changes

**Commands**:
```bash
git add .
git commit -m "feat: implement app launch with runtime tracking

- Add AppPackageLaunchSettings, RuntimeSession, CachedUpload entities
- Create ProcessMonitorService for process lifecycle tracking
- Extend LibrarianClientService with runtime/save upload APIs
- Add launch button and progress dialog to AppDetailForm
- Implement FR-001 through FR-021 per spec

Closes #[issue-number]"
```

---

## Troubleshooting

### Issue: Migration fails with "table already exists"

**Solution**:
```bash
dotnet ef database drop
dotnet ef database update
```

---

### Issue: Process.WaitForExitAsync() not available

**Cause**: Incorrect .NET version  
**Solution**: Verify `TargetFramework` is `net8.0-windows` in `.csproj`

---

### Issue: gRPC call returns UNAUTHENTICATED

**Cause**: Token expired or invalid  
**Solution**: Check `TokenService` implementation, verify `ClientTokenInterceptor` registered

---

### Issue: Save file upload fails with 403 Forbidden

**Cause**: Presigned URL expired  
**Solution**: Reduce delay between getting token and uploading file

---

## Next Steps

After implementation complete:

1. **Create Pull Request**: Push branch and open PR against `main`
2. **Request Code Review**: Tag reviewers, reference constitution compliance
3. **Run Manual Tests**: Complete all test scenarios from Phase 4
4. **Update Changelog**: Document new features and breaking changes
5. **Merge**: After approval and tests pass

---

## Time Estimates

| Phase | Estimated Time | Description |
|-------|----------------|-------------|
| Database Layer | 1-2 hours | Entities, migrations, queries |
| Service Layer | 3-4 hours | Core business logic |
| UI Layer | 2-3 hours | Forms and event handlers |
| Testing | 2 hours | Manual test scenarios |
| Documentation | 30 minutes | README, XML docs |
| **Total** | **8-11 hours** | Full feature implementation |

---

## Resources

- [Feature Spec](spec.md) - Requirements and user stories
- [Research](research.md) - Technology decisions and patterns
- [Data Model](data-model.md) - Database schema and entities
- [API Contracts](contracts/api-contracts.md) - gRPC endpoint details
- [Service Contracts](contracts/service-contracts.md) - Internal service interfaces
- [Constitution](../.specify/memory/constitution.md) - Architecture principles

---

**Questions?** Contact the team lead or refer to the constitution for architecture guidance.
