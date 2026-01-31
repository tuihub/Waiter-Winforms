# API Contracts: App Launch with Runtime Tracking

**Phase**: 1 (Design & Contracts)  
**Date**: 2026-01-31  
**Status**: Complete

This document defines all API contracts between the Waiter-Winforms client and TuiHub server for app launch and runtime tracking functionality.

---

## Overview

All server communication uses gRPC via `LibrarianSephirahService` from TuiHub.Protos package. The client follows Constitution Principle III: all API calls routed through `LibrarianClientService` with automatic authentication via `ClientTokenInterceptor`.

**Service**: `TuiHub.Protos.Librarian.Sephirah.V1.LibrarianSephirahService`  
**Authentication**: JWT token via interceptor (automatic)  
**Error Handling**: gRPC status codes wrapped in meaningful user messages

---

## 1. Report Runtime Statistics

### RPC Method
```protobuf
rpc BatchCreateAppRunTime(BatchCreateAppRunTimeRequest) returns (BatchCreateAppRunTimeResponse);
```

### Request Contract

**Proto Definition**:
```protobuf
message BatchCreateAppRunTimeRequest {
  repeated AppRunTime app_run_times = 1;
}

message AppRunTime {
  librarian.v1.InternalID id = 1;           // Runtime session ID (client-generated or 0)
  librarian.v1.InternalID app_id = 2;        // App package ID from TuiHub
  librarian.v1.InternalID device_id = 3;     // Current device ID
  librarian.v1.TimeRange run_time = 4;       // Start and end timestamps
}

message TimeRange {
  google.protobuf.Timestamp start_time = 1;
  google.protobuf.Timestamp end_time = 2;
}
```

**C# Client Usage**:
```csharp
var request = new BatchCreateAppRunTimeRequest();
request.AppRunTimes.Add(new AppRunTime
{
    Id = new InternalID { Id = 0 },  // Server assigns ID
    AppId = new InternalID { Id = appPackageId },
    DeviceId = new InternalID { Id = currentDeviceId },
    RunTime = new TimeRange
    {
        StartTime = Timestamp.FromDateTime(session.StartTime.ToUniversalTime()),
        EndTime = Timestamp.FromDateTime(session.EndTime.Value.ToUniversalTime())
    }
});

var response = await client.BatchCreateAppRunTimeAsync(request, cancellationToken: ct);
```

### Response Contract

**Proto Definition**:
```protobuf
message BatchCreateAppRunTimeResponse {}  // Empty response on success
```

**Success Behavior**: HTTP 200, empty response body  
**Error Codes**:
- `UNAUTHENTICATED`: Token expired or invalid (handled by interceptor)
- `INVALID_ARGUMENT`: Invalid app_id, device_id, or time range
- `PERMISSION_DENIED`: User doesn't own the app package
- `UNAVAILABLE`: Server temporarily unavailable

---

## 2. Upload Save File (Step 1: Get Upload Token)

### RPC Method
```protobuf
rpc UploadAppSaveFile(UploadAppSaveFileRequest) returns (UploadAppSaveFileResponse);
```

### Request Contract

**Proto Definition**:
```protobuf
message UploadAppSaveFileRequest {
  librarian.v1.FileMetadata file_metadata = 1;
  librarian.v1.InternalID app_id = 2;
}

message FileMetadata {
  string name = 1;                          // File name (e.g., "save_20260131.zip")
  int64 size_bytes = 2;                     // File size in bytes
  string sha256 = 3;                        // SHA256 hash (hex string)
  librarian.v1.FileType type = 4;           // File type enum
}
```

**C# Client Usage**:
```csharp
var fileInfo = new FileInfo(archivePath);
var sha256Hash = await CalculateSHA256Async(fileInfo, ct);

var request = new UploadAppSaveFileRequest
{
    AppId = new InternalID { Id = appPackageId },
    FileMetadata = new FileMetadata
    {
        Name = fileInfo.Name,
        SizeBytes = fileInfo.Length,
        Sha256 = sha256Hash,
        Type = FileType.AppSave  // Enum value for save files
    }
};

var response = await client.UploadAppSaveFileAsync(request, cancellationToken: ct);
string uploadToken = response.UploadToken;
```

### Response Contract

**Proto Definition**:
```protobuf
message UploadAppSaveFileResponse {
  string upload_token = 1;  // Presigned URL or upload token for actual file transfer
}
```

**Success Behavior**: HTTP 200, returns upload token (typically a presigned S3 URL or similar)  
**Error Codes**:
- `UNAUTHENTICATED`: Token expired or invalid
- `INVALID_ARGUMENT`: Invalid file metadata (size, hash, name)
- `PERMISSION_DENIED`: User doesn't own the app package
- `RESOURCE_EXHAUSTED`: User storage quota exceeded

---

## 3. Upload Save File (Step 2: Transfer File Data)

### HTTP Upload (Presigned URL Pattern)

**Method**: `PUT`  
**URL**: Value from `UploadAppSaveFileResponse.upload_token`  
**Headers**:
```
Content-Type: application/zip
Content-Length: {file_size}
x-amz-content-sha256: {sha256_hash}  (if AWS S3)
```

**Body**: Binary file data (streaming upload)

**C# Client Usage**:
```csharp
using var httpClient = new HttpClient();
using var fileStream = File.OpenRead(archivePath);

var request = new HttpRequestMessage(HttpMethod.Put, uploadToken)
{
    Content = new StreamContent(fileStream)
};
request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/zip");
request.Content.Headers.ContentLength = fileStream.Length;

var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);
response.EnsureSuccessStatusCode();
```

**Success Behavior**: HTTP 200 or 204  
**Error Codes**:
- `400 Bad Request`: File hash mismatch
- `403 Forbidden`: Token expired or invalid
- `413 Payload Too Large`: File exceeds size limit
- `500 Internal Server Error`: Storage service error

**Notes**:
- Per Constitution Principle III, HTTP upload is acceptable for presigned URLs as an implementation detail
- Must be encapsulated in a Service method (not in Forms)
- Upload progress should be reported for files >1MB

---

## 4. Query Runtime Summary (Optional, Future Use)

### RPC Method
```protobuf
rpc SumAppRunTime(SumAppRunTimeRequest) returns (SumAppRunTimeResponse);
```

**Purpose**: Retrieve total runtime for an app across all sessions  
**Use Case**: Display accumulated playtime in app details view

**Request**:
```protobuf
message SumAppRunTimeRequest {
  librarian.v1.TimeRange time_range_cross = 1;     // Optional time filter
  repeated librarian.v1.InternalID app_id_filter = 2;  // Filter by app IDs
  repeated librarian.v1.InternalID device_id_filter = 3;  // Filter by devices
}
```

**Response**:
```protobuf
message SumAppRunTimeResponse {
  google.protobuf.Duration run_time_sum = 1;  // Total duration
}
```

**Note**: Not required for MVP; can be implemented later per FR-013 (refresh displayed runtime)

---

## Error Handling Strategy

### Network Failures (Per FR-018)

**Behavior**: Gracefully fail and cache data locally

```csharp
try
{
    await UploadRuntimeDataAsync(session, ct);
}
catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable)
{
    // Network unavailable - cache for later
    await CacheUploadDataAsync(session, UploadType.RuntimeData);
    ShowNotification("Runtime data cached. Will retry when online.");
}
catch (RpcException ex)
{
    // Other gRPC errors - log and notify user
    _logger.LogError(ex, "Failed to upload runtime data");
    ShowError($"Upload failed: {ex.Status.Detail}");
}
```

### Authentication Failures

**Behavior**: Automatic token refresh via interceptor, re-prompt login if refresh fails

```csharp
// Handled by ClientTokenInterceptor automatically
// If interceptor fails, exception bubbles up
try
{
    await client.BatchCreateAppRunTimeAsync(request);
}
catch (RpcException ex) when (ex.StatusCode == StatusCode.Unauthenticated)
{
    // Interceptor couldn't refresh token - require re-login
    await EnsureLoginHelper.EnsureAuthenticatedAsync();
    // Retry operation after login
    await client.BatchCreateAppRunTimeAsync(request);
}
```

### Validation Failures

**Behavior**: User-friendly error messages

```csharp
catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
{
    MessageBox.Show(
        $"Invalid data: {ex.Status.Detail}",
        "Upload Failed",
        MessageBoxButtons.OK,
        MessageBoxIcon.Warning
    );
}
```

---

## Service Integration

All API calls MUST go through `LibrarianClientService`. Extend the service with new methods:

```csharp
public class LibrarianClientService
{
    private readonly LibrarianSephirahService.LibrarianSephirahServiceClient _client;
    
    public async Task ReportRuntimeAsync(RuntimeSession session, CancellationToken ct)
    {
        var request = BuildBatchCreateAppRunTimeRequest(session);
        await _client.BatchCreateAppRunTimeAsync(request, cancellationToken: ct);
    }
    
    public async Task<string> GetSaveFileUploadTokenAsync(
        long appId, 
        FileInfo saveFile, 
        string sha256Hash, 
        CancellationToken ct)
    {
        var request = new UploadAppSaveFileRequest
        {
            AppId = new InternalID { Id = appId },
            FileMetadata = new FileMetadata
            {
                Name = saveFile.Name,
                SizeBytes = saveFile.Length,
                Sha256 = sha256Hash,
                Type = FileType.AppSave
            }
        };
        
        var response = await _client.UploadAppSaveFileAsync(request, cancellationToken: ct);
        return response.UploadToken;
    }
    
    public async Task UploadSaveFileAsync(
        string uploadToken, 
        FileInfo saveFile, 
        IProgress<int> progress, 
        CancellationToken ct)
    {
        // HTTP upload implementation (presigned URL)
        // Per Constitution III: acceptable as implementation detail
        using var httpClient = new HttpClient();
        using var fileStream = saveFile.OpenRead();
        
        var request = new HttpRequestMessage(HttpMethod.Put, uploadToken)
        {
            Content = new ProgressStreamContent(fileStream, progress)
        };
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/zip");
        
        var response = await httpClient.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();
    }
}
```

---

## Data Flow Diagram

```
┌──────────────┐                                    ┌──────────────────┐
│ AppLaunch    │                                    │ TuiHub Server    │
│ Service      │                                    │ (gRPC)           │
└──────┬───────┘                                    └────────┬─────────┘
       │                                                     │
       │ 1. App exits, create RuntimeSession                │
       │    (local DB)                                      │
       │                                                     │
       │ 2. BatchCreateAppRunTimeAsync()                    │
       ├────────────────────────────────────────────────────▶│
       │    {app_id, device_id, start_time, end_time}       │
       │                                                     │
       │ 3. Response (success)                              │
       │◀────────────────────────────────────────────────────┤
       │                                                     │
       │ 4. UploadAppSaveFileAsync()                        │
       ├────────────────────────────────────────────────────▶│
       │    {app_id, file_metadata}                         │
       │                                                     │
       │ 5. Response {upload_token}                         │
       │◀────────────────────────────────────────────────────┤
       │                                                     │
       │ 6. HTTP PUT to presigned URL                       │
       ├────────────────────────────────────────────────────▶│
       │    (binary file data)                              │
       │                                                     │
       │ 7. Response (200 OK)                               │
       │◀────────────────────────────────────────────────────┤
       │                                                     │
       │ 8. Update RuntimeSession.UploadedAt                │
       │    (local DB)                                      │
       │                                                     │
       
       [If network fails at step 2 or 4]
       │
       │ 9. Create CachedUpload record
       │    (local DB + cache directory)
       │
       │ [Later: manual retry via "Sync Now" button]
       │ 10. Retry steps 2-7
       │
```

---

## Summary

| Operation | RPC Method | Request | Response | Notes |
|-----------|-----------|---------|----------|-------|
| Report Runtime | `BatchCreateAppRunTime` | AppRunTime[] | Empty | Single session per call typical |
| Get Upload Token | `UploadAppSaveFile` | FileMetadata + app_id | upload_token | Step 1 of save upload |
| Upload File Data | HTTP PUT | Binary file | 200 OK | Step 2 of save upload (presigned URL) |
| Query Total Runtime | `SumAppRunTime` | Filters | Duration | Optional, future enhancement |

**All implementations**:
- MUST use LibrarianClientService
- MUST handle network failures gracefully (FR-018)
- MUST be async/await for UI responsiveness (Constitution IV)
- MUST cache failed uploads locally for manual retry

See [data-model.md](../data-model.md) for CachedUpload entity details.
