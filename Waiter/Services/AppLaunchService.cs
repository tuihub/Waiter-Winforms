using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Waiter.Data;
using Waiter.Data.Models;

namespace Waiter.Services
{
    /// <summary>
    /// Service for orchestrating app launch, runtime tracking, and post-processing.
    /// </summary>
    public class AppLaunchService : IAppLaunchService
    {
        private readonly ILogger<AppLaunchService> _logger;
        private readonly IProcessMonitorService _processMonitor;
        private readonly ISaveDataService _saveDataService;
        private readonly LibrarianClientService _librarianClient;
        private readonly DatabaseService _database;
        private readonly ConfigService _config;

        // Default device ID - TODO: implement proper device registration
        private const long DefaultDeviceId = 0;

        public AppLaunchService(
            ILogger<AppLaunchService> logger,
            IProcessMonitorService processMonitor,
            ISaveDataService saveDataService,
            LibrarianClientService librarianClient,
            DatabaseService database,
            ConfigService config)
        {
            _logger = logger;
            _processMonitor = processMonitor;
            _saveDataService = saveDataService;
            _librarianClient = librarianClient;
            _database = database;
            _config = config;
        }

        /// <inheritdoc />
        public async Task<LaunchValidationResult> ValidateLaunchConfigurationAsync(long appPackageId)
        {
            var errors = new List<string>();

            var settings = await _database.GetLaunchSettingsAsync(appPackageId);
            if (settings == null)
            {
                errors.Add("Launch settings not configured for this app.");
                return new LaunchValidationResult(false, errors);
            }

            // Check executable path
            if (string.IsNullOrWhiteSpace(settings.ExecutablePath))
            {
                errors.Add("Executable path is not configured.");
            }
            else if (!File.Exists(settings.ExecutablePath))
            {
                errors.Add($"Executable not found: {settings.ExecutablePath}");
            }

            // Check working directory
            if (string.IsNullOrWhiteSpace(settings.WorkingDirectory))
            {
                errors.Add("Working directory is not configured.");
            }
            else if (!Directory.Exists(settings.WorkingDirectory))
            {
                errors.Add($"Working directory not found: {settings.WorkingDirectory}");
            }

            // Check process name if using ProcessName mode
            if (settings.MonitoringMode == MonitoringMode.ProcessName &&
                string.IsNullOrWhiteSpace(settings.ProcessName))
            {
                errors.Add("Process name is required for process listen mode.");
            }

            return new LaunchValidationResult(errors.Count == 0, errors);
        }

        /// <inheritdoc />
        public async Task<RuntimeSession> LaunchAndTrackAsync(
            long appPackageId,
            IProgress<LaunchProgress>? progress,
            CancellationToken cancellationToken)
        {
            // 1. Load settings
            progress?.Report(new LaunchProgress(LaunchPhase.Validating, "Loading configuration..."));
            var settings = await _database.GetLaunchSettingsAsync(appPackageId)
                ?? throw new InvalidOperationException($"Launch settings not found for app {appPackageId}");

            _logger.LogInformation("Launching app {AppPackageId} with executable {Executable}",
                appPackageId, settings.ExecutablePath);

            // 2. Create runtime session record
            var session = new RuntimeSession
            {
                AppPackageId = appPackageId,
                DeviceId = DefaultDeviceId, // TODO: get actual device ID
                StartTime = DateTime.UtcNow,
                Status = SessionStatus.Tracking
            };
            session = await _database.CreateRuntimeSessionAsync(session);

            try
            {
                // 3. Start process
                progress?.Report(new LaunchProgress(LaunchPhase.Starting, "Starting application..."));
                var startInfo = new ProcessStartInfo
                {
                    FileName = settings.ExecutablePath,
                    WorkingDirectory = settings.WorkingDirectory,
                    UseShellExecute = settings.UseShellExecute
                };

                var process = Process.Start(startInfo);
                if (process == null)
                {
                    throw new InvalidOperationException("Failed to start process");
                }

                _logger.LogInformation("Process started: {ProcessName} (PID: {ProcessId})",
                    process.ProcessName, process.Id);

                // 4. Monitor process
                ProcessResult result;

                if (settings.MonitoringMode == MonitoringMode.DirectProcess)
                {
                    progress?.Report(new LaunchProgress(LaunchPhase.Tracking, $"Monitoring {process.ProcessName}..."));
                    result = await _processMonitor.TrackProcessAsync(process, cancellationToken);
                }
                else
                {
                    progress?.Report(new LaunchProgress(LaunchPhase.WaitingForProcess,
                        $"Waiting for {settings.ProcessName}..."));
                    var timeout = TimeSpan.FromSeconds(settings.LaunchTimeout);
                    result = await _processMonitor.TrackProcessByNameAsync(
                        settings.ProcessName!, timeout, cancellationToken);
                }

                // 5. Update session with results
                session.EndTime = DateTime.UtcNow;
                session.ExitCode = result.ExitCode;
                session.Status = result.IsAbnormalExit ? SessionStatus.Abnormal : SessionStatus.Processing;
                await _database.UpdateRuntimeSessionAsync(session);

                _logger.LogInformation("Session {SessionId} completed: ExitCode={ExitCode}, Duration={Duration}",
                    session.Id, session.ExitCode, session.Duration);

                progress?.Report(new LaunchProgress(LaunchPhase.Complete, "Application closed"));
                return session;
            }
            catch (Exception ex)
            {
                // Update session as failed
                session.EndTime = DateTime.UtcNow;
                session.Status = SessionStatus.Failed;
                await _database.UpdateRuntimeSessionAsync(session);

                _logger.LogError(ex, "Error during launch/tracking for session {SessionId}", session.Id);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task UploadSessionDataAsync(
            long sessionId,
            IProgress<UploadProgress>? progress,
            CancellationToken cancellationToken)
        {
            var session = await _database.GetRuntimeSessionAsync(sessionId)
                ?? throw new InvalidOperationException($"Session not found: {sessionId}");

            try
            {
                // 1. Report runtime
                progress?.Report(new UploadProgress(UploadPhase.ReportingRuntime, "Uploading runtime statistics..."));
                await _librarianClient.ReportRuntimeAsync(session, cancellationToken);

                // 2. Handle save data if configured
                var settings = await _database.GetLaunchSettingsAsync(session.AppPackageId);
                if (settings != null && !string.IsNullOrWhiteSpace(settings.SaveDataPath))
                {
                    await UploadSaveDataAsync(session, settings.SaveDataPath, progress, cancellationToken);
                }

                // 3. Update session status
                session.UploadAttempted = true;
                session.UploadedAt = DateTime.UtcNow;
                session.Status = SessionStatus.Completed;
                await _database.UpdateRuntimeSessionAsync(session);

                progress?.Report(new UploadProgress(UploadPhase.Complete, "Upload complete"));
                _logger.LogInformation("Upload completed for session {SessionId}", sessionId);
            }
            catch (Exception ex)
            {
                // Mark upload as attempted but failed
                session.UploadAttempted = true;
                session.Status = SessionStatus.Failed;
                await _database.UpdateRuntimeSessionAsync(session);

                // Try to cache the runtime data for later retry
                try
                {
                    await CacheRuntimeDataAsync(session);
                    _logger.LogWarning("Upload failed for session {SessionId}, data cached for retry", sessionId);
                }
                catch (Exception cacheEx)
                {
                    _logger.LogError(cacheEx, "Failed to cache runtime data for session {SessionId}", sessionId);
                }

                _logger.LogError(ex, "Upload failed for session {SessionId}", sessionId);
                throw;
            }
        }

        /// <summary>
        /// Caches runtime data locally when upload fails.
        /// </summary>
        private async Task CacheRuntimeDataAsync(RuntimeSession session)
        {
            var cacheDir = GetCacheDirectory();
            var fileName = $"{Guid.NewGuid()}_runtime.json";
            var filePath = Path.Combine(cacheDir, fileName);

            var metadata = new RuntimeDataMetadata(
                session.AppPackageId,
                session.DeviceId,
                session.StartTime,
                session.EndTime ?? DateTime.UtcNow);

            var json = JsonSerializer.Serialize(metadata);
            await File.WriteAllTextAsync(filePath, json);

            var cachedUpload = new CachedUpload
            {
                RuntimeSessionId = session.Id,
                UploadType = Data.Models.UploadType.RuntimeData,
                FilePath = filePath,
                Metadata = json,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(30)
            };

            await _database.CreateCachedUploadAsync(cachedUpload);
            _logger.LogInformation("Cached runtime data for session {SessionId} to {FilePath}", session.Id, filePath);
        }

        /// <summary>
        /// Gets the cache directory for pending uploads.
        /// </summary>
        private string GetCacheDirectory()
        {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var cacheDir = Path.Combine(folder, "TuiHub", "Waiter", "pending-uploads");
            Directory.CreateDirectory(cacheDir);
            return cacheDir;
        }

        private record RuntimeDataMetadata(long AppPackageId, long DeviceId, DateTime StartTime, DateTime EndTime);

        private async Task UploadSaveDataAsync(
            RuntimeSession session,
            string saveDataPath,
            IProgress<UploadProgress>? progress,
            CancellationToken cancellationToken)
        {
            if (!Directory.Exists(saveDataPath))
            {
                _logger.LogWarning("Save data path does not exist: {SaveDataPath}", saveDataPath);
                return;
            }

            // Create archive
            progress?.Report(new UploadProgress(UploadPhase.CreatingArchive, "Compressing save data..."));
            var archiveFile = await _saveDataService.CreateSaveArchiveAsync(saveDataPath, null, cancellationToken);
            var shouldDeleteArchive = true;

            try
            {
                // Calculate hash
                progress?.Report(new UploadProgress(UploadPhase.CalculatingHash, "Verifying file integrity..."));
                var sha256Hash = await _saveDataService.CalculateSHA256Async(archiveFile, cancellationToken);

                try
                {
                    // Get upload token
                    var uploadToken = await _librarianClient.GetSaveFileUploadTokenAsync(
                        session.AppPackageId, archiveFile, sha256Hash, cancellationToken);

                    // Upload file with progress
                    var uploadProgress = new Progress<int>(percent =>
                    {
                        progress?.Report(new UploadProgress(UploadPhase.UploadingSave,
                            $"Uploading save file... {percent}%", percent));
                    });

                    await _librarianClient.UploadSaveFileDataAsync(uploadToken, archiveFile, uploadProgress, cancellationToken);
                }
                catch (Exception ex)
                {
                    // Cache save data for later retry
                    _logger.LogWarning(ex, "Save data upload failed for session {SessionId}, caching for retry", session.Id);
                    await CacheSaveDataAsync(session, archiveFile, sha256Hash);
                    shouldDeleteArchive = false; // Keep the archive for later retry
                    throw;
                }
            }
            finally
            {
                // Cleanup temp archive only if not cached
                if (shouldDeleteArchive)
                {
                    try
                    {
                        if (archiveFile.Exists)
                        {
                            archiveFile.Delete();
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to delete temp archive: {FilePath}", archiveFile.FullName);
                    }
                }
            }
        }

        /// <summary>
        /// Caches save data locally when upload fails.
        /// </summary>
        private async Task CacheSaveDataAsync(RuntimeSession session, FileInfo archiveFile, string sha256Hash)
        {
            var cacheDir = GetCacheDirectory();
            var fileName = $"{Guid.NewGuid()}_{archiveFile.Name}";
            var cachedFilePath = Path.Combine(cacheDir, fileName);

            // Move archive to cache directory
            archiveFile.MoveTo(cachedFilePath);

            var metadata = new SaveFileMetadata(
                session.AppPackageId,
                sha256Hash,
                archiveFile.Name,
                new FileInfo(cachedFilePath).Length);

            var cachedUpload = new CachedUpload
            {
                RuntimeSessionId = session.Id,
                UploadType = Data.Models.UploadType.SaveFile,
                FilePath = cachedFilePath,
                Metadata = JsonSerializer.Serialize(metadata),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(30)
            };

            await _database.CreateCachedUploadAsync(cachedUpload);
            _logger.LogInformation("Cached save data for session {SessionId} to {FilePath}", session.Id, cachedFilePath);
        }

        /// <inheritdoc />
        public async Task<RunningProcessInfo?> CheckIfRunningAsync(long appPackageId)
        {
            var settings = await _database.GetLaunchSettingsAsync(appPackageId);
            if (settings == null)
            {
                return null;
            }

            // Get process name from executable path or explicit setting
            var processName = settings.MonitoringMode == MonitoringMode.ProcessName && !string.IsNullOrWhiteSpace(settings.ProcessName)
                ? settings.ProcessName
                : Path.GetFileNameWithoutExtension(settings.ExecutablePath);

            return _processMonitor.FindRunningProcess(processName);
        }

        /// <inheritdoc />
        public async Task RetryUploadAsync(long cachedUploadId, CancellationToken cancellationToken)
        {
            var cachedUpload = await _database.GetCachedUploadAsync(cachedUploadId)
                ?? throw new InvalidOperationException($"Cached upload not found: {cachedUploadId}");

            if (cachedUpload.RetryCount >= 10)
            {
                _logger.LogWarning("Maximum retry count reached for cached upload {UploadId}", cachedUploadId);
                return;
            }

            try
            {
                if (cachedUpload.UploadType == Data.Models.UploadType.RuntimeData && cachedUpload.RuntimeSessionId.HasValue)
                {
                    var session = await _database.GetRuntimeSessionAsync(cachedUpload.RuntimeSessionId.Value);
                    if (session != null)
                    {
                        await _librarianClient.ReportRuntimeAsync(session, cancellationToken);
                    }
                }
                else if (cachedUpload.UploadType == Data.Models.UploadType.SaveFile)
                {
                    var metadata = JsonSerializer.Deserialize<SaveFileMetadata>(cachedUpload.Metadata);
                    if (metadata != null && File.Exists(cachedUpload.FilePath))
                    {
                        var fileInfo = new FileInfo(cachedUpload.FilePath);
                        var uploadToken = await _librarianClient.GetSaveFileUploadTokenAsync(
                            metadata.AppId, fileInfo, metadata.Sha256, cancellationToken);
                        await _librarianClient.UploadSaveFileDataAsync(uploadToken, fileInfo, null, cancellationToken);
                    }
                }

                // Success - delete cached upload
                if (File.Exists(cachedUpload.FilePath))
                {
                    File.Delete(cachedUpload.FilePath);
                }
                await _database.DeleteCachedUploadAsync(cachedUploadId);

                _logger.LogInformation("Successfully retried cached upload {UploadId}", cachedUploadId);
            }
            catch (Exception ex)
            {
                // Update retry count and error
                cachedUpload.RetryCount++;
                cachedUpload.LastError = ex.Message;
                await _database.UpdateCachedUploadAsync(cachedUpload);

                _logger.LogError(ex, "Retry failed for cached upload {UploadId}, attempt {RetryCount}",
                    cachedUploadId, cachedUpload.RetryCount);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task CleanupExpiredCacheAsync()
        {
            var expiredUploads = await _database.GetExpiredCachedUploadsAsync();

            _logger.LogInformation("Cleaning up {Count} expired cached uploads", expiredUploads.Count);

            foreach (var upload in expiredUploads)
            {
                try
                {
                    if (File.Exists(upload.FilePath))
                    {
                        File.Delete(upload.FilePath);
                    }
                    await _database.DeleteCachedUploadAsync(upload.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to cleanup cached upload {UploadId}", upload.Id);
                }
            }
        }

        private record SaveFileMetadata(long AppId, string Sha256, string FileName, long FileSize);
    }
}
