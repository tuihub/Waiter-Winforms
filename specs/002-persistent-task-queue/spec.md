# Feature Specification: Persistent Task Queue

**Feature Branch**: `002-persistent-task-queue`  
**Created**: 2026-01-31  
**Status**: Draft  
**Input**: User description: "添加任务队列功能，要求失败的任务可以重试（在应用重启后也可以恢复重试），在数据库中持久化，任务完成时进行标记或删除；注意可以暂时不考虑migration（尚未开始统一管理）"

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Retry Failed Task After App Restart (Priority: P1)

As a user, when a background task (such as downloading an app or syncing save data) fails and I restart the application, I want to be able to see the failed task and retry it without having to manually re-initiate the operation.

**Why this priority**: This is the core value proposition of the feature - ensuring users don't lose progress on long-running operations due to transient failures or app restarts. This directly addresses the user's primary requirement.

**Independent Test**: Can be fully tested by creating a download task, simulating a failure, restarting the app, and verifying the failed task appears with a retry option.

**Acceptance Scenarios**:

1. **Given** a download task that failed due to a network error, **When** the user restarts the application, **Then** the failed task is displayed in the background tasks list with its original parameters and a "Retry" option.
2. **Given** a sync save task that failed, **When** the user clicks "Retry" on the task, **Then** the task is re-executed with the same configuration as the original attempt.
3. **Given** multiple failed tasks from previous sessions, **When** the application starts, **Then** all retryable failed tasks are loaded from the database and displayed.

---

### User Story 2 - Task Persistence During Execution (Priority: P2)

As a user, I want my running tasks to be saved to the database so that if the application crashes unexpectedly, I can see what tasks were in progress when I restart.

**Why this priority**: Persistence during execution is important for crash recovery, but is secondary to the core retry functionality since crashes are less frequent than voluntary restarts.

**Independent Test**: Can be tested by creating a task, forcefully terminating the app while the task is running, restarting, and verifying the interrupted task is shown.

**Acceptance Scenarios**:

1. **Given** a task is created, **When** the task begins execution, **Then** the task state is persisted to the database.
2. **Given** a task is running and the app is forcefully closed, **When** the user restarts the application, **Then** the interrupted task is displayed with status indicating it needs retry.
3. **Given** a task's progress is updated, **When** the progress changes significantly (e.g., every 10%), **Then** the new progress is saved to the database.

---

### User Story 3 - Completed Task Cleanup (Priority: P3)

As a user, I want completed tasks to be marked as complete and optionally removed from the database to keep the task list manageable and storage minimal.

**Why this priority**: Cleanup is a maintenance feature that improves long-term usability but doesn't affect core functionality.

**Independent Test**: Can be tested by completing several tasks, verifying they're marked complete, and using the clear function to remove them.

**Acceptance Scenarios**:

1. **Given** a task completes successfully, **When** the task finishes, **Then** the task is marked as "Completed" in the database.
2. **Given** completed tasks exist in the database, **When** the user clicks "Clear Completed Tasks", **Then** all completed tasks are removed from the database.
3. **Given** the application has been running for a while, **When** checking database size, **Then** completed tasks don't accumulate indefinitely (old completed tasks can be auto-cleaned based on age).

---

### User Story 4 - View Task History (Priority: P4)

As a user, I want to see the history of past tasks including when they ran and whether they succeeded or failed.

**Why this priority**: Task history is a "nice to have" feature for debugging and auditing but not essential for core retry functionality.

**Independent Test**: Can be tested by running several tasks, then viewing the task history list to verify all tasks are recorded with accurate timestamps.

**Acceptance Scenarios**:

1. **Given** several tasks have been executed over time, **When** the user views the task history, **Then** a list of past tasks is displayed with their name, type, status, and timestamps.
2. **Given** a task failed multiple times before succeeding, **When** viewing task history, **Then** each attempt is recorded with its result.

---

### Edge Cases

- What happens when the database file is corrupted or inaccessible?
  - The app should continue to function with in-memory tasks only and log a warning.
- What happens when a task's required resources (e.g., download URL) are no longer valid when retrying?
  - The retry should fail gracefully with a clear error message explaining the resource is unavailable.
- What happens when a task is retried while another instance of the same task type is already running?
  - The system should prevent duplicate tasks for the same target (e.g., same app download).
- What happens when the user manually cancels a task?
  - Cancelled tasks should be marked as cancelled in the database and not be automatically retried.
- What happens when there are many (100+) persisted tasks during app startup?
  - The loading should not significantly delay app startup; consider loading tasks lazily or in background.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST persist task state (including all parameters needed for retry) to the database when a task is created.
- **FR-002**: System MUST update the persisted task state when task status changes (started, progress update, completed, failed, cancelled).
- **FR-003**: System MUST load all non-completed tasks from the database when the application starts.
- **FR-004**: System MUST provide a mechanism for users to retry failed tasks.
- **FR-005**: System MUST mark interrupted tasks (status was "Running" when app closed) as "Interrupted" and make them retryable.
- **FR-006**: System MUST store sufficient task details (task type, parameters, configuration) to recreate and retry the task.
- **FR-007**: System MUST allow users to clear completed and failed tasks from the database.
- **FR-008**: System MUST track retry count for each task.
- **FR-009**: System MUST prevent creation of duplicate tasks for the same target (e.g., same download URL or same app sync).
- **FR-010**: System MUST mark tasks as "Completed" or "Failed" and persist this final state.
- **FR-011**: System MUST NOT auto-retry tasks automatically; retry requires explicit user action.
- **FR-012**: System MUST preserve the original task creation time and track last retry time separately.

### Key Entities

- **PersistentTask**: Represents a task that can be persisted and restored. Contains task metadata (ID, name, type, status), execution state (progress, retry count, start/end times), and serialized task parameters needed for retry.
- **TaskParameters**: Serialized/structured data containing all information needed to re-execute a specific task type (e.g., download URL, destination path for downloads; app ID, sync direction for save syncs).

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Failed tasks can be retried after app restart within 3 clicks (open task list → select task → click retry).
- **SC-002**: App startup time increases by no more than 500ms when loading up to 50 persisted tasks.
- **SC-003**: 100% of task state changes are persisted within 1 second of occurrence.
- **SC-004**: Users can clear all completed tasks in a single action.
- **SC-005**: Retried tasks complete successfully at the same rate as newly created tasks (retry mechanism doesn't introduce new failure modes).
- **SC-006**: Task persistence survives unexpected app termination (crash, force quit, power loss) with data loss limited to changes in the last 1 second.

## Assumptions

- The existing `BackgroundTaskService` will be extended or wrapped rather than replaced, maintaining backward compatibility with existing task creation code.
- SQLite database is already set up and accessible; no new database configuration is required.
- Migration management is not a concern for this feature; the new table will be created via EnsureCreated or manual schema addition.
- Task parameters can be serialized to JSON for storage.
- The existing `TaskHistory` model may be repurposed or a new model created specifically for retryable persistent tasks.
- Maximum reasonable number of concurrent tasks is under 100.
