# Waiter-Winforms Development Guidelines

Auto-generated from all feature plans. Last updated: 2026-01-31

## Active Technologies
- C# / .NET 8.0 (Windows Forms) + TuiHub.Protos (gRPC), Entity Framework Core 9.0 (SQLite), Microsoft.Extensions.DependencyInjection, System.Text.Json (for parameter serialization) (002-persistent-task-queue)
- SQLite via Entity Framework Core - extend existing `WaiterDbContext` with new `PersistentTask` entity (002-persistent-task-queue)

- C# / .NET 8.0 (Windows Forms) + TuiHub.Protos (gRPC), Entity Framework Core (SQLite), TuiHub.ProcessTimeMonitorLibrary (NEEDS CLARIFICATION - availability and API surface), Microsoft.Extensions.DependencyInjection (001-app-launch-runtime)

## Project Structure

```text
src/
tests/
```

## Commands

# Add commands for C# / .NET 8.0 (Windows Forms)

## Code Style

C# / .NET 8.0 (Windows Forms): Follow standard conventions

## Recent Changes
- 002-persistent-task-queue: Added C# / .NET 8.0 (Windows Forms) + TuiHub.Protos (gRPC), Entity Framework Core 9.0 (SQLite), Microsoft.Extensions.DependencyInjection, System.Text.Json (for parameter serialization)

- 001-app-launch-runtime: Added C# / .NET 8.0 (Windows Forms) + TuiHub.Protos (gRPC), Entity Framework Core (SQLite), TuiHub.ProcessTimeMonitorLibrary (NEEDS CLARIFICATION - availability and API surface), Microsoft.Extensions.DependencyInjection

<!-- MANUAL ADDITIONS START -->
<!-- MANUAL ADDITIONS END -->
