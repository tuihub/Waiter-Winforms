# Waiter-Winforms

A WinForms client application for TuiHub, designed with a classic Steam-like interface for managing Apps in the TuiHub ecosystem.

## Features

- **Login and Authentication**
  - JWT-based authentication with username/password login
  - Automatic token refresh using RefreshToken
  - Support for DownloadToken (reserved for future use)
  - Token interceptor pattern for seamless API authentication

- **App Management**
  - Browse and manage your app library
  - Search apps in the store
  - Acquire apps from the store
  - View app details

- **App Category Management**
  - Create, update, and delete app categories
  - Organize apps into categories

- **Background Task Management**
  - Download app tasks (stub implementation)
  - Sync save file tasks (stub implementation)
  - Task progress tracking

- **Settings**
  - Configurable server URL
  - Persistent configuration storage

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
├── Forms/                    # WinForms UI
│   ├── MainForm.cs          # Main application window (Steam-like layout)
│   ├── LoginForm.cs         # Login dialog
│   ├── SettingsForm.cs      # Settings dialog
│   ├── AppCategoryForm.cs   # App category management
│   └── BackgroundTasksForm.cs # Background task viewer
├── Services/                 # Core services
│   ├── LibrarianClientService.cs  # gRPC client for TuiHub API
│   ├── TokenService.cs           # JWT token management
│   ├── ConfigService.cs          # Application configuration
│   └── BackgroundTaskService.cs  # Background task management
├── Interceptors/             # gRPC interceptors
│   └── ClientTokenInterceptor.cs # Automatic token handling
├── Helpers/                  # Helper utilities
│   └── WindowHelper.cs      # Window positioning helpers
└── Program.cs               # Application entry point with DI setup
```

## Dependencies

- **TuiHub.Protos** - Protocol buffer definitions for TuiHub API
- **Grpc.Net.Client** - gRPC client for .NET
- **Google.Protobuf** - Protocol buffers runtime
- **Microsoft.Extensions.DependencyInjection** - Dependency injection
- **Microsoft.Extensions.Hosting** - Application hosting
- **Microsoft.Extensions.Logging** - Logging infrastructure

## API Reference

This application uses the TuiHub Protos library to communicate with the server. The main service is `LibrarianSephirahService` which provides:

### Tiphereth (Authentication)
- `GetToken` - Login with username/password
- `RefreshToken` - Refresh access token
- `GetUser` - Get current user info

### Gebura (App Management)
- `ListApps` - List user's apps
- `CreateApp` - Create a new app
- `UpdateApp` - Update app information
- `SearchStoreApps` - Search apps in store
- `GetStoreAppSummary` - Get store app details
- `AcquireStoreApp` - Acquire an app from store
- `ListAppCategories` / `CreateAppCategory` / `UpdateAppCategory` / `DeleteAppCategory` - Category management
- `ListAppSaveFiles` / `UploadAppSaveFile` / `DownloadAppSaveFile` - Save file management
- `BatchCreateAppRunTime` / `ListAppRunTimes` - Runtime tracking

## License

MIT License