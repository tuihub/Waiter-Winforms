using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Waiter.Services;
using Waiter.Forms;
using Waiter.Interceptors;
using Waiter.Data;

namespace Waiter;

static class Program
{
    private static IHost? _host;

    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        // Setup dependency injection
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                // Database service
                services.AddSingleton<DatabaseService>();

                // Core services
                services.AddSingleton<ConfigService>();
                services.AddSingleton<TokenService>();
                services.AddSingleton<BackgroundTaskService>();

                // Logging
                services.AddLogging(builder =>
                {
                    builder.AddDebug();
                    builder.SetMinimumLevel(LogLevel.Information);
                });

                // gRPC client service
                services.AddSingleton<LibrarianClientService>();

                // Forms
                services.AddTransient<MainForm>();
                services.AddTransient<LoginForm>();
                services.AddTransient<SettingsForm>();
                services.AddTransient<AppCategoryForm>();
                services.AddTransient<BackgroundTasksForm>();
            })
            .Build();

        // Initialize database
        var dbService = _host.Services.GetRequiredService<DatabaseService>();
        dbService.InitializeAsync().GetAwaiter().GetResult();

        // Get the main form from DI container
        var mainForm = _host.Services.GetRequiredService<MainForm>();

        // Run the application
        Application.Run(mainForm);

        // Cleanup
        _host.Dispose();
    }

    /// <summary>
    /// Gets a service from the DI container.
    /// </summary>
    public static T GetService<T>() where T : class
    {
        if (_host == null)
            throw new InvalidOperationException("Host is not initialized");

        return _host.Services.GetRequiredService<T>();
    }
}