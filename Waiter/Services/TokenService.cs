using System;
using Waiter.Data;

namespace Waiter.Services
{
    /// <summary>
    /// Manages token state for authentication with the server.
    /// Supports AccessToken, RefreshToken, and DownloadToken.
    /// Persists tokens to SQLite database.
    /// </summary>
    public class TokenService
    {
        private readonly DatabaseService _databaseService;
        private readonly ConfigService _configService;
        private string _accessToken = string.Empty;
        private string _refreshToken = string.Empty;
        private string _downloadToken = string.Empty;
        private string _currentUsername = string.Empty;

        public event EventHandler? TokensChanged;

        public TokenService(DatabaseService databaseService, ConfigService configService)
        {
            _databaseService = databaseService;
            _configService = configService;
            LoadTokensFromDatabaseAsync().GetAwaiter().GetResult();
        }

        public string AccessToken
        {
            get => _accessToken;
            private set
            {
                _accessToken = value;
                TokensChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public string RefreshToken
        {
            get => _refreshToken;
            private set
            {
                _refreshToken = value;
                TokensChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public string DownloadToken
        {
            get => _downloadToken;
            private set
            {
                _downloadToken = value;
                TokensChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public string CurrentUsername => _currentUsername;

        public bool IsLoggedIn => !string.IsNullOrWhiteSpace(AccessToken);

        /// <summary>
        /// Loads tokens from the database for the current server URL.
        /// </summary>
        private async Task LoadTokensFromDatabaseAsync()
        {
            var credential = await _databaseService.GetActiveCredentialAsync();
            if (credential != null && credential.ServerUrl == _configService.ServerUrl)
            {
                _accessToken = credential.AccessToken ?? string.Empty;
                _refreshToken = credential.RefreshToken ?? string.Empty;
                _currentUsername = credential.Username;
            }
        }

        /// <summary>
        /// Sets both access and refresh tokens atomically.
        /// Fires TokensChanged event only once for both tokens.
        /// Persists tokens to database.
        /// </summary>
        public void SetTokens(string accessToken, string refreshToken)
        {
            _accessToken = accessToken;
            _refreshToken = refreshToken;
            TokensChanged?.Invoke(this, EventArgs.Empty);

            // Persist to database asynchronously with error handling
            _ = Task.Run(async () =>
            {
                try
                {
                    await _databaseService.UpdateTokensAsync(_configService.ServerUrl, accessToken, refreshToken);
                }
                catch
                {
                    // Silently ignore database errors - tokens are still in memory
                }
            });
        }

        /// <summary>
        /// Sets tokens and username after successful login.
        /// </summary>
        public async Task SetTokensWithUsernameAsync(string username, string accessToken, string refreshToken)
        {
            _accessToken = accessToken;
            _refreshToken = refreshToken;
            _currentUsername = username;
            TokensChanged?.Invoke(this, EventArgs.Empty);

            await _databaseService.SaveCredentialAsync(username, _configService.ServerUrl, accessToken, refreshToken);
        }

        public void SetDownloadToken(string downloadToken)
        {
            DownloadToken = downloadToken;
        }

        /// <summary>
        /// Clears all tokens (access, refresh, and download).
        /// Fires TokensChanged event once after clearing all tokens.
        /// Updates database.
        /// </summary>
        public void ClearTokens()
        {
            _accessToken = string.Empty;
            _refreshToken = string.Empty;
            _downloadToken = string.Empty;
            _currentUsername = string.Empty;
            TokensChanged?.Invoke(this, EventArgs.Empty);

            // Clear in database asynchronously with error handling
            _ = Task.Run(async () =>
            {
                try
                {
                    await _databaseService.ClearActiveCredentialAsync();
                }
                catch
                {
                    // Silently ignore database errors - tokens are still cleared from memory
                }
            });
        }
    }
}
