# Data Model: App Launch with Runtime Tracking

**Phase**: 1 (Design & Contracts)  
**Date**: 2026-01-31  
**Status**: Complete

This document defines all data entities, their relationships, validation rules, and state transitions for the app launch and runtime tracking feature.

---

## Entity Diagram

```
┌─────────────────────────────────────┐
│         AppPackage (existing)       │
│ ─────────────────────────────────── │
│ + Id: long                          │
│ + Name: string                      │
│ + [other existing fields]           │
└─────────────────────────────────────┘
         │
         │ 1
         │
         │ *
┌─────────────────────────────────────┐       ┌─────────────────────────────────┐
│  AppPackageLaunchSettings (new)     │       │    RuntimeSession (new)         │
│ ─────────────────────────────────── │       │ ─────────────────────────────── │
│ + AppPackageId: long (FK)           │───────│ + Id: long (PK)                 │
│ + ExecutablePath: string            │   *   │ + AppPackageId: long (FK)       │
│ + WorkingDirectory: string          │   1   │ + DeviceId: long                │
│ + MonitoringMode: enum              │       │ + StartTime: DateTime           │
│ + ProcessName: string?              │       │ + EndTime: DateTime?            │
│ + UseShellExecute: bool             │       │ + ExitCode: int?                │
│ + LaunchTimeout: int (seconds)      │       │ + Status: enum                  │
└─────────────────────────────────────┘       │ + UploadAttempted: bool         │
                                               │ + UploadedAt: DateTime?         │
                                               └─────────────────────────────────┘
                                                         │
                                                         │ 1
                                                         │
                                                         │ 0..1
                                               ┌─────────────────────────────────┐
                                               │    CachedUpload (new)           │
                                               │ ─────────────────────────────── │
                                               │ + Id: long (PK)                 │
                                               │ + RuntimeSessionId: long (FK)?  │
                                               │ + UploadType: enum              │
                                               │ + FilePath: string              │
                                               │ + Metadata: string (JSON)       │
                                               │ + CreatedAt: DateTime           │
                                               │ + RetryCount: int               │
                                               │ + LastError: string?            │
                                               └─────────────────────────────────┘
```

---

## Entities

### 1. AppPackageLaunchSettings (New Entity)

Stores local configuration for launching an app package.

**Fields**:

| Field | Type | Required | Default | Validation |
|-------|------|----------|---------|------------|
| `Id` | `long` | Yes | Auto-increment | PK |
| `AppPackageId` | `long` | Yes | - | FK to AppPackage; UNIQUE index |
| `ExecutablePath` | `string` | Yes | - | NOT NULL; must be valid file path (existence checked at runtime per FR-016) |
| `WorkingDirectory` | `string` | Yes | - | NOT NULL; must be valid directory path (existence checked at runtime per FR-016) |
| `MonitoringMode` | `MonitoringMode` (enum) | Yes | `DirectProcess` | `DirectProcess` or `ProcessName` |
| `ProcessName` | `string?` | No | `null` | Required if MonitoringMode = `ProcessName`; regex: `^[a-zA-Z0-9_\-\.]+$` |
| `UseShellExecute` | `bool` | Yes | `false` | Per FR-014 |
| `LaunchTimeout` | `int` | Yes | `30` | Seconds; range: [5, 300]; per FR-011 and FR-021 |
| `SaveDataPath` | `string?` | No | `null` | Optional; directory for save data backup |

**Relationships**:
- One-to-one with `AppPackage` (FK: `AppPackageId`)
- One-to-many with `RuntimeSession`

**Validation Rules**:
- `ExecutablePath` and `WorkingDirectory` MUST exist on disk before launch (FR-016)
- `ProcessName` is REQUIRED when `MonitoringMode = ProcessName`
- `LaunchTimeout` must be between 5 and 300 seconds (reasonable bounds)

**State Transitions**: N/A (configuration data, no lifecycle states)

---

### 2. RuntimeSession (New Entity)

Records a single execution session of an application.

**Fields**:

| Field | Type | Required | Default | Validation |
|-------|------|----------|---------|------------|
| `Id` | `long` | Yes | Auto-increment | PK |
| `AppPackageId` | `long` | Yes | - | FK to AppPackage |
| `DeviceId` | `long` | Yes | - | Current device identifier (TuiHub device ID) |
| `StartTime` | `DateTime` | Yes | - | UTC timestamp; NOT NULL |
| `EndTime` | `DateTime?` | No | `null` | UTC timestamp; must be >= StartTime |
| `ExitCode` | `int?` | No | `null` | Process exit code; 0 = normal, non-zero = error |
| `Status` | `SessionStatus` (enum) | Yes | `Tracking` | See state diagram below |
| `UploadAttempted` | `bool` | Yes | `false` | Set to true after attempting upload (success or failure) |
| `UploadedAt` | `DateTime?` | No | `null` | UTC timestamp of successful upload; per FR-006 |
| `Duration` | `TimeSpan` | No | Computed | Calculated: `EndTime - StartTime` (if EndTime not null) |

**Relationships**:
- Many-to-one with `AppPackage` (FK: `AppPackageId`)
- One-to-one with `CachedUpload` (if upload fails)

**Computed Properties**:
```csharp
public TimeSpan? Duration => EndTime.HasValue ? EndTime.Value - StartTime : null;
public bool IsRunning => Status == SessionStatus.Tracking && !EndTime.HasValue;
public bool IsAbnormalExit => ExitCode.HasValue && ExitCode.Value != 0;
```

**Validation Rules**:
- `EndTime` must be >= `StartTime`
- `Status` transitions must follow state diagram
- `UploadedAt` can only be set if `UploadAttempted = true` and `Status = Completed`

**State Transitions**:

```
┌─────────┐
│ Initial │
└────┬────┘
     │
     ▼
┌──────────┐  App exits normally (ExitCode=0)   ┌────────────┐
│ Tracking │───────────────────────────────────▶│ Processing │
└──────────┘                                     └──────┬─────┘
     │                                                  │
     │ App exits abnormally (ExitCode≠0)               │ Upload success
     ▼                                                  ▼
┌──────────┐  User confirms upload               ┌───────────┐
│ Abnormal │──────────────────────────────────▶│ Processing │
└──────────┘                                     └──────┬────┘
     │                                                  │
     │ User declines upload                            │
     ▼                                                  │
┌──────────┐                                           │
│  Skipped │◀──────────────────────────────────────────┤ Upload failed
└──────────┘                                           ▼
                                                  ┌──────────┐
                                                  │  Failed  │
                                                  └─────┬────┘
                                                        │ Manual retry
                                                        ▼
                                                  ┌───────────┐
                                                  │ Completed │
                                                  └───────────┘
```

**SessionStatus Enum**:
```csharp
public enum SessionStatus
{
    Tracking = 0,      // Process is running
    Processing = 1,    // App exited, uploading data
    Completed = 2,     // Successfully uploaded
    Failed = 3,        // Upload failed (cached locally)
    Skipped = 4,       // User declined upload after abnormal exit
    Abnormal = 5       // Awaiting user confirmation after abnormal exit
}
```

---

### 3. CachedUpload (New Entity)

Stores failed upload data locally for manual retry.

**Fields**:

| Field | Type | Required | Default | Validation |
|-------|------|----------|---------|------------|
| `Id` | `long` | Yes | Auto-increment | PK |
| `RuntimeSessionId` | `long?` | No | `null` | FK to RuntimeSession; nullable (could be other upload types in future) |
| `UploadType` | `UploadType` (enum) | Yes | - | `RuntimeData` or `SaveFile` |
| `FilePath` | `string` | Yes | - | Absolute path to cached file in cache directory |
| `Metadata` | `string` | Yes | - | JSON-serialized metadata (upload request details) |
| `CreatedAt` | `DateTime` | Yes | `DateTime.UtcNow` | UTC timestamp |
| `RetryCount` | `int` | Yes | `0` | Number of retry attempts; range: [0, 10] |
| `LastError` | `string?` | No | `null` | Last error message from failed upload attempt |
| `ExpiresAt` | `DateTime` | Yes | `CreatedAt + 30 days` | UTC timestamp; auto-cleanup threshold |

**Relationships**:
- Many-to-one with `RuntimeSession` (FK: `RuntimeSessionId`, nullable)

**Validation Rules**:
- `FilePath` must exist at time of creation
- `RetryCount` must be >= 0
- `Metadata` must be valid JSON
- `ExpiresAt` must be > `CreatedAt`

**UploadType Enum**:
```csharp
public enum UploadType
{
    RuntimeData = 0,  // Runtime statistics (BatchCreateAppRunTime API)
    SaveFile = 1      // Save data archive (UploadAppSaveFile API)
}
```

**Metadata Structure**:

For `RuntimeData`:
```json
{
  "appRunTimes": [
    {
      "appId": 123,
      "deviceId": 456,
      "startTime": "2026-01-31T10:00:00Z",
      "endTime": "2026-01-31T12:30:00Z"
    }
  ]
}
```

For `SaveFile`:
```json
{
  "appId": 123,
  "fileName": "save_20260131_120000.zip",
  "fileSize": 1048576,
  "sha256": "abcdef1234567890...",
  "uploadToken": "presigned-url-or-token"
}
```

**State Transitions**: N/A (queued items only; deleted after successful upload or expiration)

---

### 4. MonitoringMode (Enum)

Defines how the system monitors process lifecycle.

```csharp
public enum MonitoringMode
{
    DirectProcess = 0,  // Monitor the launched process directly (default, simpler)
    ProcessName = 1     // Monitor by process name (for multi-process apps with launchers)
}
```

**Usage**:
- `DirectProcess`: Track the process returned by `Process.Start()` (FR-004)
- `ProcessName`: Poll for process matching `AppPackageLaunchSettings.ProcessName` (FR-004, User Story 3)

---

## Database Migrations

### Migration: `AddAppLaunchTables`

**Up Script**:
```sql
-- AppPackageLaunchSettings table
CREATE TABLE AppPackageLaunchSettings (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    AppPackageId INTEGER NOT NULL UNIQUE,
    ExecutablePath TEXT NOT NULL,
    WorkingDirectory TEXT NOT NULL,
    MonitoringMode INTEGER NOT NULL DEFAULT 0,
    ProcessName TEXT,
    UseShellExecute INTEGER NOT NULL DEFAULT 0,
    LaunchTimeout INTEGER NOT NULL DEFAULT 30,
    SaveDataPath TEXT,
    FOREIGN KEY (AppPackageId) REFERENCES AppPackages(Id) ON DELETE CASCADE
);

-- RuntimeSession table
CREATE TABLE RuntimeSessions (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    AppPackageId INTEGER NOT NULL,
    DeviceId INTEGER NOT NULL,
    StartTime TEXT NOT NULL,
    EndTime TEXT,
    ExitCode INTEGER,
    Status INTEGER NOT NULL DEFAULT 0,
    UploadAttempted INTEGER NOT NULL DEFAULT 0,
    UploadedAt TEXT,
    FOREIGN KEY (AppPackageId) REFERENCES AppPackages(Id) ON DELETE CASCADE
);

-- CachedUpload table
CREATE TABLE CachedUploads (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    RuntimeSessionId INTEGER,
    UploadType INTEGER NOT NULL,
    FilePath TEXT NOT NULL,
    Metadata TEXT NOT NULL,
    CreatedAt TEXT NOT NULL,
    RetryCount INTEGER NOT NULL DEFAULT 0,
    LastError TEXT,
    ExpiresAt TEXT NOT NULL,
    FOREIGN KEY (RuntimeSessionId) REFERENCES RuntimeSessions(Id) ON DELETE CASCADE
);

-- Indexes for common queries
CREATE INDEX IX_RuntimeSessions_AppPackageId ON RuntimeSessions(AppPackageId);
CREATE INDEX IX_RuntimeSessions_DeviceId ON RuntimeSessions(DeviceId);
CREATE INDEX IX_RuntimeSessions_Status ON RuntimeSessions(Status);
CREATE INDEX IX_CachedUploads_RuntimeSessionId ON CachedUploads(RuntimeSessionId);
CREATE INDEX IX_CachedUploads_ExpiresAt ON CachedUploads(ExpiresAt);
```

**Down Script**:
```sql
DROP INDEX IF EXISTS IX_CachedUploads_ExpiresAt;
DROP INDEX IF EXISTS IX_CachedUploads_RuntimeSessionId;
DROP INDEX IF EXISTS IX_RuntimeSessions_Status;
DROP INDEX IF EXISTS IX_RuntimeSessions_DeviceId;
DROP INDEX IF EXISTS IX_RuntimeSessions_AppPackageId;
DROP TABLE IF EXISTS CachedUploads;
DROP TABLE IF EXISTS RuntimeSessions;
DROP TABLE IF EXISTS AppPackageLaunchSettings;
```

---

## Entity Framework Configuration

### AppPackageLaunchSettings Configuration

```csharp
public class AppPackageLaunchSettingsConfiguration : IEntityTypeConfiguration<AppPackageLaunchSettings>
{
    public void Configure(EntityTypeBuilder<AppPackageLaunchSettings> builder)
    {
        builder.ToTable("AppPackageLaunchSettings");
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.ExecutablePath)
            .IsRequired()
            .HasMaxLength(500);
            
        builder.Property(x => x.WorkingDirectory)
            .IsRequired()
            .HasMaxLength(500);
            
        builder.Property(x => x.ProcessName)
            .HasMaxLength(100);
            
        builder.Property(x => x.SaveDataPath)
            .HasMaxLength(500);
            
        builder.HasIndex(x => x.AppPackageId)
            .IsUnique();
            
        builder.HasOne<AppPackage>()
            .WithOne()
            .HasForeignKey<AppPackageLaunchSettings>(x => x.AppPackageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
```

### RuntimeSession Configuration

```csharp
public class RuntimeSessionConfiguration : IEntityTypeConfiguration<RuntimeSession>
{
    public void Configure(EntityTypeBuilder<RuntimeSession> builder)
    {
        builder.ToTable("RuntimeSessions");
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.StartTime)
            .IsRequired();
            
        builder.HasIndex(x => x.AppPackageId);
        builder.HasIndex(x => x.DeviceId);
        builder.HasIndex(x => x.Status);
        
        builder.Ignore(x => x.Duration);
        builder.Ignore(x => x.IsRunning);
        builder.Ignore(x => x.IsAbnormalExit);
        
        builder.HasOne<AppPackage>()
            .WithMany()
            .HasForeignKey(x => x.AppPackageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
```

### CachedUpload Configuration

```csharp
public class CachedUploadConfiguration : IEntityTypeConfiguration<CachedUpload>
{
    public void Configure(EntityTypeBuilder<CachedUpload> builder)
    {
        builder.ToTable("CachedUploads");
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.FilePath)
            .IsRequired()
            .HasMaxLength(500);
            
        builder.Property(x => x.Metadata)
            .IsRequired();
            
        builder.Property(x => x.LastError)
            .HasMaxLength(1000);
            
        builder.HasIndex(x => x.RuntimeSessionId);
        builder.HasIndex(x => x.ExpiresAt);
        
        builder.HasOne<RuntimeSession>()
            .WithOne()
            .HasForeignKey<CachedUpload>(x => x.RuntimeSessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
```

---

## Validation Summary

| Entity | Critical Validations |
|--------|---------------------|
| `AppPackageLaunchSettings` | ExecutablePath and WorkingDirectory must exist on disk (runtime check); ProcessName required if MonitoringMode=ProcessName |
| `RuntimeSession` | EndTime >= StartTime; Status transitions must be valid; UploadedAt requires UploadAttempted=true |
| `CachedUpload` | FilePath must exist; Metadata must be valid JSON; RetryCount [0,10]; ExpiresAt > CreatedAt |

---

## Notes

- **Device ID**: Requires implementation of device identification strategy (may need new DeviceInfo entity or configuration)
- **Timezone**: All timestamps stored as UTC for consistency
- **Cascading Deletes**: Enabled for all FK relationships (delete AppPackage → delete settings, sessions, cached uploads)
- **Performance**: Indexes added for common query patterns (filter by app, device, status, expiration)
- **Future Extensions**: CachedUpload.RuntimeSessionId is nullable to support other upload types (e.g., manual backups, screenshot uploads)
