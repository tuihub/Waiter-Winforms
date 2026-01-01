using System;
using Waiter.Data;

namespace Waiter.Services
{
    /// <summary>
    /// Manages application configuration including server URL.
    /// Persists configuration to SQLite database.
    /// </summary>
    public class ConfigService
    {
        private const string ServerUrlKey = "ServerUrl";
        private readonly DatabaseService _databaseService;
        private string _serverUrl = "https://localhost:5001";

        public event EventHandler? ConfigChanged;

        public string ServerUrl
        {
            get => _serverUrl;
            set
            {
                if (_serverUrl != value)
                {
                    _serverUrl = value;
                    SaveConfigAsync().GetAwaiter().GetResult();
                    ConfigChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public ConfigService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            LoadConfigAsync().GetAwaiter().GetResult();
        }

        private async Task LoadConfigAsync()
        {
            try
            {
                var serverUrl = await _databaseService.GetSettingAsync(ServerUrlKey);
                if (!string.IsNullOrWhiteSpace(serverUrl))
                {
                    _serverUrl = serverUrl;
                }
            }
            catch
            {
                // Use default config if loading fails
            }
        }

        private async Task SaveConfigAsync()
        {
            try
            {
                await _databaseService.SetSettingAsync(ServerUrlKey, _serverUrl);
            }
            catch
            {
                // Ignore save errors
            }
        }
    }
}
