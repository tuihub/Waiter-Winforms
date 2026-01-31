# Implementation Plan: App Launch with Runtime Tracking

**Branch**: `001-app-launch-runtime` | **Date**: 2026-01-31 | **Spec**: [spec.md](spec.md)  
**Input**: Feature specification from `/specs/001-app-launch-runtime/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/commands/plan.md` for the execution workflow.

## Summary

Implement app launch functionality with runtime tracking integration. Users can launch installed applications directly from the Waiter-Winforms interface, with the system automatically tracking runtime statistics and uploading session data to the TuiHub server. This feature integrates ProcessTimeMonitor library for process lifecycle management and handles various edge cases including network failures, abnormal exits, and multi-process applications.

## Technical Context

**Language/Version**: C# / .NET 8.0 (Windows Forms)  
**Primary Dependencies**: TuiHub.Protos (gRPC), Entity Framework Core (SQLite), TuiHub.ProcessTimeMonitorLibrary (NEEDS CLARIFICATION - availability and API surface), Microsoft.Extensions.DependencyInjection  
**Storage**: SQLite via Entity Framework Core for app package settings and local cache of failed uploads  
**Testing**: Manual testing (per constitution) for UI changes; unit tests encouraged for Services (NEEDS CLARIFICATION - current test infrastructure)  
**Target Platform**: Windows desktop (WinForms requirement)  
**Project Type**: Desktop application (single project with Services/Forms/Data separation)  
**Performance Goals**: <5 seconds overhead for launch/tracking cycle (excluding actual app runtime), <500ms UI feedback for user actions, runtime accuracy within 1 second for sessions >30 seconds  
**Constraints**: UI thread responsiveness (async/await required for all I/O), Windows-only APIs (System.Diagnostics.Process), graceful network failure handling with local caching  
**Scale/Scope**: Feature adds 1 major Service class, updates to existing Forms (AppDetailForm likely), database model extensions for tracking sessions and cached uploads

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Principle I: Service-Oriented Architecture ✅ PASS
- **Requirement**: All business logic in dedicated Service classes under `Services/`
- **Application**: Create new `AppLaunchService` for launch orchestration, integrate with existing `LibrarianClientService` for API calls, use DI for all dependencies
- **Compliance**: Design will separate launch logic (Service) from UI controls (Forms)

### Principle II: Data Persistence Consistency ✅ PASS
- **Requirement**: All persistent data through EF Core with WaiterDbContext
- **Application**: Extend database models for RuntimeSession and CachedUpload entities, use DatabaseService for queries, create EF migration for schema changes
- **Compliance**: No direct file I/O for persistent data; cache directory only for temporary upload failures

### Principle III: gRPC Client Discipline ✅ PASS
- **Requirement**: All server communication via gRPC clients from TuiHub.Protos through LibrarianClientService
- **Application**: Use LibrarianSephirahService for runtime reporting and save file upload, leverage existing ClientTokenInterceptor for auth, handle presigned URLs for large file transfers (if applicable)
- **Compliance**: No ad-hoc REST endpoints; all API operations async/await

### Principle IV: UI Responsiveness ✅ PASS
- **Requirement**: All long-running operations async, use BackgroundTaskService
- **Application**: Launch and tracking operations are async, progress dialogs provide feedback, cross-thread UI updates via Invoke pattern
- **Compliance**: Network calls, process monitoring, and file I/O all async; progress feedback <500ms requirement aligns with constitution

### Principle V: Simplicity and YAGNI ✅ PASS
- **Requirement**: Simplest viable solution; complexity requires justification
- **Application**: Start with direct process tracking (simpler), add process listen mode only if needed, reuse existing progress dialog components
- **Compliance**: No premature abstractions; stub placeholders acceptable for future enhancements

**GATE STATUS: ✅ ALL CHECKS PASS** - No violations requiring justification. Design aligns with all five core principles.

## Project Structure

### Documentation (this feature)

```text
specs/[###-feature]/
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
│   ├── AppLaunchService.cs           # NEW: Launch orchestration and runtime tracking
│   ├── ProcessMonitorService.cs      # NEW: Process lifecycle management
│   ├── LibrarianClientService.cs     # EXISTING: Extend for runtime/save upload APIs
│   ├── BackgroundTaskService.cs      # EXISTING: Manage async operations
│   └── TokenService.cs               # EXISTING: Auth handling
├── Data/
│   ├── WaiterDbContext.cs            # EXISTING: Extend for new entities
│   ├── DatabaseService.cs            # EXISTING: May need query extensions
│   └── Models/
│       ├── AppPackage.cs             # EXISTING: May need setting properties
│       ├── RuntimeSession.cs         # NEW: Track app runtime sessions
│       └── CachedUpload.cs           # NEW: Store failed upload data
├── Forms/
│   ├── AppDetailForm.cs              # EXISTING: Add launch button and handlers
│   ├── ProgressBarWindow.cs          # EXISTING: Reuse for launch progress
│   └── [Other Forms]                 # EXISTING: No changes expected
├── Helpers/
│   └── EnsureLoginHelper.cs          # EXISTING: Validate auth before operations
└── Migrations/                        # NEW: EF Core migration for schema

tests/ (optional, encouraged per constitution)
└── Services/
    └── AppLaunchService.Tests.cs     # NEW: Unit tests for launch service
```

**Structure Decision**: Single-project WinForms application following existing Waiter/ structure. New feature primarily adds Service classes (AppLaunchService, ProcessMonitorService) and extends Data models. UI changes minimal (button + handlers in existing Forms). Follows constitution's Forms/Services/Data/Helpers organization.

## Complexity Tracking

**No violations**: All constitution principles satisfied. No additional complexity justification required.

---

## Phase 0 Deliverables ?

All NEEDS CLARIFICATION items resolved in [research.md](research.md):
- ProcessTimeMonitor library  Custom implementation using System.Diagnostics.Process
- Test infrastructure  Manual testing with optional unit tests
- gRPC API methods  BatchCreateAppRunTime and UploadAppSaveFile identified
- SavedataManager service  Inline compression using System.IO.Compression
- ProgressBarWindow  Create new ProgressDialog Form
- Async patterns  Task-based async with proper WinForms marshalling
- File transfer  Streaming upload with chunked reading and SHA256 hashing
- Cache directory  Use GetRealCacheDirPath() with database tracking

## Phase 1 Deliverables ?

Design and contracts complete:
- [data-model.md](data-model.md) - Database entities, migrations, EF configurations
- [contracts/api-contracts.md](contracts/api-contracts.md) - gRPC endpoints and error handling
- [contracts/service-contracts.md](contracts/service-contracts.md) - Service interfaces and DI registration
- [quickstart.md](quickstart.md) - Step-by-step implementation guide (8-11 hours)

## Constitution Re-Check ?

**Post-Design Validation**:
- ? Service-Oriented: All business logic in Services (AppLaunchService, ProcessMonitorService, SaveDataService)
- ? Data Persistence: 3 new entities via EF Core, proper migrations defined
- ? gRPC Discipline: All server communication through LibrarianClientService, presigned URLs encapsulated
- ? UI Responsiveness: Full async/await pattern, progress feedback, proper cross-thread marshalling
- ? Simplicity: Minimal viable implementation, no premature abstractions

**No violations introduced during design phase.**

## Next Steps

**Phase 2**: Run /speckit.tasks command to generate tasks.md with:
- Detailed implementation checklist (broken down by file)
- Manual test scenarios per user story
- PR submission and review criteria

**Implementation**: Follow [quickstart.md](quickstart.md) for step-by-step development workflow.

---

**Planning Complete** | Branch:  01-app-launch-runtime | Date: 2026-01-31
