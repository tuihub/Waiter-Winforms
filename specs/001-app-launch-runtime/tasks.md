# Implementation Tasks: App Launch with Runtime Tracking

**Feature**: App Launch with Runtime Tracking  
**Branch**: `001-app-launch-runtime`  
**Spec**: [spec.md](spec.md) | **Plan**: [plan.md](plan.md)  
**Generated**: 2026-01-31

---

## Overview

This document provides a complete, actionable task breakdown for implementing app launch with runtime tracking. Tasks are organized by user story to enable independent implementation and testing of each feature increment.

**Implementation Strategy**: MVP-first approach - implement User Story 1 (P1) first for a working launch feature, then incrementally add error handling (US2), advanced monitoring (US3), and polish features (US4, US5).

**Test Approach**: Manual testing per constitution. Each user story includes independent test criteria that can be validated without dependencies on other stories.

---

## Phase 1: Setup & Infrastructure

**Goal**: Initialize project structure, database schema, and foundational services.

**Dependencies**: None - can start immediately

**Estimated Time**: 2-3 hours

### Tasks

- [x] T001 Create entity model classes in Waiter/Data/Models/
  - Create `AppPackageLaunchSettings.cs` with properties: Id, AppPackageId, ExecutablePath, WorkingDirectory, MonitoringMode, ProcessName, UseShellExecute, LaunchTimeout, SaveDataPath
  - Create `RuntimeSession.cs` with properties: Id, AppPackageId, DeviceId, StartTime, EndTime, ExitCode, Status, UploadAttempted, UploadedAt
  - Create `CachedUpload.cs` with properties: Id, RuntimeSessionId, UploadType, FilePath, Metadata, CreatedAt, RetryCount, LastError, ExpiresAt
  - Create enums: `MonitoringMode.cs`, `SessionStatus.cs`, `UploadType.cs`
  - Reference: [data-model.md](data-model.md) sections 1-4

- [x] T002 [P] Create EF Core configurations in Waiter/Data/Configurations/
  - Create `AppPackageLaunchSettingsConfiguration.cs` implementing IEntityTypeConfiguration
  - Create `RuntimeSessionConfiguration.cs` implementing IEntityTypeConfiguration
  - Create `CachedUploadConfiguration.cs` implementing IEntityTypeConfiguration
  - Apply configurations in WaiterDbContext.OnModelCreating()
  - Reference: [data-model.md](data-model.md) Entity Framework Configuration section

- [x] T003 Create and apply EF Core migration for new tables
  - Run: `dotnet ef migrations add AddAppLaunchTables` from Waiter/ directory
  - Review generated migration SQL matches [data-model.md](data-model.md) Migration section
  - Run: `dotnet ef database update` to apply migration
  - Verify tables created: AppPackageLaunchSettings, RuntimeSessions, CachedUploads
  - Verify indexes created: IX_RuntimeSessions_AppPackageId, IX_RuntimeSessions_DeviceId, IX_RuntimeSessions_Status, IX_CachedUploads_RuntimeSessionId, IX_CachedUploads_ExpiresAt

- [x] T004 [P] Extend DatabaseService with query methods in Waiter/Data/DatabaseService.cs
  - Add `GetLaunchSettingsAsync(long appPackageId)` returning AppPackageLaunchSettings?
  - Add `SaveLaunchSettingsAsync(AppPackageLaunchSettings settings)` for insert/update
  - Add `CreateRuntimeSessionAsync(RuntimeSession session)` returning created entity
  - Add `UpdateRuntimeSessionAsync(RuntimeSession session)` for status updates
  - Add `GetRuntimeSessionAsync(long sessionId)` returning RuntimeSession?
  - Add `CreateCachedUploadAsync(CachedUpload upload)` returning created entity
  - Add `GetPendingUploadsAsync()` returning List<CachedUpload>
  - Add `DeleteCachedUploadAsync(long uploadId)` for cleanup
  - Reference: [service-contracts.md](contracts/service-contracts.md) Extended DatabaseService section

- [x] T005 Verify compilation and database schema
  - Run: `dotnet build` from Waiter-Winforms/ directory - must compile without errors
  - Open SQLite database with DB Browser, verify all tables and indexes present
  - Test query: `SELECT * FROM AppPackageLaunchSettings` - should return empty result set

---

## Phase 2: Foundational Services (Blocking Prerequisites)

**Goal**: Implement core services required by all user stories.

**Dependencies**: Phase 1 must be complete

**Estimated Time**: 2-3 hours

### Tasks

- [x] T006 Create IProcessMonitorService interface in Waiter/Services/IProcessMonitorService.cs
  - Define `Task<ProcessResult> TrackProcessAsync(Process process, CancellationToken ct)`
  - Define `Task<ProcessResult> TrackProcessByNameAsync(string processName, TimeSpan timeout, CancellationToken ct)`
  - Define `RunningProcessInfo? FindRunningProcess(string processName)`
  - Define supporting types: ProcessResult, RunningProcessInfo
  - Reference: [service-contracts.md](contracts/service-contracts.md) section 2

- [x] T007 Implement ProcessMonitorService in Waiter/Services/ProcessMonitorService.cs
  - Implement TrackProcessAsync using Process.WaitForExitAsync() (.NET 8 API)
  - Capture Process.StartTime, calculate EndTime, Duration, ExitCode
  - Implement TrackProcessByNameAsync with polling logic (500ms intervals)
  - Implement FindRunningProcess using Process.GetProcessesByName()
  - Handle TimeoutException when process doesn't appear within timeout
  - Reference: [research.md](research.md) section 1 for implementation pattern

- [x] T008 [P] Create ISaveDataService interface in Waiter/Services/ISaveDataService.cs
  - Define `Task<FileInfo> CreateSaveArchiveAsync(string savePath, string? outputFileName, CancellationToken ct)`
  - Define `Task<string> CalculateSHA256Async(FileInfo file, CancellationToken ct)`
  - Reference: [service-contracts.md](contracts/service-contracts.md) section 3

- [x] T009 [P] Implement SaveDataService in Waiter/Services/SaveDataService.cs
  - Implement CreateSaveArchiveAsync using System.IO.Compression.ZipFile.CreateFromDirectory()
  - Save archive to temp directory with timestamp naming: `save_yyyyMMdd_HHmmss.zip`
  - Implement CalculateSHA256Async using SHA256.Create() and ComputeHashAsync()
  - Return hash as lowercase hex string
  - Reference: [research.md](research.md) section 4 for inline approach

- [x] T010 Extend LibrarianClientService with runtime upload methods in Waiter/Services/LibrarianClientService.cs
  - Add `Task ReportRuntimeAsync(RuntimeSession session, CancellationToken ct)`
  - Build BatchCreateAppRunTimeRequest from session data
  - Convert DateTime to Timestamp using Timestamp.FromDateTime(dt.ToUniversalTime())
  - Add `Task<string> GetSaveFileUploadTokenAsync(long appId, FileInfo saveFile, string sha256Hash, CancellationToken ct)`
  - Build UploadAppSaveFileRequest with FileMetadata
  - Add `Task UploadSaveFileDataAsync(string uploadToken, FileInfo saveFile, IProgress<int> progress, CancellationToken ct)`
  - Implement HTTP PUT upload to presigned URL using HttpClient
  - Reference: [api-contracts.md](contracts/api-contracts.md) sections 1-3

- [x] T011 Register new services in DI container in Waiter/Program.cs
  - Add `services.AddSingleton<IProcessMonitorService, ProcessMonitorService>()`
  - Add `services.AddSingleton<ISaveDataService, SaveDataService>()`
  - Add `services.AddSingleton<IAppLaunchService, AppLaunchService>()` (will create in next phase)
  - Verify app starts without DI resolution errors

---

## Phase 3: User Story 1 - Launch Application and Track Runtime (P1)

**Goal**: Core launch functionality with runtime tracking and server upload.

**Dependencies**: Phase 2 complete

**Estimated Time**: 3-4 hours

**Independent Test Criteria**: 
- Launch app from UI successfully
- App runs for measurable duration (30+ seconds)
- Close app normally (exit code 0)
- Verify RuntimeSession record created in database with correct start/end times
- Verify duration accuracy within 1 second
- Verify no errors in application logs

### Tasks

- [x] T012 Create IAppLaunchService interface in Waiter/Services/IAppLaunchService.cs
  - Define `Task<LaunchValidationResult> ValidateLaunchConfigurationAsync(long appPackageId)`
  - Define `Task<RuntimeSession> LaunchAndTrackAsync(long appPackageId, IProgress<LaunchProgress> progress, CancellationToken ct)`
  - Define `Task UploadSessionDataAsync(long sessionId, IProgress<UploadProgress> progress, CancellationToken ct)`
  - Define supporting types: LaunchValidationResult, LaunchProgress, LaunchPhase, UploadProgress, UploadPhase
  - Reference: [service-contracts.md](contracts/service-contracts.md) section 1

- [x] T013 [US1] Implement AppLaunchService core methods in Waiter/Services/AppLaunchService.cs
  - Inject dependencies: IProcessMonitorService, IDatabaseService, ILibrarianClientService, ISaveDataService, ILogger
  - Implement ValidateLaunchConfigurationAsync: check ExecutablePath and WorkingDirectory exist using File.Exists() and Directory.Exists()
  - Return LaunchValidationResult with errors list if validation fails
  - Reference: [service-contracts.md](contracts/service-contracts.md) AppLaunchService Implementation section

- [x] T014 [US1] Implement LaunchAndTrackAsync in Waiter/Services/AppLaunchService.cs
  - Load AppPackageLaunchSettings from database
  - Create RuntimeSession record with Status = Tracking, StartTime = DateTime.UtcNow
  - Create ProcessStartInfo with ExecutablePath, WorkingDirectory, UseShellExecute from settings
  - Call Process.Start() to launch application
  - Report LaunchProgress(LaunchPhase.Starting, "Starting application...")
  - For DirectProcess mode: call ProcessMonitorService.TrackProcessAsync()
  - Update RuntimeSession with EndTime, ExitCode, Status = Processing (if exit code 0) or Abnormal (if non-zero)
  - Save updated session to database
  - Reference: [service-contracts.md](contracts/service-contracts.md) workflow example

- [x] T015 [US1] Implement UploadSessionDataAsync in Waiter/Services/AppLaunchService.cs
  - Load RuntimeSession from database
  - Report UploadProgress(UploadPhase.ReportingRuntime, "Uploading runtime data...")
  - Call LibrarianClientService.ReportRuntimeAsync(session)
  - If SaveDataPath configured: create save archive using SaveDataService.CreateSaveArchiveAsync()
  - Calculate SHA256 hash using SaveDataService.CalculateSHA256Async()
  - Get upload token using LibrarianClientService.GetSaveFileUploadTokenAsync()
  - Upload file data using LibrarianClientService.UploadSaveFileDataAsync() with progress callback
  - Update RuntimeSession: UploadAttempted = true, UploadedAt = DateTime.UtcNow, Status = Completed
  - Delete temp save archive file
  - Reference: [api-contracts.md](contracts/api-contracts.md) Data Flow Diagram

- [x] T016 [US1] Create ProgressDialog Form in Waiter/Forms/ProgressDialog.cs
  - Add Form with title parameter in constructor
  - Add ProgressBar control (default style: Marquee for indeterminate)
  - Add Label control for status text
  - Add Hide button (minimizes dialog, accessible via taskbar)
  - Add Cancel button (triggers cancellation token)
  - Implement UpdateStatus(string message) with Invoke() for thread safety
  - Implement UpdateProgress(int percentage) - if >= 0, set Continuous style and Value; if < 0, set Marquee style
  - Reference: [research.md](research.md) section 5 for UI mockup

- [x] T017 [US1] Add launch button to AppDetailForm in Waiter/Forms/AppDetailForm.cs
  - Add Button control in Designer: `btnLaunch` with text "Launch App"
  - Position button in app details section (exact placement per existing UI layout)
  - Wire up btnLaunch_Click event handler
  - Inject IAppLaunchService via constructor (update DI registration if needed)

- [x] T018 [US1] Implement launch click handler in Waiter/Forms/AppDetailForm.cs
  - In btnLaunch_Click: call ValidateLaunchConfigurationAsync()
  - If validation fails: show MessageBox with errors, return early
  - Create ProgressDialog with app name in title
  - Create CancellationTokenSource
  - Create Progress<LaunchProgress> callback updating ProgressDialog.UpdateStatus()
  - Show ProgressDialog.Show(this)
  - Await LaunchAndTrackAsync() with progress and cancellation token
  - Hide ProgressDialog during app runtime (minimize window)
  - After app exits: show ProgressDialog again for upload phase
  - Create Progress<UploadProgress> callback
  - Await UploadSessionDataAsync() with progress
  - Close ProgressDialog
  - Show success MessageBox with runtime duration
  - Catch OperationCanceledException: show "Launch cancelled" message
  - Catch Exception: log error, show error MessageBox
  - Reference: [service-contracts.md](contracts/service-contracts.md) Service Interaction Flow

- [ ] T019 [US1] Manual test: Basic launch and track
  - Configure test app package: Executable = `C:\Windows\System32\notepad.exe`, WorkingDirectory = `C:\Windows\System32`
  - Click "Launch App" button
  - Verify ProgressDialog appears with "Starting notepad..." message
  - Verify notepad.exe launches successfully
  - Use notepad for 30+ seconds (type some text)
  - Close notepad normally (File > Exit or X button)
  - Verify ProgressDialog reappears showing "Uploading runtime data..."
  - Verify success message shows runtime duration (should be ~30 seconds)
  - Open database, query: `SELECT * FROM RuntimeSessions ORDER BY Id DESC LIMIT 1`
  - Verify: StartTime and EndTime set, ExitCode = 0, Duration ~30 seconds, Status = Completed, UploadedAt set
  - Check application logs for any errors

---

## Phase 4: User Story 2 - Handle Application Launch Errors (P2)

**Goal**: Graceful error handling with clear user feedback.

**Dependencies**: User Story 1 complete

**Estimated Time**: 1-2 hours

**Independent Test Criteria**:
- Invalid executable path displays "File not found" error
- Missing working directory displays "Directory not found" error
- Launch timeout displays timeout error with retry option
- All errors display without crashing application

### Tasks

- [x] T020 [US2] Enhance ValidateLaunchConfigurationAsync with detailed error messages in Waiter/Services/AppLaunchService.cs
  - Check ExecutablePath: if !File.Exists(), add error "Executable not found: {path}"
  - Check WorkingDirectory: if !Directory.Exists(), add error "Working directory not found: {path}"
  - Check ProcessName: if MonitoringMode = ProcessName and string.IsNullOrWhiteSpace(ProcessName), add error "Process name required for process listen mode"
  - Return LaunchValidationResult with isValid = false if any errors

- [x] T021 [US2] Add error handling in LaunchAndTrackAsync in Waiter/Services/AppLaunchService.cs
  - Wrap Process.Start() in try-catch: catch Win32Exception for permission/access errors
  - If Process.Start() returns null: throw InvalidOperationException("Failed to start process")
  - Catch exceptions during tracking, update RuntimeSession.Status = Failed
  - Log all exceptions with ILogger.LogError()
  - Rethrow exceptions to be handled by UI layer

- [x] T022 [US2] Add network error handling in UploadSessionDataAsync in Waiter/Services/AppLaunchService.cs
  - Wrap ReportRuntimeAsync() in try-catch for RpcException
  - If StatusCode = Unavailable: call CacheRuntimeDataAsync() (implement next task)
  - Show notification "Runtime data cached. Will retry when online."
  - Update RuntimeSession.Status = Failed (not Completed)
  - Same pattern for save file upload: catch network errors, cache data
  - Reference: [api-contracts.md](contracts/api-contracts.md) Error Handling Strategy

- [x] T023 [P] [US2] Implement cache logic for failed uploads in Waiter/Services/AppLaunchService.cs
  - Create private method `Task CacheRuntimeDataAsync(RuntimeSession session)`
  - Serialize session data to JSON using System.Text.Json.JsonSerializer
  - Save JSON to cache directory: `{CachePath}/pending-uploads/{guid}_runtime.json`
  - Create CachedUpload record: UploadType = RuntimeData, FilePath = json file path, Metadata = serialized request
  - Same for save files: cache archive + metadata, create CachedUpload record with UploadType = SaveFile
  - Reference: [research.md](research.md) section 8 for cache structure

- [x] T024 [US2] Update launch button click handler with error display in Waiter/Forms/AppDetailForm.cs
  - Validation errors: show MessageBox with all errors joined by newlines, icon = Warning
  - Launch exceptions: show MessageBox "Launch failed: {exception.Message}", icon = Error
  - Network errors: show MessageBox "Upload failed. Data cached locally.", icon = Information
  - Ensure ProgressDialog closes in finally block even if errors occur

- [ ] T025 [US2] Manual test: Invalid configuration
  - Set ExecutablePath to non-existent file: `C:\NonExistent\app.exe`
  - Click "Launch App"
  - Verify error MessageBox displays: "Executable not found: C:\NonExistent\app.exe"
  - Set ExecutablePath back to valid, set WorkingDirectory to non-existent: `C:\NonExistent\`
  - Click "Launch App"
  - Verify error MessageBox displays: "Working directory not found: C:\NonExistent\"
  - Verify application doesn't crash, can correct settings and retry

- [ ] T026 [US2] Manual test: Network failure handling
  - Disconnect network (disable Wi-Fi/Ethernet)
  - Configure valid app, launch and complete session normally
  - Verify error MessageBox: "Upload failed. Data cached locally."
  - Query database: `SELECT * FROM CachedUploads` - verify records created
  - Check cache directory for pending upload files
  - Reconnect network (will implement retry in next phase)

---

## Phase 5: User Story 3 - Monitor Process by Name (P2)

**Goal**: Support multi-process applications using process name monitoring.

**Dependencies**: User Story 1 complete (can be done in parallel with User Story 2)

**Estimated Time**: 1-2 hours

**Independent Test Criteria**:
- Configure app with MonitoringMode = ProcessName and target process name
- Launch app that spawns child process
- Verify system detects target process appearance
- Verify runtime calculated from target process lifecycle, not launcher
- Verify timeout error if target process never appears

### Tasks

- [x] T027 [US3] Implement process name monitoring mode in LaunchAndTrackAsync in Waiter/Services/AppLaunchService.cs
  - After Process.Start(), check if settings.MonitoringMode == MonitoringMode.ProcessName
  - If ProcessName mode: call ProcessMonitorService.TrackProcessByNameAsync(settings.ProcessName, TimeSpan.FromSeconds(settings.LaunchTimeout))
  - Report LaunchProgress(LaunchPhase.WaitingForProcess, $"Waiting for {settings.ProcessName}...")
  - If timeout: update session Status = Failed, throw TimeoutException($"Process {settings.ProcessName} not detected within {settings.LaunchTimeout} seconds")
  - Use ProcessResult from TrackProcessByNameAsync to update session (start/end times from actual target process)

- [ ] T028 [P] [US3] Add launch settings configuration UI (optional, can stub for now)
  - Option 1: Add to existing app settings form/dialog
  - Option 2: Hardcode in test data for manual testing
  - Required fields: MonitoringMode dropdown, ProcessName textbox (enabled when ProcessName selected)
  - For MVP: can manually update database records for testing

- [ ] T029 [US3] Manual test: Process listen mode
  - Find multi-process test app (e.g., installer that spawns setup.exe)
  - Alternative: Create batch file that launches notepad.exe then exits: `start notepad.exe`
  - Set AppPackageLaunchSettings: ExecutablePath = batch file, MonitoringMode = ProcessName, ProcessName = "notepad"
  - Click "Launch App"
  - Verify ProgressDialog shows "Waiting for notepad..."
  - Verify batch exits immediately but tracking continues
  - Verify notepad runtime tracked correctly when you close notepad
  - Verify RuntimeSession.Duration matches notepad lifetime, not batch lifetime

- [ ] T030 [US3] Manual test: Timeout scenario
  - Set MonitoringMode = ProcessName, ProcessName = "NonExistentProcess"
  - Set LaunchTimeout = 5 seconds
  - Launch app that doesn't spawn target process
  - Wait 5 seconds
  - Verify timeout error MessageBox: "Process NonExistentProcess not detected within 5 seconds"
  - Verify RuntimeSession.Status = Failed

---

## Phase 6: User Story 4 - Handle Abnormal Application Exit (P3)

**Goal**: Prompt user for save upload confirmation after abnormal exit.

**Dependencies**: User Story 1 complete

**Estimated Time**: 1 hour

**Independent Test Criteria**:
- Launch app and forcefully terminate (Task Manager kill)
- Verify prompt appears: "App exited abnormally. Upload save data anyway?"
- Test "Yes": verify save data uploaded, RuntimeSession.Status = Completed
- Test "No": verify save skipped, RuntimeSession.Status = Skipped

### Tasks

- [x] T031 [US4] Detect abnormal exit in LaunchAndTrackAsync in Waiter/Services/AppLaunchService.cs
  - After ProcessResult received, check if result.ExitCode != 0
  - If abnormal exit: update RuntimeSession.Status = Abnormal (don't set to Processing/Completed yet)
  - Save session to database
  - Return session without calling UploadSessionDataAsync automatically

- [x] T032 [US4] Add abnormal exit prompt in launch button handler in Waiter/Forms/AppDetailForm.cs
  - After LaunchAndTrackAsync() returns, check if session.Status == SessionStatus.Abnormal
  - Show MessageBox with Yes/No buttons: "App exited abnormally (exit code {session.ExitCode}). Upload save data anyway?"
  - If Yes: call UploadSessionDataAsync(session.Id) normally, update Status = Completed
  - If No: update RuntimeSession.Status = Skipped, update UploadAttempted = true, save to database, skip upload
  - Show appropriate completion message based on user choice

- [ ] T033 [US4] Manual test: Abnormal exit with upload
  - Launch notepad.exe
  - Type some text
  - Kill notepad via Task Manager (force terminate)
  - Verify prompt: "App exited abnormally (exit code -1). Upload save data anyway?"
  - Click "Yes"
  - Verify upload proceeds normally
  - Verify RuntimeSession.Status = Completed, UploadedAt set

- [ ] T034 [US4] Manual test: Abnormal exit without upload
  - Repeat above test
  - Click "No" when prompted
  - Verify no upload occurs
  - Verify RuntimeSession.Status = Skipped, UploadAttempted = true
  - Verify success message acknowledges skip: "Runtime recorded. Save upload skipped per user request."

---

## Phase 7: User Story 5 - Display Launch Progress and Post-Processing Status (P3)

**Goal**: Enhanced progress feedback during all phases.

**Dependencies**: User Story 1 complete

**Estimated Time**: 1 hour

**Independent Test Criteria**:
- Progress dialog shows detailed status messages during each phase
- Progress bar animates during indeterminate operations
- Progress bar shows percentage during file upload
- Dialog hides during app runtime, reappears for upload

### Tasks

- [x] T035 [US5] Enhance LaunchAndTrackAsync progress reporting in Waiter/Services/AppLaunchService.cs
  - Report LaunchProgress(LaunchPhase.Validating, "Loading configuration...") before loading settings
  - Report LaunchProgress(LaunchPhase.Starting, $"Starting {appName}...") before Process.Start()
  - Report LaunchProgress(LaunchPhase.Tracking, $"Monitoring {appName}...") during WaitForExitAsync()
  - Report LaunchProgress(LaunchPhase.Complete, "Application closed") after tracking complete

- [x] T036 [US5] Enhance UploadSessionDataAsync progress reporting in Waiter/Services/AppLaunchService.cs
  - Report UploadProgress(UploadPhase.ReportingRuntime, "Uploading runtime statistics...") before ReportRuntimeAsync()
  - Report UploadProgress(UploadPhase.CreatingArchive, "Compressing save data...") before CreateSaveArchiveAsync()
  - Report UploadProgress(UploadPhase.CalculatingHash, "Verifying file integrity...") during CalculateSHA256Async()
  - Report UploadProgress(UploadPhase.UploadingSave, "Uploading save file...", percentage) during UploadSaveFileDataAsync() with progress callback
  - Report UploadProgress(UploadPhase.Complete, "Upload complete") when finished

- [x] T037 [US5] Implement hide/show logic in launch button handler in Waiter/Forms/AppDetailForm.cs
  - After LaunchAndTrackAsync() call before await: progressDialog.Show(this)
  - When LaunchPhase.Tracking reported: progressDialog.Hide() or WindowState = Minimized
  - After LaunchAndTrackAsync() returns (app exited): progressDialog.Show() again to become visible
  - Continue with upload phase using same dialog
  - Close dialog only after all operations complete

- [x] T038 [US5] Add percentage progress support to ProgressDialog in Waiter/Forms/ProgressDialog.cs
  - Modify UpdateProgress() to accept optional percentage parameter
  - If percentage provided: set ProgressBar.Style = Continuous, Value = percentage
  - If percentage is null or < 0: set ProgressBar.Style = Marquee (indeterminate animation)
  - Ensure thread-safe updates using Invoke()

- [ ] T039 [US5] Manual test: Progress feedback
  - Launch notepad with save data configured
  - Observe ProgressDialog during each phase:
    - "Loading configuration..." (brief)
    - "Starting notepad..." (brief)
    - "Monitoring notepad..." (dialog should hide/minimize)
  - Close notepad
  - Observe ProgressDialog reappears:
    - "Uploading runtime statistics..." (brief)
    - "Compressing save data..." (brief)
    - "Uploading save file..." (with percentage 0% → 100%)
    - "Upload complete" (brief)
  - Verify dialog closes automatically after completion
  - Verify smooth user experience, no long pauses without feedback

---

## Phase 8: Polish & Cross-Cutting Concerns

**Goal**: Cleanup, optimization, and final quality checks.

**Dependencies**: All user stories complete

**Estimated Time**: 1-2 hours

### Tasks

- [x] T040 Implement CheckIfRunningAsync detection in Waiter/Services/AppLaunchService.cs
  - Add method per IAppLaunchService interface
  - Load launch settings to get ExecutablePath
  - Extract process name from executable path using Path.GetFileNameWithoutExtension()
  - Call ProcessMonitorService.FindRunningProcess()
  - Return RunningProcessInfo if found, null otherwise

- [x] T041 Add already-running check to launch button handler in Waiter/Forms/AppDetailForm.cs
  - Before showing ProgressDialog, call CheckIfRunningAsync()
  - If running process found: show MessageBox with Yes/No buttons: "App is already running (PID: {pid}). Launch anyway?"
  - If No: return early, don't launch
  - If Yes: proceed with launch (allows multiple instances if user wants)

- [x] T042 Implement RetryUploadAsync for manual retry in Waiter/Services/AppLaunchService.cs
  - Load CachedUpload record from database
  - Deserialize Metadata JSON to get upload request details
  - Retry upload based on UploadType (RuntimeData or SaveFile)
  - If successful: delete CachedUpload record and cached file
  - If failed: increment RetryCount, update LastError, save to database
  - Max 10 retries per cached upload (check before retrying)

- [x] T043 Implement CleanupExpiredCacheAsync in Waiter/Services/AppLaunchService.cs
  - Query CachedUploads where ExpiresAt < DateTime.UtcNow
  - For each expired upload: delete cached file from disk, delete database record
  - Log cleanup results: X cached uploads expired and removed
  - Call this method on app startup (in Program.cs or main form Load event)

- [x] T044 Add XML documentation comments to all public APIs
  - Document all public methods in IAppLaunchService, IProcessMonitorService, ISaveDataService
  - Document all public properties in entity classes
  - Follow standard XML doc format: <summary>, <param>, <returns>, <exception>
  - Reference: [quickstart.md](quickstart.md) section 5.2

- [x] T045 Add logging throughout services
  - Log information: "Launching app {appId} with executable {path}"
  - Log warning: "Upload failed, caching data locally: {error}"
  - Log error: "Launch failed: {exception}"
  - Use ILogger<T> injected via DI, follow ASP.NET Core logging patterns

- [x] T046 Update README.md with feature documentation
  - Add section "App Launch & Runtime Tracking" under Features
  - Describe: one-click launch, automatic runtime tracking, save data backup, offline cache
  - Link to specs/001-app-launch-runtime/ for details
  - Reference: [quickstart.md](quickstart.md) section 5.1

- [ ] T047 Final integration test: Complete user workflow
  - Install fresh copy of Waiter, apply migrations
  - Configure app package: notepad.exe with save data path
  - Test sequence:
    1. Launch with invalid path → see error
    2. Fix path, launch successfully → track runtime
    3. Normal exit → upload succeeds
    4. Launch again while running → see warning
    5. Force kill → confirm upload prompt
    6. Disconnect network, launch → see cache message
    7. Reconnect, retry upload → succeeds
  - Verify all scenarios work end-to-end
  - Verify database state correct after each step
  - Verify no errors in application logs

- [ ] T048 Code review and refactoring
  - Review all new code for constitution compliance
  - Check: all business logic in Services (not Forms)
  - Check: all DB operations through EF Core
  - Check: all API calls through LibrarianClientService
  - Check: all async operations follow proper patterns
  - Refactor any violations found
  - Run code formatter/linter if available

---

## Dependency Graph

This graph shows the completion order for user stories. User Stories 2, 3, 4, 5 can be implemented in any order after User Story 1 is complete.

```
┌─────────────┐
│   Phase 1   │  Setup & Infrastructure
│   (T001-005)│
└──────┬──────┘
       │
       ▼
┌─────────────┐
│   Phase 2   │  Foundational Services (Blocking)
│   (T006-011)│
└──────┬──────┘
       │
       ▼
┌─────────────────────────────────────────────────────┐
│            Phase 3: User Story 1 (P1)               │
│ Launch Application and Track Runtime (T012-019)     │
│ *** CORE FEATURE - Must complete first ***          │
└───────┬────────────────────┬────────────────────────┘
        │                    │
        │                    │
        ▼                    ▼
┌──────────────┐      ┌──────────────┐
│  Phase 4:    │      │  Phase 5:    │
│  User Story 2│      │  User Story 3│
│  (P2) Errors │      │  (P2) Process│
│  (T020-026)  │      │  Listen Mode │
└──────┬───────┘      │  (T027-030)  │
       │              └──────┬───────┘
       │                     │
       │    ┌────────────────┴─────┬─────────────────┐
       │    │                      │                 │
       ▼    ▼                      ▼                 ▼
┌──────────────┐         ┌──────────────┐  ┌──────────────┐
│  Phase 6:    │         │  Phase 7:    │  │  Phase 8:    │
│  User Story 4│         │  User Story 5│  │  Polish &    │
│  (P3) Abnormal│        │  (P3) Progress│ │  Cleanup     │
│  Exit        │         │  Feedback    │  │  (T040-048)  │
│  (T031-034)  │         │  (T035-039)  │  │              │
└──────────────┘         └──────────────┘  └──────────────┘
        │                       │                  │
        └───────────────┬───────┴──────────────────┘
                        ▼
                  ┌──────────┐
                  │ Complete │
                  └──────────┘
```

**Parallel Opportunities per Phase**:

- **Phase 1**: T002 can run parallel with T001 (different files)
- **Phase 2**: T006+T007 parallel with T008+T009 (independent services); T010 can run after interfaces defined
- **Phase 3**: T016 (ProgressDialog) parallel with T012-015 (service implementation)
- **Phases 4-7**: All user stories 2-5 independent, can be implemented in parallel by different developers

---

## Implementation Strategy

### MVP Scope (Minimum Viable Product)

**Goal**: Working launch feature with basic functionality

**Includes**:
- Phase 1: Setup & Infrastructure (T001-T005)
- Phase 2: Foundational Services (T006-T011)  
- Phase 3: User Story 1 - Basic Launch & Track (T012-T019)

**Estimated Time**: 7-10 hours

**Test**: User can launch notepad, track runtime, upload to server

---

### Full Feature Scope

**Goal**: Production-ready feature with all user stories

**Includes**: All phases (T001-T048)

**Estimated Time**: 15-20 hours

**Test**: All acceptance scenarios pass, all edge cases handled

---

## Manual Testing Checklist

Complete testing checklist organized by user story for independent validation.

### User Story 1: Basic Launch & Track ✅
- [ ] Launch app with valid configuration succeeds
- [ ] Runtime tracking accurate within 1 second for 30+ second session
- [ ] RuntimeSession record created with correct data
- [ ] Server upload succeeds (check server-side logs)
- [ ] Progress dialog appears during launch
- [ ] Success message shows correct runtime duration

### User Story 2: Error Handling ✅
- [ ] Invalid executable path shows clear error message
- [ ] Invalid working directory shows clear error message
- [ ] Network failure caches data locally with message
- [ ] CachedUpload records created when network fails
- [ ] Application doesn't crash on any error condition
- [ ] User can correct errors and retry successfully

### User Story 3: Process Listen Mode ✅
- [ ] Process name monitoring detects child process
- [ ] Runtime calculated from target process, not launcher
- [ ] Timeout error appears if target process never starts
- [ ] Progress dialog shows "Waiting for {processName}..." message

### User Story 4: Abnormal Exit ✅
- [ ] Force-killed app triggers upload confirmation prompt
- [ ] "Yes" option uploads data and sets Status = Completed
- [ ] "No" option skips upload and sets Status = Skipped
- [ ] Runtime still recorded even if upload skipped

### User Story 5: Progress Feedback ✅
- [ ] Progress dialog shows detailed status during all phases
- [ ] Dialog hides/minimizes during app runtime
- [ ] Dialog reappears automatically after app exits
- [ ] Upload progress shows percentage (0-100%)
- [ ] All operations provide feedback within 500ms

### Edge Cases ✅
- [ ] Already running app shows warning with option to continue
- [ ] Special characters in paths (spaces, Unicode) handled correctly
- [ ] Multiple simultaneous launches work independently
- [ ] Long-running sessions (>1 hour) tracked correctly
- [ ] Expired cached uploads cleaned up on startup

### Constitution Compliance ✅
- [ ] All business logic in Services (not Forms)
- [ ] All database operations through EF Core
- [ ] All server communication through LibrarianClientService
- [ ] All long operations are async with UI feedback
- [ ] No premature abstractions (YAGNI principle followed)

---

## Commit Strategy

Recommended commit points for clean git history:

1. **After T005**: "feat(data): add app launch database schema and entities"
2. **After T011**: "feat(services): add foundational services for process monitoring and uploads"
3. **After T019**: "feat(launch): implement basic app launch with runtime tracking (US1)"
4. **After T026**: "feat(errors): add error handling and offline caching (US2)"
5. **After T030**: "feat(monitoring): add process name monitoring mode (US3)"
6. **After T034**: "feat(exit): handle abnormal app exit with user confirmation (US4)"
7. **After T039**: "feat(progress): enhance progress feedback UI (US5)"
8. **After T048**: "feat(polish): final cleanup and documentation"

Each commit should compile and pass manual tests for completed features.

---

## Task Summary

**Total Tasks**: 48  
**Phase 1 (Setup)**: 5 tasks  
**Phase 2 (Foundational)**: 6 tasks  
**Phase 3 (US1 - P1)**: 8 tasks  
**Phase 4 (US2 - P2)**: 7 tasks  
**Phase 5 (US3 - P2)**: 4 tasks  
**Phase 6 (US4 - P3)**: 4 tasks  
**Phase 7 (US5 - P3)**: 5 tasks  
**Phase 8 (Polish)**: 9 tasks  

**Parallelizable Tasks**: 9 tasks marked with [P]  
**MVP Tasks**: 19 tasks (T001-T019)  
**Full Feature Tasks**: 48 tasks

**Estimated Time**:
- MVP: 7-10 hours
- Full Feature: 15-20 hours

---

**Ready for Implementation** | Feature: `001-app-launch-runtime` | Generated: 2026-01-31
