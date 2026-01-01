using System;

namespace Waiter.Services
{
    /// <summary>
    /// Manages token state for authentication with the server.
    /// Supports AccessToken, RefreshToken, and DownloadToken.
    /// </summary>
    public class TokenService
    {
        private string _accessToken = string.Empty;
        private string _refreshToken = string.Empty;
        private string _downloadToken = string.Empty;

        public event EventHandler? TokensChanged;

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

        public bool IsLoggedIn => !string.IsNullOrWhiteSpace(AccessToken);

        public void SetTokens(string accessToken, string refreshToken)
        {
            _accessToken = accessToken;
            _refreshToken = refreshToken;
            TokensChanged?.Invoke(this, EventArgs.Empty);
        }

        public void SetDownloadToken(string downloadToken)
        {
            DownloadToken = downloadToken;
        }

        public void ClearTokens()
        {
            _accessToken = string.Empty;
            _refreshToken = string.Empty;
            _downloadToken = string.Empty;
            TokensChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
