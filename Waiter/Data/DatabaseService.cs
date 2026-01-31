using Microsoft.EntityFrameworkCore;
using Waiter.Data.Models;

namespace Waiter.Data
{
    /// <summary>
    /// Service for database operations.
    /// Provides methods to interact with the SQLite database.
    /// </summary>
    public class DatabaseService : IDisposable
    {
        private readonly WaiterDbContext _context;
        private bool _disposed;

        public DatabaseService()
        {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var waiterFolder = Path.Combine(folder, "TuiHub", "Waiter");
            var dbPath = Path.Combine(waiterFolder, "waiter.db");
            _context = new WaiterDbContext(dbPath);
        }

        public DatabaseService(string dbPath)
        {
            _context = new WaiterDbContext(dbPath);
        }

        /// <summary>
        /// Ensures the database is created and migrations are applied.
        /// </summary>
        public async Task InitializeAsync()
        {
            await _context.Database.EnsureCreatedAsync();
        }

        #region UserCredential Operations

        public async Task<UserCredential?> GetActiveCredentialAsync()
        {
            return await _context.UserCredentials
                .Where(c => c.IsActive)
                .OrderByDescending(c => c.UpdatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<UserCredential?> GetCredentialByServerUrlAsync(string serverUrl)
        {
            return await _context.UserCredentials
                .Where(c => c.ServerUrl == serverUrl)
                .OrderByDescending(c => c.UpdatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task SaveCredentialAsync(string username, string serverUrl, string? accessToken, string? refreshToken)
        {
            var credential = await _context.UserCredentials
                .FirstOrDefaultAsync(c => c.ServerUrl == serverUrl && c.Username == username);

            if (credential == null)
            {
                credential = new UserCredential
                {
                    Username = username,
                    ServerUrl = serverUrl
                };
                _context.UserCredentials.Add(credential);
            }

            // Deactivate other credentials
            var otherCredentials = await _context.UserCredentials
                .Where(c => c.Id != credential.Id)
                .ToListAsync();
            foreach (var c in otherCredentials)
            {
                c.IsActive = false;
            }

            credential.AccessToken = accessToken;
            credential.RefreshToken = refreshToken;
            credential.LastTokenUpdate = DateTime.UtcNow;
            credential.UpdatedAt = DateTime.UtcNow;
            credential.IsActive = true;

            await _context.SaveChangesAsync();
        }

        public async Task UpdateTokensAsync(string serverUrl, string? accessToken, string? refreshToken)
        {
            var credential = await _context.UserCredentials
                .FirstOrDefaultAsync(c => c.ServerUrl == serverUrl && c.IsActive);

            if (credential != null)
            {
                credential.AccessToken = accessToken;
                credential.RefreshToken = refreshToken;
                credential.LastTokenUpdate = DateTime.UtcNow;
                credential.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task ClearActiveCredentialAsync()
        {
            var activeCredential = await GetActiveCredentialAsync();
            if (activeCredential != null)
            {
                activeCredential.AccessToken = null;
                activeCredential.RefreshToken = null;
                activeCredential.IsActive = false;
                activeCredential.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        #endregion

        #region AppSetting Operations

        public async Task<string?> GetSettingAsync(string key)
        {
            var setting = await _context.AppSettings.FindAsync(key);
            return setting?.Value;
        }

        public async Task SetSettingAsync(string key, string? value)
        {
            var setting = await _context.AppSettings.FindAsync(key);
            if (setting == null)
            {
                setting = new AppSetting { Key = key };
                _context.AppSettings.Add(setting);
            }
            setting.Value = value;
            setting.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        #endregion

        #region CachedApp Operations

        public async Task<List<CachedApp>> GetCachedAppsAsync(string serverUrl)
        {
            return await _context.CachedApps
                .Where(a => a.ServerUrl == serverUrl)
                .OrderBy(a => a.Name)
                .ToListAsync();
        }

        public async Task SaveCachedAppAsync(CachedApp app)
        {
            var existing = await _context.CachedApps
                .FirstOrDefaultAsync(a => a.Id == app.Id && a.ServerUrl == app.ServerUrl);

            if (existing != null)
            {
                existing.Name = app.Name;
                existing.Description = app.Description;
                existing.Developer = app.Developer;
                existing.Publisher = app.Publisher;
                existing.AppType = app.AppType;
                existing.CachedAt = DateTime.UtcNow;
            }
            else
            {
                app.CachedAt = DateTime.UtcNow;
                _context.CachedApps.Add(app);
            }

            await _context.SaveChangesAsync();
        }

        public async Task ClearCachedAppsAsync(string serverUrl)
        {
            var apps = await _context.CachedApps
                .Where(a => a.ServerUrl == serverUrl)
                .ToListAsync();
            _context.CachedApps.RemoveRange(apps);
            await _context.SaveChangesAsync();
        }

        #endregion

        #region CachedAppCategory Operations

        public async Task<List<CachedAppCategory>> GetCachedAppCategoriesAsync(string serverUrl)
        {
            return await _context.CachedAppCategories
                .Where(c => c.ServerUrl == serverUrl)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task SaveCachedAppCategoryAsync(CachedAppCategory category)
        {
            var existing = await _context.CachedAppCategories
                .FirstOrDefaultAsync(c => c.Id == category.Id && c.ServerUrl == category.ServerUrl);

            if (existing != null)
            {
                existing.Name = category.Name;
                existing.CachedAt = DateTime.UtcNow;
            }
            else
            {
                category.CachedAt = DateTime.UtcNow;
                _context.CachedAppCategories.Add(category);
            }

            await _context.SaveChangesAsync();
        }

        #endregion

        #region TaskHistory Operations

        public async Task<List<TaskHistory>> GetRecentTaskHistoryAsync(int count = 50)
        {
            return await _context.TaskHistories
                .OrderByDescending(t => t.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task SaveTaskHistoryAsync(TaskHistory history)
        {
            var existing = await _context.TaskHistories
                .FirstOrDefaultAsync(t => t.TaskId == history.TaskId);

            if (existing != null)
            {
                existing.Status = history.Status;
                existing.StatusMessage = history.StatusMessage;
                existing.Progress = history.Progress;
                existing.StartTime = history.StartTime;
                existing.EndTime = history.EndTime;
            }
            else
            {
                _context.TaskHistories.Add(history);
            }

            await _context.SaveChangesAsync();
        }

        public async Task ClearOldTaskHistoryAsync(int daysToKeep = 30)
        {
            var cutoff = DateTime.UtcNow.AddDays(-daysToKeep);
            var oldTasks = await _context.TaskHistories
                .Where(t => t.CreatedAt < cutoff)
                .ToListAsync();
            _context.TaskHistories.RemoveRange(oldTasks);
            await _context.SaveChangesAsync();
        }

        #endregion

        #region AppPackageLaunchSettings Operations

        /// <summary>
        /// Gets launch settings for an app package.
        /// </summary>
        /// <param name="appPackageId">App package ID</param>
        /// <returns>Launch settings or null if not found</returns>
        public async Task<AppPackageLaunchSettings?> GetLaunchSettingsAsync(long appPackageId)
        {
            return await _context.AppPackageLaunchSettings
                .FirstOrDefaultAsync(x => x.AppPackageId == appPackageId);
        }

        /// <summary>
        /// Saves launch settings for an app package.
        /// </summary>
        /// <param name="settings">Launch settings to save</param>
        public async Task SaveLaunchSettingsAsync(AppPackageLaunchSettings settings)
        {
            if (settings.Id == 0)
            {
                _context.AppPackageLaunchSettings.Add(settings);
            }
            else
            {
                _context.AppPackageLaunchSettings.Update(settings);
            }

            await _context.SaveChangesAsync();
        }

        #endregion

        #region RuntimeSession Operations

        /// <summary>
        /// Creates a new runtime session.
        /// </summary>
        /// <param name="session">Runtime session to create</param>
        /// <returns>Created session with ID</returns>
        public async Task<RuntimeSession> CreateRuntimeSessionAsync(RuntimeSession session)
        {
            _context.RuntimeSessions.Add(session);
            await _context.SaveChangesAsync();
            return session;
        }

        /// <summary>
        /// Updates an existing runtime session.
        /// </summary>
        /// <param name="session">Runtime session to update</param>
        public async Task UpdateRuntimeSessionAsync(RuntimeSession session)
        {
            _context.RuntimeSessions.Update(session);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Gets a runtime session by ID.
        /// </summary>
        /// <param name="sessionId">Session ID</param>
        /// <returns>Runtime session or null if not found</returns>
        public async Task<RuntimeSession?> GetRuntimeSessionAsync(long sessionId)
        {
            return await _context.RuntimeSessions.FindAsync(sessionId);
        }

        /// <summary>
        /// Gets recent runtime sessions for an app package.
        /// </summary>
        /// <param name="appPackageId">App package ID</param>
        /// <param name="count">Maximum number of sessions to return</param>
        /// <returns>List of recent sessions</returns>
        public async Task<List<RuntimeSession>> GetRecentSessionsAsync(long appPackageId, int count = 10)
        {
            return await _context.RuntimeSessions
                .Where(s => s.AppPackageId == appPackageId)
                .OrderByDescending(s => s.StartTime)
                .Take(count)
                .ToListAsync();
        }

        /// <summary>
        /// Gets total runtime for an app package.
        /// </summary>
        /// <param name="appPackageId">App package ID</param>
        /// <returns>Total runtime duration</returns>
        public async Task<TimeSpan> GetTotalRuntimeAsync(long appPackageId)
        {
            var sessions = await _context.RuntimeSessions
                .Where(s => s.AppPackageId == appPackageId && s.EndTime != null)
                .ToListAsync();

            var totalTicks = sessions
                .Where(s => s.Duration.HasValue)
                .Sum(s => s.Duration!.Value.Ticks);

            return TimeSpan.FromTicks(totalTicks);
        }

        #endregion

        #region CachedUpload Operations

        /// <summary>
        /// Creates a new cached upload record.
        /// </summary>
        /// <param name="upload">Cached upload to create</param>
        /// <returns>Created upload with ID</returns>
        public async Task<CachedUpload> CreateCachedUploadAsync(CachedUpload upload)
        {
            _context.CachedUploads.Add(upload);
            await _context.SaveChangesAsync();
            return upload;
        }

        /// <summary>
        /// Gets all pending uploads that haven't expired.
        /// </summary>
        /// <returns>List of pending uploads</returns>
        public async Task<List<CachedUpload>> GetPendingUploadsAsync()
        {
            return await _context.CachedUploads
                .Where(u => u.ExpiresAt > DateTime.UtcNow)
                .OrderBy(u => u.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Gets a cached upload by ID.
        /// </summary>
        /// <param name="uploadId">Upload ID</param>
        /// <returns>Cached upload or null if not found</returns>
        public async Task<CachedUpload?> GetCachedUploadAsync(long uploadId)
        {
            return await _context.CachedUploads.FindAsync(uploadId);
        }

        /// <summary>
        /// Updates an existing cached upload.
        /// </summary>
        /// <param name="upload">Cached upload to update</param>
        public async Task UpdateCachedUploadAsync(CachedUpload upload)
        {
            _context.CachedUploads.Update(upload);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes a cached upload by ID.
        /// </summary>
        /// <param name="uploadId">Upload ID</param>
        public async Task DeleteCachedUploadAsync(long uploadId)
        {
            var upload = await _context.CachedUploads.FindAsync(uploadId);
            if (upload != null)
            {
                _context.CachedUploads.Remove(upload);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Gets all expired cached uploads.
        /// </summary>
        /// <returns>List of expired uploads</returns>
        public async Task<List<CachedUpload>> GetExpiredCachedUploadsAsync()
        {
            return await _context.CachedUploads
                .Where(u => u.ExpiresAt < DateTime.UtcNow)
                .ToListAsync();
        }

        #endregion

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                _disposed = true;
            }
        }
    }
}
