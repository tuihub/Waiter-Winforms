using System;
using System.Text.Json;

namespace Waiter.Services
{
    /// <summary>
    /// Manages application configuration including server URL.
    /// </summary>
    public class ConfigService
    {
        private const string ConfigFileName = "waiter_config.json";
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
                    SaveConfig();
                    ConfigChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public ConfigService()
        {
            LoadConfig();
        }

        private string GetConfigPath()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var waiterPath = Path.Combine(appDataPath, "TuiHub", "Waiter");
            Directory.CreateDirectory(waiterPath);
            return Path.Combine(waiterPath, ConfigFileName);
        }

        private void LoadConfig()
        {
            try
            {
                var configPath = GetConfigPath();
                if (File.Exists(configPath))
                {
                    var json = File.ReadAllText(configPath);
                    var config = JsonSerializer.Deserialize<ConfigData>(json);
                    if (config != null && !string.IsNullOrWhiteSpace(config.ServerUrl))
                    {
                        _serverUrl = config.ServerUrl;
                    }
                }
            }
            catch
            {
                // Use default config if loading fails
            }
        }

        private void SaveConfig()
        {
            try
            {
                var configPath = GetConfigPath();
                var config = new ConfigData { ServerUrl = _serverUrl };
                var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(configPath, json);
            }
            catch
            {
                // Ignore save errors
            }
        }

        private class ConfigData
        {
            public string ServerUrl { get; set; } = string.Empty;
        }
    }
}
