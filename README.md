# Waiter-Winforms

A WinForms client application for TuiHub, designed with a classic Steam-like interface for managing Apps in the TuiHub ecosystem.

## Features

- **Login and Authentication**
  - JWT-based authentication with username/password login
  - Automatic token refresh using RefreshToken
  - Support for DownloadToken (reserved for future use)
  - Token interceptor pattern for seamless API authentication
  - Persistent token storage with SQLite database

- **App Management**
  - Browse and manage your app library
  - Search apps in the store
  - Acquire apps from the store
  - View app details
  - Delete apps

- **App Category Management**
  - Create, update, and delete app categories
  - Organize apps into categories

- **Background Task Management**
  - Download app tasks (stub implementation)
  - Sync save file tasks (stub implementation)
  - Task progress tracking

- **Settings**
  - Configurable server URL
  - Persistent configuration storage with SQLite database

- **Data Persistence**
  - SQLite database backend using Entity Framework Core
  - Stores user credentials, settings, cached data, and task history

## Requirements

- .NET 8.0 SDK
- Windows (WinForms application)

## Building

```bash
# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run the application
dotnet run --project Waiter
```

## Project Structure

```
Waiter/
├── Data/                     # Entity Framework Core
│   ├── Models/              # Database entity models
│   │   ├── UserCredential.cs
│   │   ├── AppSetting.cs
│   │   ├── CachedApp.cs
│   │   ├── CachedAppCategory.cs
│   │   └── TaskHistory.cs
│   ├── WaiterDbContext.cs   # EF Core DbContext
│   └── DatabaseService.cs   # Database operations service
├── Forms/                    # WinForms UI
│   ├── MainForm.cs          # Main application window (Steam-like layout)
│   ├── LoginForm.cs         # Login dialog
│   ├── SettingsForm.cs      # Settings dialog
│   ├── AppCategoryForm.cs   # App category management
│   └── BackgroundTasksForm.cs # Background task viewer
├── Services/                 # Core services
│   ├── LibrarianClientService.cs  # gRPC client for TuiHub API
│   ├── TokenService.cs           # JWT token management (with DB persistence)
│   ├── ConfigService.cs          # Application configuration (with DB persistence)
│   └── BackgroundTaskService.cs  # Background task management
├── Interceptors/             # gRPC interceptors
│   └── ClientTokenInterceptor.cs # Automatic token handling
├── Helpers/                  # Helper utilities
│   └── WindowHelper.cs      # Window positioning helpers
└── Program.cs               # Application entry point with DI setup
```

## Dependencies

- **TuiHub.Protos** (0.6.2) - Protocol buffer definitions for TuiHub API
- **Grpc.Net.Client** - gRPC client for .NET
- **Google.Protobuf** - Protocol buffers runtime
- **Microsoft.Extensions.DependencyInjection** - Dependency injection
- **Microsoft.Extensions.Hosting** - Application hosting
- **Microsoft.Extensions.Logging** - Logging infrastructure
- **Microsoft.EntityFrameworkCore** - ORM for database access
- **Microsoft.EntityFrameworkCore.Sqlite** - SQLite database provider

## Database

The application uses SQLite for persistent storage. The database file is located at:
- Windows: `%APPDATA%\TuiHub\Waiter\waiter.db`

### Tables
- **UserCredentials** - Stores user authentication tokens
- **AppSettings** - Key-value pairs for application settings
- **CachedApps** - Cached app information for offline viewing
- **CachedAppCategories** - Cached category information
- **TaskHistories** - Background task history

## API Reference

This application uses the TuiHub Protos library to communicate with the server. The main service is `LibrarianSephirahService` which provides:

### Tiphereth (Authentication & User Management)
- `GetToken` - Login with username/password
- `RefreshToken` - Refresh access token
- `GetUser` / `UpdateUser` - Get/update current user info
- `RegisterUser` - Register a new user
- `RegisterDevice` - Register a device
- `ListUserSessions` / `DeleteUserSession` - Session management
- `LinkAccount` / `UnLinkAccount` / `ListLinkAccounts` - Account linking

### Gebura (App Management)
- `ListApps` / `CreateApp` / `UpdateApp` / `DeleteApp` - App CRUD operations
- `SearchStoreApps` / `GetStoreAppSummary` / `AcquireStoreApp` - Store operations
- `SearchAppInfos` - Search app information
- `ListAppCategories` / `CreateAppCategory` / `UpdateAppCategory` / `DeleteAppCategory` - Category management
- `ListAppSaveFiles` / `UploadAppSaveFile` / `DownloadAppSaveFile` / `DeleteAppSaveFile` - Save file management
- `PinAppSaveFile` / `UnpinAppSaveFile` / `GetAppSaveFileCapacity` / `SetAppSaveFileCapacity` - Save file capacity
- `BatchCreateAppRunTime` / `ListAppRunTimes` / `DeleteAppRunTime` / `SumAppRunTime` - Runtime tracking
- `ListStoreAppBinaries` / `DownloadStoreAppBinary` / `ListStoreAppBinaryFiles` - Store app binaries
- `ListStoreAppSaveFiles` / `DownloadStoreAppSaveFile` - Store app save files

### Yesod (Feed Management)
- `CreateFeedConfig` / `UpdateFeedConfig` / `ListFeedConfigs` - Feed configuration
- `ListFeedItems` / `GetFeedItem` / `GetBatchFeedItems` / `ReadFeedItem` - Feed items
- `ListFeedCategories` / `ListFeedPlatforms` - Feed metadata
- `CreateFeedItemCollection` / `UpdateFeedItemCollection` / `ListFeedItemCollections` - Collections
- `AddFeedItemToCollection` / `RemoveFeedItemFromCollection` / `ListFeedItemsInCollection` - Collection items
- `CreateFeedActionSet` / `UpdateFeedActionSet` / `ListFeedActionSets` - Feed actions

### Netzach (Notification Management)
- `CreateNotifyTarget` / `UpdateNotifyTarget` / `ListNotifyTargets` - Notification targets
- `CreateNotifyFlow` / `UpdateNotifyFlow` / `ListNotifyFlows` - Notification flows
- `ListSystemNotifications` / `UpdateSystemNotification` - System notifications

### Chesed (Image Management)
- `UploadImage` / `UpdateImage` / `ListImages` - Image operations
- `SearchImages` / `GetImage` / `DownloadImage` - Image retrieval

### Binah (File Operations)
- `PresignedUploadFile` / `PresignedUploadFileStatus` - File upload
- `PresignedDownloadFile` - File download
- `GetStorageCapacityUsage` - Storage usage

### Angela (Porter Management)
- `ListPorterDigests` - List porter digests
- `CreatePorterContext` / `ListPorterContexts` / `UpdatePorterContext` - Porter context management

## License

MIT License