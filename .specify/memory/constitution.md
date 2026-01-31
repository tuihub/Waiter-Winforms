<!--
================================================================================
SYNC IMPACT REPORT
================================================================================
Version change: 0.1.0 → 0.1.1 (Clarify communication contract requirements)
Modified principles:
  - III. gRPC Client Discipline (clarified scope and presigned URL exception)
Added sections:
  - None
Removed sections: N/A
Templates requiring updates:
  - plan-template.md: ✅ No changes needed (generic template compatible)
  - spec-template.md: ✅ No changes needed (generic template compatible)
  - tasks-template.md: ✅ No changes needed (generic template compatible)
Follow-up TODOs: None
================================================================================
-->

# Waiter-Winforms Constitution

A WinForms desktop client for TuiHub ecosystem, providing a Steam-like interface for managing Apps.

## Core Principles

### I. Service-Oriented Architecture

All business logic MUST be encapsulated in dedicated Service classes under the `Services/` directory.

- Forms/UI code MUST NOT contain business logic; Forms are responsible only for presentation and user interaction
- Services MUST be registered with the DI container and injected via constructor injection
- Each service MUST have a single, clear responsibility (e.g., `TokenService` handles JWT tokens only)
- Services MUST NOT directly reference UI components (Forms, Controls)

**Rationale**: Enables testability, reusability, and clear separation of concerns in the codebase.

### II. Data Persistence Consistency

All persistent data MUST be managed through Entity Framework Core with the `WaiterDbContext`.

- Database models MUST reside in `Data/Models/` directory
- Database operations MUST use `DatabaseService` or the DbContext directly via DI
- All database changes MUST use EF Core migrations for schema updates
- Cached/temporary data MUST be clearly distinguished from persistent data in the model layer

**Rationale**: Ensures data integrity, simplifies database management, and maintains consistent data access patterns.

### III. gRPC Client Discipline

All communication with the TuiHub server MUST use gRPC clients and request/response types from
TuiHub.Protos, routed through `LibrarianClientService`.

- API calls MUST use the `ClientTokenInterceptor` for automatic authentication handling
- Token refresh logic MUST be centralized in `TokenService`
- API errors MUST be caught, logged, and presented to users with meaningful messages
- All gRPC client operations MUST be async/await compliant
- The client MUST NOT introduce ad-hoc REST/JSON endpoints to replace proto-defined APIs
- If a proto-defined API returns a presigned upload/download URL, the actual data transfer MAY use HTTP,
  but MUST be encapsulated in a dedicated Service (not in Forms) and treated as an implementation detail

**Rationale**: Centralizes API communication, ensures consistent authentication, and prevents scattered network code.

### IV. UI Responsiveness

All long-running operations MUST be executed asynchronously to keep the UI responsive.

- Network calls, database operations, and file I/O MUST use async/await patterns
- Background tasks MUST be managed through `BackgroundTaskService`
- Progress feedback MUST be provided for operations exceeding 500ms
- UI updates from background threads MUST use proper cross-thread invocation (`Invoke`/`BeginInvoke`)

**Rationale**: WinForms applications freeze when blocking the UI thread; async patterns ensure a smooth user experience.

### V. Simplicity and YAGNI

Features MUST be implemented with the simplest viable solution; complexity requires explicit justification.

- Start with minimal implementation that satisfies requirements
- Do not add features, services, or abstractions "just in case"
- Stub implementations (marked with TODO) are acceptable for planned but unimplemented features
- Refactor only when current structure impedes new requirements

**Rationale**: The project is a client application; over-engineering increases maintenance burden without proportional benefit.

## Technical Constraints

The following technology stack and constraints MUST be maintained:

- **Framework**: .NET 8.0 with Windows Forms (WinForms)
- **API Communication**: gRPC via `TuiHub.Protos` package
- **Database**: SQLite via Entity Framework Core
- **Dependency Injection**: Microsoft.Extensions.DependencyInjection
- **Target Platform**: Windows only (WinForms requirement)
- **License**: MIT License - all contributions MUST be compatible

Code organization:
- `Forms/` - All WinForms UI classes (*.cs, *.Designer.cs, *.resx)
- `Services/` - Business logic and external service integrations
- `Data/` - Database context, services, and entity models
- `Interceptors/` - gRPC interceptors for cross-cutting concerns
- `Helpers/` - Utility classes with no external dependencies

## Development Workflow

Development follows these practices:

- **Branching**: Feature branches from main; descriptive names (e.g., `feat/add-notification-support`)
- **Commits**: Conventional commits format (`feat:`, `fix:`, `chore:`, `docs:`)
- **Code Review**: All PRs require review; verify constitution compliance before merge
- **Testing**: Manual testing required for UI changes; unit tests encouraged for Services
- **Documentation**: README updates required for new features; inline XML docs for public APIs

Quality gates before merge:
1. Application builds without errors (`dotnet build`)
2. No obvious UI regressions in affected Forms
3. Constitution principles verified

## Governance

This constitution supersedes all other development practices for the Waiter-Winforms project.

- **Amendments**: Changes to principles require documentation of rationale and impact assessment
- **Versioning**: Constitution uses semantic versioning (MAJOR.MINOR.PATCH)
  - MAJOR: Principle removal or incompatible redefinition
  - MINOR: New principle or significant expansion
  - PATCH: Clarifications and wording improvements
- **Compliance**: All code changes SHOULD be checked against principles during review
- **Exceptions**: Temporary exceptions MUST be documented with TODO and tracking issue

**Version**: 0.1.1 | **Ratified**: 2026-01-31 | **Last Amended**: 2026-01-31
