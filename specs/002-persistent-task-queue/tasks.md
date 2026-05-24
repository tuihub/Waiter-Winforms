# Tasks: Persistent Task Queue

**Feature Branch**: `002-persistent-task-queue`  
**Date**: 2026-01-31  
**Input**: Design documents from `/specs/002-persistent-task-queue/`  
**Prerequisites**: plan.md ✅, spec.md ✅, research.md ✅, data-model.md ✅, contracts/ ✅, quickstart.md ✅

**Tests**: Not explicitly requested in feature specification - tests are omitted per YAGNI principle.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

---

## Format: `[ID] [P?] [Story?] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3, US4)
- Include exact file paths in descriptions

## Path Conventions

Per plan.md structure:
- **Models**: `Waiter/Data/Models/`
- **Configurations**: `Waiter/Data/Configurations/`
- **Services**: `Waiter/Services/`
- **Forms**: `Waiter/Forms/`
- **DI/Startup**: `Waiter/Program.cs`

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization and verify development environment

- [X] T001 Verify branch `002-persistent-task-queue` is checked out and builds successfully
- [X] T002 Review existing BackgroundTaskService.cs events in Waiter/Services/BackgroundTaskService.cs

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core data layer and infrastructure that MUST be complete before ANY user story can be implemented

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

### Data Layer

- [X] T003 [P] Create PersistentTask entity model in Waiter/Data/Models/PersistentTask.cs
- [X] T004 [P] Create PersistentTaskConfiguration EF config in Waiter/Data/Configurations/PersistentTaskConfiguration.cs
- [X] T005 Update WaiterDbContext with DbSet<PersistentTask> and configuration in Waiter/Data/WaiterDbContext.cs

### Task Parameters Serialization

- [X] T006 [P] Create TaskParametersBase abstract class with JSON polymorphism in Waiter/Services/TaskParameters.cs
- [X] T007 [P] Create DownloadTaskParameters class deriving from TaskParametersBase in Waiter/Services/TaskParameters.cs
- [X] T008 [P] Create SyncSaveTaskParameters class deriving from TaskParametersBase in Waiter/Services/TaskParameters.cs

### BackgroundTaskService Extensions

- [X] T009 Add Interrupted status to TaskStatus enum in Waiter/Services/BackgroundTaskService.cs
- [X] T010 Add RestoreTask(BackgroundTask task) method to BackgroundTaskService in Waiter/Services/BackgroundTaskService.cs
- [X] T011 Add GetTask(string taskId) method to BackgroundTaskService in Waiter/Services/BackgroundTaskService.cs

### Service Infrastructure

- [X] T012 Create DuplicateTaskException class in Waiter/Services/PersistentTaskService.cs
- [X] T013 Create PersistentTaskEventArgs class in Waiter/Services/PersistentTaskService.cs
- [X] T014 Create PersistentTaskOperation enum in Waiter/Services/PersistentTaskService.cs
- [X] T015 Create IPersistentTaskService interface in Waiter/Services/PersistentTaskService.cs
- [X] T016 Create PersistentTaskService class skeleton with constructor and DI in Waiter/Services/PersistentTaskService.cs

**Checkpoint**: Foundation ready - database entity created, task parameters serialization ready, service infrastructure in place. User story implementation can now begin.

---

## Phase 3: User Story 1 - Retry Failed Task After App Restart (Priority: P1) 🎯 MVP

**Goal**: Enable users to see and retry failed tasks after application restart

**Independent Test**: Create a download task, simulate a failure, restart the app, verify the failed task appears with a retry option, click retry and verify task re-executes.

### Core Persistence Implementation

- [X] T017 [US1] Implement PersistTaskAsync method in Waiter/Services/PersistentTaskService.cs
- [X] T018 [US1] Implement IsDuplicateTaskAsync method in Waiter/Services/PersistentTaskService.cs
- [X] T019 [US1] Implement GetAllPersistedTasksAsync method in Waiter/Services/PersistentTaskService.cs

### Event Handlers for Automatic Persistence

- [X] T020 [US1] Implement OnBackgroundTaskAdded event handler in Waiter/Services/PersistentTaskService.cs
- [X] T021 [US1] Implement OnBackgroundTaskFailed event handler in Waiter/Services/PersistentTaskService.cs

### Retry Capability

- [X] T022 [US1] Implement FailTaskAsync method in Waiter/Services/PersistentTaskService.cs
- [X] T023 [US1] Implement RetryTaskAsync method with parameter deserialization in Waiter/Services/PersistentTaskService.cs

### Startup Loading

- [X] T024 [US1] Implement InitializeAsync method to load failed tasks on startup in Waiter/Services/PersistentTaskService.cs

### DI Registration & Startup

- [X] T025 [US1] Register IPersistentTaskService in DI container in Waiter/Program.cs
- [X] T026 [US1] Call PersistentTaskService.InitializeAsync on application startup in Waiter/Program.cs

### UI: Retry Button

- [X] T027 [US1] Add Retry button (_btnRetry) to BackgroundTasksForm Designer in Waiter/Forms/BackgroundTasksForm.Designer.cs
- [X] T028 [US1] Add IPersistentTaskService dependency to BackgroundTasksForm in Waiter/Forms/BackgroundTasksForm.cs
- [X] T029 [US1] Implement _btnRetry_Click event handler in Waiter/Forms/BackgroundTasksForm.cs
- [X] T030 [US1] Implement UpdateRetryButtonState method for selection changes in Waiter/Forms/BackgroundTasksForm.cs
- [X] T031 [US1] Wire SelectedIndexChanged event to UpdateRetryButtonState in Waiter/Forms/BackgroundTasksForm.cs

**Checkpoint**: User Story 1 complete. Failed tasks can be seen and retried after app restart. This is a fully functional MVP.

---

## Phase 4: User Story 2 - Task Persistence During Execution (Priority: P2)

**Goal**: Persist running tasks so interrupted tasks can be recovered after crash

**Independent Test**: Create a task, forcefully terminate the app while task is running, restart, verify the interrupted task appears with "Interrupted" status and can be retried.

### Status Update Persistence

- [X] T032 [US2] Implement UpdateTaskAsync method for progress updates in Waiter/Services/PersistentTaskService.cs
- [X] T033 [US2] Implement OnBackgroundTaskUpdated event handler with throttling in Waiter/Services/PersistentTaskService.cs

### Interrupted Task Detection

- [X] T034 [US2] Add MarkInterruptedTasksAsync private method in Waiter/Services/PersistentTaskService.cs
- [X] T035 [US2] Call MarkInterruptedTasksAsync in InitializeAsync in Waiter/Services/PersistentTaskService.cs

### UI: Interrupted Status Display

- [X] T036 [US2] Update SetItemColor method to handle Interrupted status in Waiter/Forms/BackgroundTasksForm.cs
- [X] T037 [US2] Ensure Interrupted tasks are selectable for retry in Waiter/Forms/BackgroundTasksForm.cs

**Checkpoint**: User Story 2 complete. Tasks running when app crashes are marked as Interrupted and can be retried.

---

## Phase 5: User Story 3 - Completed Task Cleanup (Priority: P3)

**Goal**: Mark completed tasks and allow users to clear them from database

**Independent Test**: Complete several tasks, verify they're marked as Completed, use Clear Completed button, verify tasks are removed from database and don't reappear after restart.

### Completion Marking

- [X] T038 [US3] Implement CompleteTaskAsync method in Waiter/Services/PersistentTaskService.cs
- [X] T039 [US3] Implement OnBackgroundTaskCompleted event handler in Waiter/Services/PersistentTaskService.cs

### Clear Operations

- [X] T040 [US3] Implement ClearCompletedTasksAsync method in Waiter/Services/PersistentTaskService.cs
- [X] T041 [US3] Implement ClearFailedTasksAsync method in Waiter/Services/PersistentTaskService.cs

### UI: Clear Completed Button Enhancement

- [X] T042 [US3] Update existing Clear button click handler to call ClearCompletedTasksAsync in Waiter/Forms/BackgroundTasksForm.cs
- [X] T043 [US3] Add confirmation dialog before clearing tasks in Waiter/Forms/BackgroundTasksForm.cs

**Checkpoint**: User Story 3 complete. Completed tasks can be cleared and don't accumulate in the database.

---

## Phase 6: User Story 4 - View Task History (Priority: P4)

**Goal**: Display task history with timestamps and status information

**Independent Test**: Run several tasks (success and failure), view task list, verify all tasks are displayed with name, type, status, and timestamps.

### History Display

- [X] T044 [US4] Add CreatedAt column to task list view in Waiter/Forms/BackgroundTasksForm.Designer.cs
- [X] T045 [US4] Add CompletedAt column to task list view in Waiter/Forms/BackgroundTasksForm.Designer.cs
- [X] T046 [US4] Add RetryCount column to task list view in Waiter/Forms/BackgroundTasksForm.Designer.cs
- [X] T047 [US4] Update list item creation to populate timestamp columns in Waiter/Forms/BackgroundTasksForm.cs
- [X] T048 [US4] Display retry count badge on tasks with RetryCount > 0 in Waiter/Forms/BackgroundTasksForm.cs

**Checkpoint**: User Story 4 complete. Task history is visible with accurate timestamps and retry counts.

---

## Phase 7: Polish & Cross-Cutting Concerns

**Purpose**: Error handling, edge cases, and final validation

### Error Handling & Edge Cases

- [X] T049 [P] Add database corruption fallback (try/catch with in-memory fallback) in Waiter/Services/PersistentTaskService.cs
- [X] T050 [P] Add graceful error messages for unavailable retry resources in Waiter/Services/PersistentTaskService.cs
- [X] T051 [P] Add logging throughout PersistentTaskService operations in Waiter/Services/PersistentTaskService.cs

### Performance

- [ ] T052 Verify startup time <500ms with 50 persisted tasks (per SC-002)

### Validation

- [ ] T053 Run quickstart.md manual test F1 (Basic Persistence)
- [ ] T054 Run quickstart.md manual test F2 (Retry Flow)
- [ ] T055 Run quickstart.md manual test F3 (Duplicate Prevention)
- [ ] T056 Run quickstart.md manual test F4 (Clear Completed)

---

## Dependencies & Execution Order

### Phase Dependencies

```text
Phase 1 (Setup)
    │
    ▼
Phase 2 (Foundational) ◄── BLOCKS ALL USER STORIES
    │
    ├──────────────────────────────────────────────┐
    │                    │                    │    │
    ▼                    ▼                    ▼    ▼
Phase 3 (US1)      Phase 4 (US2)      Phase 5 (US3)  Phase 6 (US4)
    │                    │                    │    │
    └──────────────────────────────────────────────┘
                         │
                         ▼
                  Phase 7 (Polish)
```

### User Story Dependencies

| Story | Depends On | Can Parallelize With |
|-------|------------|---------------------|
| US1 (P1) | Phase 2 only | None initially (MVP) |
| US2 (P2) | Phase 2 + T024 (InitializeAsync) | US3, US4 |
| US3 (P3) | Phase 2 only | US2, US4 |
| US4 (P4) | Phase 2 only | US2, US3 |

### Within Each Phase: Task Dependencies

**Phase 2 (Foundational)**:
- T003, T004 → T005 (entity before DbContext)
- T006 → T007, T008 (base class before derived)
- T009 → T010, T011 (enum before methods using it)
- T012-T015 → T016 (supporting types before service)

**Phase 3 (US1)**:
- T017, T018, T019 can run in parallel
- T020, T021 depend on T017
- T022, T023 depend on T017, T021
- T024 depends on all service methods
- T025, T026 depend on T024
- T027-T031 depend on T025, T026

### Parallel Opportunities

**Phase 2 - Can Run in Parallel**:
```text
Group A: T003, T004, T006, T007, T008
Group B (after A): T005, T009
Group C (after B): T010, T011, T012, T013, T014
Group D (after C): T015, T016
```

**Phase 3 (US1) - Can Run in Parallel**:
```text
Group A: T017, T018, T019
Group B (after A): T020, T021, T022
Group C (after B): T023, T024
Group D (after C): T025, T026, T027
Group E (after D): T028, T029, T030, T031
```

**User Stories (after Phase 2)** - Can work on multiple stories if team capacity allows:
```text
Developer A: Phase 3 (US1) - MVP priority
Developer B: Phase 4 (US2) after T024 complete
Developer C: Phase 5 (US3) - independent
Developer D: Phase 6 (US4) - independent
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup (verify environment)
2. Complete Phase 2: Foundational (data layer + service skeleton)
3. Complete Phase 3: User Story 1 (retry failed tasks)
4. **STOP and VALIDATE**: Test US1 independently via quickstart.md F1, F2
5. This is a deployable MVP!

### Incremental Delivery

1. **Foundation** → Phase 1 + Phase 2 complete
2. **MVP (US1)** → Failed task retry works → Validate with F1, F2 tests
3. **Add US2** → Interrupted task recovery works → Validate with crash test
4. **Add US3** → Task cleanup works → Validate with F4 test
5. **Add US4** → Task history visible → Validate timestamps correct
6. **Polish** → Edge cases, performance validation

### Estimated Time (from quickstart.md)

| Phase | Estimated Time |
|-------|---------------|
| Phase 1: Setup | 15 min |
| Phase 2: Foundational | 1.5-2 hours |
| Phase 3: US1 (MVP) | 2-3 hours |
| Phase 4: US2 | 1 hour |
| Phase 5: US3 | 30 min |
| Phase 6: US4 | 30 min |
| Phase 7: Polish | 1 hour |
| **Total** | **6-8 hours** |

---

## Notes

- [P] tasks = different files, no dependencies within that group
- [USn] label maps task to specific user story for traceability
- Each user story can be tested independently after completion
- Commit after each task or logical group of tasks
- Stop at any checkpoint to validate the story independently
- Database table will be created automatically via EnsureCreated (no migrations needed)
- If database issues occur during development, delete `waiter.db` and restart

---

## Success Criteria Mapping

| Criterion | Tasks | Validation |
|-----------|-------|------------|
| SC-001: 3 clicks to retry | T027-T031 | Manual test F2 |
| SC-002: Startup <500ms | T024, T052 | Performance test |
| SC-003: State changes persist <1s | T017, T032, T038 | Manual observation |
| SC-004: Clear completed in single action | T040, T042 | Manual test F4 |
| SC-005: Retry success rate | T023, T029 | Manual test F2 |
| SC-006: Survive unexpected termination | T034, T035 | Crash test |
