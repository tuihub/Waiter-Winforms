# Research: Persistent Task Queue

**Feature Branch**: `002-persistent-task-queue`  
**Date**: 2026-01-31  
**Status**: Complete

This document captures research findings for implementing persistent task queue functionality.

---

## 1. Task Serialization Approach

### Decision: System.Text.Json with Type Discriminator

**Rationale**: The existing codebase uses .NET 8.0 which includes System.Text.Json with polymorphic serialization support. This is simpler than adding Newtonsoft.Json and aligns with modern .NET practices.

**Implementation Pattern**:
```csharp
// Use JsonDerivedType attribute for polymorphic serialization
[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(DownloadTaskDetails), "download")]
[JsonDerivedType(typeof(SyncSaveTaskDetails), "syncsave")]
public abstract class TaskDetailsBase { }
```

**Alternatives Considered**:
- Newtonsoft.Json: More mature but adds unnecessary dependency
- Manual serialization: Error-prone and verbose
- Protobuf: Overkill for local storage

---

## 2. EF Core Migration Strategy

### Decision: Use EnsureCreated (No Migrations)

**Rationale**: Per spec assumptions, "Migration management is not a concern for this feature; the new table will be created via EnsureCreated or manual schema addition." The existing codebase uses `EnsureCreated` in `DatabaseService.InitializeAsync()`.

**Implementation**:
- Add `DbSet<PersistentTask>` to `WaiterDbContext`
- Configure entity in `OnModelCreating`
- Existing `EnsureCreated` call will create the new table

**Alternatives Considered**:
- EF Core Migrations: More robust but adds complexity; deferred to future
- Raw SQL: Harder to maintain

---

## 3. Duplicate Task Detection

### Decision: Composite Key Based on Task Type + Target Identifier

**Rationale**: Spec FR-009 requires preventing duplicate tasks for the same target. Each task type has a unique target identifier:
- Download: `downloadUrl` + `destinationPath`
- SyncSave: `appId` + `syncDirection`

**Implementation**:
```csharp
// Generate a unique key for duplicate detection
public string GenerateTaskKey(BackgroundTaskType type, object details) => type switch
{
    BackgroundTaskType.Download => $"download:{((DownloadTaskDetails)details).DownloadUrl}:{((DownloadTaskDetails)details).DestinationPath}",
    BackgroundTaskType.SyncSave => $"syncsave:{((SyncSaveTaskDetails)details).AppId}:{((SyncSaveTaskDetails)details).SyncDirection}",
    _ => $"other:{Guid.NewGuid()}"
};
```

**Alternatives Considered**:
- Hash-based: More compact but harder to debug
- GUID only: Doesn't prevent duplicates

---

## 4. Interrupted Task Detection

### Decision: Mark "Running" Status as "Interrupted" on Startup

**Rationale**: Spec FR-005 requires marking interrupted tasks (status was "Running" when app closed) as retryable. This is detected by checking for tasks with status "Running" when the application starts.

**Implementation**:
1. On startup, query all tasks with status `Running`
2. Update their status to `Interrupted`
3. Set `NeedsRetry = true`
4. These tasks will appear in the UI as retryable

**Alternatives Considered**:
- Heartbeat mechanism: Overkill for desktop app
- Transaction log: Too complex

---

## 5. Startup Performance

### Decision: Background Loading with Early UI Display

**Rationale**: Spec SC-002 requires startup delay <500ms for up to 50 tasks. Loading tasks synchronously could block UI.

**Implementation**:
1. Application starts normally
2. BackgroundTasksForm loads with empty list
3. `PersistentTaskService` loads persisted tasks on background thread
4. Tasks are added to UI incrementally via events
5. Loading indicator shows during load

**Performance Considerations**:
- SQLite query for 50 tasks: ~10-50ms
- JSON deserialization per task: ~1ms
- Total expected: <100ms, well under 500ms limit

**Alternatives Considered**:
- Lazy loading on form open: Delays first view
- Pagination: Unnecessary for expected scale (<100 tasks)

---

## 6. Database Corruption Handling

### Decision: Try/Catch with Fallback to In-Memory Only

**Rationale**: Spec edge case requires graceful handling when database is corrupted or inaccessible. The app should continue functioning with in-memory tasks.

**Implementation**:
```csharp
public async Task<List<PersistentTask>> LoadPersistedTasksAsync()
{
    try
    {
        return await _context.PersistentTasks.ToListAsync();
    }
    catch (Exception ex)
    {
        _logger.LogWarning(ex, "Failed to load persisted tasks, continuing with in-memory only");
        return new List<PersistentTask>();
    }
}
```

**Logging**: Warning-level log when persistence fails, allows app to continue.

**Alternatives Considered**:
- Database recovery: Too complex for this feature
- User notification: Could be added later if needed

---

## 7. Task Status Flow

### Decision: Explicit State Machine

**Rationale**: Clear state transitions prevent invalid states and simplify retry logic.

```text
                    ┌─────────────┐
                    │   Pending   │
                    └──────┬──────┘
                           │ StartTask()
                           ▼
                    ┌─────────────┐
      ┌────────────►│   Running   │◄────────────┐
      │             └──────┬──────┘             │
      │                    │                    │
      │         ┌──────────┼──────────┐         │
      │         │          │          │         │
      │         ▼          ▼          ▼         │
      │  ┌───────────┐ ┌───────┐ ┌──────────┐   │
      │  │ Completed │ │Failed │ │Cancelled │   │
      │  └───────────┘ └───┬───┘ └──────────┘   │
      │                    │                    │
      │                    │ RetryTask()        │
      │                    └────────────────────┘
      │
      │  App restart while Running
      │             ┌─────────────┐
      └─────────────│ Interrupted │
                    └─────────────┘
```

**Valid Status Transitions**:
- Pending → Running (StartTask)
- Running → Completed, Failed, Cancelled
- Failed → Running (RetryTask - resets to Pending then starts)
- Interrupted → Running (RetryTask)

---

## 8. Retry Mechanism

### Decision: Create New Task from Persisted Parameters

**Rationale**: Spec FR-011 requires explicit user action for retry (no auto-retry). The retry operation deserializes stored parameters and creates a fresh task execution.

**Implementation**:
1. User clicks "Retry" on failed/interrupted task
2. Service deserializes `ParametersJson` to task details
3. Service creates new in-memory task with same parameters
4. Original persisted task is updated to `Running` status
5. Task executes normally through BackgroundTaskService

**Alternatives Considered**:
- Resume from checkpoint: Too complex, not all tasks support it
- Auto-retry with backoff: Explicitly excluded by spec

---

## 9. Integration with Existing BackgroundTaskService

### Decision: Event-Based Integration (Observer Pattern)

**Rationale**: The existing `BackgroundTaskService` has events (`TaskAdded`, `TaskUpdated`, `TaskCompleted`, `TaskFailed`). The new `PersistentTaskService` subscribes to these events to persist changes without modifying the existing service significantly.

**Implementation**:
```csharp
public class PersistentTaskService
{
    public PersistentTaskService(BackgroundTaskService backgroundTaskService)
    {
        backgroundTaskService.TaskAdded += OnTaskAdded;
        backgroundTaskService.TaskUpdated += OnTaskUpdated;
        backgroundTaskService.TaskCompleted += OnTaskCompleted;
        backgroundTaskService.TaskFailed += OnTaskFailed;
    }
    
    private async void OnTaskAdded(object? sender, TaskEventArgs e)
    {
        await PersistTaskAsync(e.Task);
    }
    // ... similar for other events
}
```

**Alternatives Considered**:
- Modify BackgroundTaskService directly: More invasive, harder to test
- Wrapper/Decorator: Over-engineered for this use case

---

## 10. UI Changes for BackgroundTasksForm

### Decision: Add Retry Button and Load Persisted Tasks

**Rationale**: Minimal UI changes to existing form. Add context-sensitive "Retry" button that appears for failed/interrupted tasks.

**UI Changes**:
1. Add "Retry" button (disabled by default)
2. Enable "Retry" when selected task is Failed or Interrupted
3. Load persisted tasks on form load (in addition to in-memory tasks)
4. Add status column distinction for "Interrupted" tasks

**Flow**:
1. Form loads → calls `PersistentTaskService.LoadPersistedTasksAsync()`
2. Persisted tasks are restored to `BackgroundTaskService`
3. UI displays combined in-memory and persisted tasks
4. User selects failed task → Retry button enables
5. User clicks Retry → `PersistentTaskService.RetryTaskAsync(taskId)`

---

## Summary of Key Decisions

| Area | Decision | Rationale |
|------|----------|-----------|
| Serialization | System.Text.Json with polymorphism | Built-in, modern .NET |
| Schema | EnsureCreated | Per spec assumptions |
| Duplicates | Composite key from type + target | Simple, debuggable |
| Interrupts | Mark Running→Interrupted on startup | Clear user feedback |
| Startup | Background loading | <500ms requirement |
| Corruption | Try/catch, fallback to in-memory | Graceful degradation |
| Integration | Event subscription | Non-invasive |
| Retry | Manual only, recreate from params | Per spec FR-011 |
