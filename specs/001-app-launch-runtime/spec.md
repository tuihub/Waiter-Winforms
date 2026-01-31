# Feature Specification: App Launch with Runtime Tracking

**Feature Branch**: `001-app-launch-runtime`  
**Created**: 2026-01-31  
**Status**: Draft  
**Input**: User description: "Implement app launch button with ProcessTimeMonitor integration for runtime statistics tracking, referencing WPF AppsViewModel OnStartApp logic"

## Clarifications

### Session 2026-01-31

- Q: What constitutes a valid minimum configuration to enable the launch button? → A: Both executable path AND working directory must be explicitly configured and exist on disk
- Q: How should the system handle network failures when uploading runtime data or save files? → A: Fail gracefully with error message, cache data locally, allow manual retry via UI button
- Q: What happens to the progress dialog during application runtime? → A: Minimize/hide during runtime, reappear automatically when app exits for post-processing
- Q: Should the system prevent launching an app that is already running? → A: Yes, detect running instance and show warning dialog with option to continue anyway
- Q: What is the default timeout value for application launch detection? → A: 30 seconds (configurable per app package)

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Launch Application and Track Runtime (Priority: P1)

Users need to launch their installed applications directly from the app management interface and have the system automatically track how long they use each application. This provides users with usage statistics to understand their gaming habits.

**Why this priority**: This is the core functionality that enables the primary use case - launching applications and collecting usage data. Without this, the feature has no value.

**Independent Test**: Can be fully tested by selecting an app package with configured launch settings, clicking the launch button, running the app for a measurable duration (e.g., 30 seconds), closing it normally, and verifying the runtime was recorded and displayed to the user.

**Acceptance Scenarios**:

1. **Given** a user has selected an app package with valid launch configuration, **When** they click the "Start App" button, **Then** the application launches successfully and begins runtime tracking
2. **Given** an application is running and being tracked, **When** the user closes the application normally, **Then** the total runtime is recorded and reported to the server
3. **Given** runtime data has been uploaded, **When** the user views the app package details, **Then** the total accumulated runtime is displayed accurately

---

### User Story 2 - Handle Application Launch Errors (Priority: P2)

Users need clear feedback when an application fails to launch, with appropriate error messages indicating what went wrong (missing files, invalid paths, permission issues, etc.).

**Why this priority**: Error handling is critical for user experience and troubleshooting, but the core launch functionality (P1) must exist first.

**Independent Test**: Can be tested by configuring an app package with an invalid executable path, attempting to launch, and verifying that a clear, actionable error message is displayed without crashing the system.

**Acceptance Scenarios**:

1. **Given** an app package has an invalid or missing executable path, **When** the user attempts to launch it, **Then** a clear error message is displayed indicating the file was not found
2. **Given** the application starts but the tracking process fails, **When** the launch attempt occurs, **Then** the user is notified of the tracking failure
3. **Given** a launch timeout occurs, **When** the configured timeout period expires without the app starting, **Then** the user receives a timeout notification and can retry

---

### User Story 3 - Monitor Process by Name When Multiple Instances Exist (Priority: P2)

For applications that spawn multiple processes or launcher applications that start the actual game executable, users need the system to track the correct process by monitoring a specified process name instead of just the initial launched process.

**Why this priority**: Many modern games use launchers that immediately exit after starting the actual game. This ensures accurate runtime tracking for such scenarios.

**Independent Test**: Can be tested by configuring an app package in "process listen mode" with a specific process name, launching an app that spawns child processes, and verifying the system tracks the child process runtime instead of the short-lived launcher.

**Acceptance Scenarios**:

1. **Given** an app package is configured with "process listen mode" enabled and a target process name, **When** the application is launched, **Then** the system monitors for the specified process name to appear
2. **Given** the target process is running, **When** the monitored process exits, **Then** the runtime is calculated from when the process appeared to when it exited
3. **Given** the target process never appears within the timeout period, **When** the timeout expires, **Then** the user is notified that the target process was not detected

---

### User Story 4 - Handle Abnormal Application Exit (Priority: P3)

Users need the option to decide whether to upload saved game data when an application exits abnormally (non-zero exit code), allowing them to avoid potentially corrupted saves.

**Why this priority**: This protects user data integrity but is less critical than the core launch and tracking functionality.

**Independent Test**: Can be tested by launching an app, forcefully terminating it (simulating a crash), and verifying the user is prompted to confirm whether to upload the game save despite the abnormal exit.

**Acceptance Scenarios**:

1. **Given** an application exits with a non-zero exit code, **When** the runtime tracking completes, **Then** the user is prompted to confirm whether to upload current save data
2. **Given** the user chooses not to upload after abnormal exit, **When** they decline the prompt, **Then** the runtime is still recorded but save data upload is skipped
3. **Given** the user confirms upload despite abnormal exit, **When** they accept the prompt, **Then** both runtime and save data are uploaded normally

---

### User Story 5 - Display Launch Progress and Post-Processing Status (Priority: P3)

Users need visual feedback during the launch sequence, runtime tracking, and post-processing phases (uploading save data, reporting statistics) so they understand what the system is doing and aren't left wondering if it's frozen.

**Why this priority**: Enhances user experience but the feature is functional without it.

**Independent Test**: Can be tested by launching an app and observing that a progress dialog appears with appropriate status messages during each phase (launching, tracking, uploading, finalizing).

**Acceptance Scenarios**:

1. **Given** the user initiates an app launch, **When** the launch sequence begins, **Then** a progress dialog displays "Starting [AppName]..."
2. **Given** the application is running, **When** the user closes it, **Then** the progress dialog updates to show "Reporting runtime..." and "Uploading game save file..."
3. **Given** all post-processing is complete, **When** operations finish, **Then** the progress dialog shows "Finalizing..." briefly before closing and displaying a success message

---

### Edge Cases

- **Already Running**: System detects if target app is already running, displays warning dialog with app name and PID, offers "Launch Anyway" and "Cancel" options
- **Never-Exiting Apps**: User can manually stop tracking via UI control; system logs warning if session exceeds 24 hours
- **Special Characters in Paths**: System handles paths with spaces, Unicode characters, and special symbols using proper escaping
- **Network Failures**: Runtime data and save files cached locally with "Upload Failed" indicator; user can retry via "Sync Now" button in app details view
- **Multiple Simultaneous Launches**: Allowed; each app tracked independently with separate progress dialogs
- **Administrative Privileges**: Launch fails with clear error message indicating elevation required; user must configure "Run as Administrator" in app settings
- **Missing Working Directory**: Detected during FR-016 validation; launch button disabled with tooltip explaining "Working directory not found: [path]"
- **Launch Timeout**: After 30 seconds (or configured value), user receives "Launch timeout" dialog with options to "Wait Longer" (add 30s) or "Cancel"
- **ProcessTimeMonitor Unavailable**: Application startup fails with error message directing user to install NuGet package; feature non-functional until resolved
- **Save Data Upload Failures**: Displayed in progress dialog; user prompted to retry or skip; failed uploads cached locally for later retry

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST provide a "Launch App" button that initiates the application launch sequence when clicked
- **FR-002**: System MUST start the application using the configured executable path and working directory from the app package settings
- **FR-003**: System MUST track the application runtime from when the process starts to when it exits
- **FR-004**: System MUST support two process monitoring modes: direct process tracking and process name monitoring
- **FR-005**: System MUST record the start time, end time, and duration of each application session
- **FR-006**: System MUST report runtime statistics to the remote server after each application session
- **FR-007**: System MUST capture the application exit code to determine if it exited normally
- **FR-008**: System MUST prompt users for confirmation when uploading save data after abnormal application exit
- **FR-009**: System MUST display progress dialogs showing current operation status during launch, tracking, and post-processing phases
- **FR-010**: System MUST handle launch failures gracefully with appropriate error messages
- **FR-011**: System MUST support configurable timeout for application launch detection
- **FR-012**: System MUST automatically create and upload compressed save data archives after successful application sessions
- **FR-013**: System MUST refresh displayed runtime statistics after each session completes
- **FR-014**: Users MUST be able to configure whether to use shell execution for launching applications
- **FR-015**: Users MUST be able to specify a custom process name for monitoring (when in process listen mode)
- **FR-016**: System MUST validate that required app package settings exist before allowing launch (minimum: executable path and working directory must be configured and exist on disk)
- **FR-017**: System MUST ensure authentication is valid before uploading runtime data or save files
- **FR-018**: System MUST cache runtime data and save files locally when network upload fails, allowing manual retry
- **FR-019**: System MUST hide/minimize progress dialog during application runtime and automatically reappear for post-processing
- **FR-020**: System MUST detect if the target application is already running and warn users before launching a second instance
- **FR-021**: System MUST use a default launch detection timeout of 30 seconds, configurable per app package

### Key Entities

- **App Package**: Represents an installable instance of an application, containing launch configuration (executable path, working directory, process monitoring settings)
- **App Package Setting**: Local configuration data for launching an app package. Required fields: executable path (must exist on disk), working directory (must exist on disk). Optional fields: monitoring mode (default: direct process tracking), process name (for listen mode), shell execution flag (default: false), launch timeout (default: 30 seconds)
- **Runtime Session**: A single execution session of an application, with start time, end time, duration, and exit code
- **Save Data Archive**: Compressed backup of application save data captured after each session
- **Process Monitor**: Service responsible for tracking process lifecycle and measuring execution time
- **Cached Upload Data**: Runtime statistics and save files stored locally when network upload fails, pending manual retry

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Users can successfully launch 95% of properly configured applications on first attempt
- **SC-002**: Runtime tracking accuracy within 1 second of actual application runtime for sessions longer than 30 seconds
- **SC-003**: System handles application launch and complete session tracking cycle in under 5 seconds overhead (excluding actual app runtime)
- **SC-004**: Error messages for failed launches provide actionable information enabling users to resolve 80% of configuration issues without external support
- **SC-005**: Users receive visual feedback within 500ms of initiating any operation (button click, app exit)
- **SC-006**: Runtime data successfully uploads to server for 98% of sessions with network connectivity
- **SC-007**: Process listen mode successfully tracks target processes for 90% of multi-process applications when correctly configured

## Assumptions

- The ProcessTimeMonitor library (TuiHub.ProcessTimeMonitorLibrary) is available via NuGet and provides methods for process lifecycle tracking
- App package settings are stored in a local database accessible via ApplicationDbContext
- The system has established network connectivity and authentication with the remote server for uploading statistics
- Users have necessary file system permissions to access configured application paths
- The grpc channel (GlobalContext.GrpcChannel) is available for server communication
- A SavedataManager service exists for creating compressed save data archives
- Standard Windows process exit codes apply (0 = success, non-zero = error)
- The system uses a progress dialog UI component (ProgressBarWindow) for showing operation status
- File transfer chunk size is configured in system settings (GlobalContext.SystemConfig.FileTransferChunkBytes)
- Cache directory paths are configured and writable (GlobalContext.SystemConfig.GetRealCacheDirPath())

## Dependencies

- ProcessTimeMonitor library for process lifetime tracking
- LibrarianSephirahService for server communication (runtime reporting, save file upload)
- SavedataManager for creating compressed save archives
- ApplicationDbContext for reading app package settings
- EnsureLoginHelper for authentication validation
- System.Diagnostics.Process for application launching
- SHA256 hashing for file integrity verification
