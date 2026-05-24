# Quickstart: Persistent Task Queue Implementation

**Feature Branch**: `002-persistent-task-queue`  
**Date**: 2026-01-31  
**Estimated Time**: 6-8 hours

---

## Prerequisites

- [ ] Branch `002-persistent-task-queue` checked out
- [ ] Application builds successfully (`dotnet build`)
- [ ] Spec and plan documents reviewed

---

## Implementation Order

Follow this order to ensure dependencies are satisfied:

### Phase A: Data Layer (1-2 hours)

#### A1. Create PersistentTask Entity

**File**: `Waiter/Data/Models/PersistentTask.cs`

1. Create new file with entity definition from [data-model.md](data-model.md)
2. Include all properties: Id, TaskId, TaskKey, Name, TaskType, Status, etc.
3. Add XML documentation comments

**Verification**: File compiles without errors

#### A2. Create EF Configuration

**File**: `Waiter/Data/Configurations/PersistentTaskConfiguration.cs`

1. Create new configuration file
2. Define table name, indexes, and constraints
3. Set up unique index on TaskId and TaskKey

**Verification**: File compiles without errors

#### A3. Update WaiterDbContext

**File**: `Waiter/Data/WaiterDbContext.cs`

1. Add `DbSet<PersistentTask> PersistentTasks { get; set; }`
2. Add `modelBuilder.ApplyConfiguration(new PersistentTaskConfiguration());` in OnModelCreating

**Verification**: Application starts and creates database with new table

---

### Phase B: Task Parameters (30 min)

#### B1. Create Task Parameters Classes

**File**: `Waiter/Services/TaskParameters.cs` (new file)

1. Create `TaskParametersBase` abstract class with `[JsonPolymorphic]` attribute
2. Create `DownloadTaskParameters` class deriving from base
3. Create `SyncSaveTaskParameters` class deriving from base
4. Implement `GenerateTaskKey()` for each type

**Verification**: JSON serialization round-trips correctly (manual test)

---

### Phase C: Service Layer (2-3 hours)

#### C1. Add Interrupted Status

**File**: `Waiter/Services/BackgroundTaskService.cs`

1. Add `Interrupted` value to `TaskStatus` enum
2. No other changes to existing enum values

**Verification**: Compiles, existing code still works

#### C2. Extend BackgroundTaskService

**File**: `Waiter/Services/BackgroundTaskService.cs`

1. Add `RestoreTask(BackgroundTask task)` method
2. Add `GetTask(string taskId)` method
3. These methods don't trigger events (for persistence restore)

**Verification**: New methods accessible, existing behavior unchanged

#### C3. Create PersistentTaskService

**File**: `Waiter/Services/PersistentTaskService.cs` (new file)

1. Implement `IPersistentTaskService` interface
2. Constructor takes DbContext, BackgroundTaskService, optional ILogger
3. Subscribe to BackgroundTaskService events in constructor
4. Implement all interface methods per [contracts/service-contracts.md](contracts/service-contracts.md)

Key methods:
- `InitializeAsync()`: Mark interrupted tasks, load all non-completed
- `PersistTaskAsync()`: Save new task with parameters JSON
- `UpdateTaskAsync()`: Update status/progress
- `RetryTaskAsync()`: Deserialize parameters, create new execution

**Verification**: Unit tests pass (if written), manual test with breakpoints

#### C4. Create Exception Types

**File**: `Waiter/Services/PersistentTaskService.cs` (same file or separate)

1. Add `DuplicateTaskException` class
2. Add `PersistentTaskEventArgs` class
3. Add `PersistentTaskOperation` enum

---

### Phase D: DI Registration (15 min)

#### D1. Register Services

**File**: `Waiter/Program.cs`

1. Locate service registration section
2. Add: `services.AddSingleton<IPersistentTaskService, PersistentTaskService>();`
3. Ensure DbContext is available for injection

**Verification**: Application starts without DI errors

#### D2. Initialize on Startup

**File**: `Waiter/Program.cs` or `Waiter/Forms/MainForm.cs`

1. After service registration, call `IPersistentTaskService.InitializeAsync()`
2. Restore loaded tasks to BackgroundTaskService
3. Handle any startup errors gracefully

**Verification**: Previously persisted tasks appear after app restart

---

### Phase E: UI Layer (1-2 hours)

#### E1. Add Retry Button to Designer

**File**: `Waiter/Forms/BackgroundTasksForm.Designer.cs`

1. Open form in Visual Studio Designer
2. Add "Retry" button (`_btnRetry`)
3. Position next to existing "Clear" button
4. Set initial Enabled = false

**Verification**: Form opens in Designer without errors

#### E2. Update BackgroundTasksForm Code

**File**: `Waiter/Forms/BackgroundTasksForm.cs`

1. Add `IPersistentTaskService` dependency (constructor or SetDependencies method)
2. Add `_btnRetry_Click` event handler
3. Add `UpdateRetryButtonState()` method
4. Call `UpdateRetryButtonState()` on selection changed
5. Update `SetItemColor()` to handle `Interrupted` status

**Verification**: Retry button enables/disables based on selection

#### E3. Wire Up Event Handlers

**File**: `Waiter/Forms/BackgroundTasksForm.cs`

1. Subscribe to `_listViewTasks.SelectedIndexChanged`
2. Call retry logic in `_btnRetry_Click`
3. Show error message if retry fails

**Verification**: Can retry failed/interrupted tasks via UI

---

### Phase F: Integration Testing (1 hour)

#### F1. Manual Test: Basic Persistence

1. Start app, create a download task
2. Close app before task completes
3. Restart app
4. Verify: Task appears with "Interrupted" status

#### F2. Manual Test: Retry Flow

1. Have a failed task (simulate network error)
2. Open background tasks form
3. Select failed task
4. Click Retry button
5. Verify: Task restarts with same parameters

#### F3. Manual Test: Duplicate Prevention

1. Create a download task for URL X
2. Try to create another download task for URL X
3. Verify: Error message shown, duplicate prevented

#### F4. Manual Test: Clear Completed

1. Have several completed and failed tasks
2. Click "Clear Completed" button
3. Verify: Completed tasks removed, failed tasks remain
4. Restart app
5. Verify: Cleared tasks don't reappear

---

## File Checklist

| File | Action | Status |
|------|--------|--------|
| `Data/Models/PersistentTask.cs` | Create | ☐ |
| `Data/Configurations/PersistentTaskConfiguration.cs` | Create | ☐ |
| `Data/WaiterDbContext.cs` | Modify | ☐ |
| `Services/TaskParameters.cs` | Create | ☐ |
| `Services/BackgroundTaskService.cs` | Modify | ☐ |
| `Services/PersistentTaskService.cs` | Create | ☐ |
| `Program.cs` | Modify | ☐ |
| `Forms/BackgroundTasksForm.Designer.cs` | Modify (Designer) | ☐ |
| `Forms/BackgroundTasksForm.cs` | Modify | ☐ |

---

## Common Issues

### Issue: Database not creating new table
**Solution**: Delete existing `waiter.db` file or call `EnsureDeleted()` then `EnsureCreated()` during development

### Issue: JSON deserialization fails
**Solution**: Ensure `[JsonDerivedType]` attributes are correctly specified with matching type discriminator strings

### Issue: Tasks not appearing after restart
**Solution**: Check that `InitializeAsync()` is called during startup and tasks are restored to BackgroundTaskService

### Issue: Duplicate task detection not working
**Solution**: Verify `TaskKey` is generated consistently (same format for same parameters)

---

## Success Criteria

- [ ] Failed tasks persist across app restarts
- [ ] Interrupted tasks (was Running) show as Interrupted after restart
- [ ] Retry button works for Failed and Interrupted tasks
- [ ] Duplicate tasks are prevented
- [ ] Clear Completed removes tasks from database
- [ ] App startup time remains <500ms with 50 tasks
- [ ] No UI freezing during database operations
