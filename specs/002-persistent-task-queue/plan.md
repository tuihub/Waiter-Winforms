````markdown
# Implementation Plan: Persistent Task Queue

**Branch**: `002-persistent-task-queue` | **Date**: 2026-01-31 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from `/specs/002-persistent-task-queue/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/commands/plan.md` for the execution workflow.

## Summary

Implement persistent task queue functionality to ensure background tasks survive application restarts. Failed or interrupted tasks (downloads, save syncs) are persisted to SQLite database with all parameters needed for retry. Users can view failed tasks after app restart and retry them explicitly. Tasks are marked as completed/failed upon finish and can be cleared by the user. The existing in-memory `BackgroundTaskService` will be extended with a persistence layer through a new `PersistentTaskService`.

## Technical Context

**Language/Version**: C# / .NET 8.0 (Windows Forms)  
**Primary Dependencies**: TuiHub.Protos (gRPC), Entity Framework Core 9.0 (SQLite), Microsoft.Extensions.DependencyInjection, System.Text.Json (for parameter serialization)  
**Storage**: SQLite via Entity Framework Core - extend existing `WaiterDbContext` with new `PersistentTask` entity  
**Testing**: Manual testing (per constitution) for UI changes; unit tests encouraged for Services  
**Target Platform**: Windows desktop (WinForms requirement)  
**Project Type**: Desktop application (single project with Services/Forms/Data separation)  
**Performance Goals**: App startup delay <500ms for loading up to 50 tasks, task state persistence <1 second, 3 clicks max for retry action  
**Constraints**: UI thread responsiveness (async/await required for all database I/O), graceful handling of corrupted database, prevent duplicate tasks  
**Scale/Scope**: Maximum ~100 concurrent tasks, extends existing BackgroundTaskService and BackgroundTasksForm, adds 1 new Service class and 1 new database entity

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Principle I: Service-Oriented Architecture ✅ PASS
- **Requirement**: All business logic in dedicated Service classes under `Services/`
- **Application**: Create new `PersistentTaskService` for persistence operations; integrate with existing `BackgroundTaskService` via events; use DI for all dependencies
- **Compliance**: Design separates persistence logic (Service) from UI controls (Forms), extends existing service pattern

### Principle II: Data Persistence Consistency ✅ PASS
- **Requirement**: All persistent data through EF Core with WaiterDbContext
- **Application**: Extend `WaiterDbContext` with new `PersistentTask` entity; use DatabaseService for queries; follow existing model patterns (`Data/Models/`)
- **Compliance**: No direct file I/O for task persistence; uses established EF Core patterns

### Principle III: gRPC Client Discipline ✅ PASS
- **Requirement**: All server communication via gRPC clients from TuiHub.Protos through LibrarianClientService
- **Application**: Feature is purely local persistence; no new gRPC calls required. Existing task types (downloads, syncs) already use proper gRPC patterns
- **Compliance**: N/A for this feature - local-only functionality

### Principle IV: UI Responsiveness ✅ PASS
- **Requirement**: All long-running operations async, use BackgroundTaskService
- **Application**: Database reads/writes are async; task loading on startup is background operation; UI updates via proper Invoke pattern
- **Compliance**: All database I/O uses async/await; progress feedback for bulk operations

### Principle V: Simplicity and YAGNI ✅ PASS
- **Requirement**: Simplest viable solution; complexity requires justification
- **Application**: Extend existing BackgroundTaskService rather than replacing it; single new entity; JSON serialization for task parameters (simplest approach)
- **Compliance**: No auto-retry mechanism (per spec FR-011); manual retry only; minimal new abstractions

### Principle VI: Visual Studio Designer Compatibility ✅ PASS
- **Requirement**: Forms must remain Designer-compatible
- **Application**: Extend existing `BackgroundTasksForm` with retry functionality; use standard event patterns; no constructor parameter changes required (use existing DI patterns)
- **Compliance**: No changes to Form constructors or Designer files required

**GATE STATUS: ✅ ALL CHECKS PASS** - No violations requiring justification. Design aligns with all six core principles.

## Project Structure

### Documentation (this feature)

```text
specs/002-persistent-task-queue/
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output (/speckit.plan command)
├── data-model.md        # Phase 1 output (/speckit.plan command)
├── quickstart.md        # Phase 1 output (/speckit.plan command)
├── contracts/           # Phase 1 output (/speckit.plan command)
└── tasks.md             # Phase 2 output (/speckit.tasks command - NOT created by /speckit.plan)
```

### Source Code (repository root)

```text
Waiter/
├── Services/
│   ├── BackgroundTaskService.cs      # EXISTING: Add persistence integration via events
│   ├── PersistentTaskService.cs      # NEW: Database persistence for tasks
│   └── [Other Services]              # EXISTING: No changes
├── Data/
│   ├── WaiterDbContext.cs            # EXISTING: Add DbSet<PersistentTask>
│   ├── DatabaseService.cs            # EXISTING: May add convenience methods
│   └── Models/
│       ├── PersistentTask.cs         # NEW: Entity for persistent task storage
│       └── [Other Models]            # EXISTING: No changes
├── Forms/
│   ├── BackgroundTasksForm.cs        # EXISTING: Add retry button, load persisted tasks
│   ├── BackgroundTasksForm.Designer.cs # EXISTING: Add retry button control
│   └── [Other Forms]                 # EXISTING: No changes
└── Migrations/                        # NEW: EF Core migration (if needed)
```

**Structure Decision**: Single-project WinForms application following existing Waiter/ structure. Feature adds one new Service (`PersistentTaskService`), one new Model (`PersistentTask`), and extends existing `BackgroundTasksForm`. Follows constitution's Forms/Services/Data organization.

## Complexity Tracking

**No violations**: All constitution principles satisfied. No additional complexity justification required.

---

## Phase 0 Deliverables ✅

Research completed in [research.md](research.md):
- Task serialization approach → System.Text.Json with discriminated union pattern
- EF Core migration strategy → EnsureCreated (per assumptions, no migration management yet)
- Duplicate task detection → Hash-based key from task type + parameters
- Interrupted task detection → Mark "Running" tasks as "Interrupted" on startup
- Startup performance → Lazy loading with background thread
- Database corruption handling → Try/catch with fallback to in-memory only

## Phase 1 Deliverables ✅

Design and contracts complete:
- [data-model.md](data-model.md) - PersistentTask entity definition, EF configuration
- [contracts/service-contracts.md](contracts/service-contracts.md) - IPersistentTaskService interface
- [quickstart.md](quickstart.md) - Step-by-step implementation guide

## Constitution Re-Check ✅

**Post-Design Validation**:
- ✅ Service-Oriented: All persistence logic in PersistentTaskService
- ✅ Data Persistence: Single new entity via EF Core
- ✅ gRPC Discipline: N/A (local-only feature)
- ✅ UI Responsiveness: Full async/await pattern, background startup loading
- ✅ Simplicity: Minimal implementation, extends existing patterns
- ✅ Designer Compatibility: No Form constructor changes

**No violations introduced during design phase.**

## Next Steps

**Phase 2**: Run /speckit.tasks command to generate tasks.md with:
- Detailed implementation checklist (broken down by file)
- Manual test scenarios per user story
- PR submission and review criteria

**Implementation**: Follow [quickstart.md](quickstart.md) for step-by-step development workflow.

---

**Planning Complete** | Branch: 002-persistent-task-queue | Date: 2026-01-31

````
