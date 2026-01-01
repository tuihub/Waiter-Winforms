using Microsoft.EntityFrameworkCore;
using Waiter.Data.Models;

namespace Waiter.Data
{
    /// <summary>
    /// Entity Framework Core database context for the Waiter application.
    /// Uses SQLite as the database backend.
    /// </summary>
    public class WaiterDbContext : DbContext
    {
        public DbSet<UserCredential> UserCredentials { get; set; }
        public DbSet<AppSetting> AppSettings { get; set; }
        public DbSet<CachedApp> CachedApps { get; set; }
        public DbSet<CachedAppCategory> CachedAppCategories { get; set; }
        public DbSet<TaskHistory> TaskHistories { get; set; }

        private readonly string _dbPath;

        public WaiterDbContext()
        {
            // Default path for design-time operations
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var waiterFolder = Path.Combine(folder, "TuiHub", "Waiter");
            Directory.CreateDirectory(waiterFolder);
            _dbPath = Path.Combine(waiterFolder, "waiter.db");
        }

        public WaiterDbContext(string dbPath)
        {
            _dbPath = dbPath;
            var folder = Path.GetDirectoryName(_dbPath);
            if (!string.IsNullOrEmpty(folder))
            {
                Directory.CreateDirectory(folder);
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={_dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // UserCredential configuration
            modelBuilder.Entity<UserCredential>(entity =>
            {
                entity.HasIndex(e => e.Username);
                entity.HasIndex(e => e.ServerUrl);
                entity.HasIndex(e => e.IsActive);
            });

            // AppSetting configuration
            modelBuilder.Entity<AppSetting>(entity =>
            {
                entity.HasKey(e => e.Key);
            });

            // CachedApp configuration
            modelBuilder.Entity<CachedApp>(entity =>
            {
                entity.HasIndex(e => e.ServerUrl);
                entity.HasIndex(e => e.Name);
            });

            // CachedAppCategory configuration
            modelBuilder.Entity<CachedAppCategory>(entity =>
            {
                entity.HasIndex(e => e.ServerUrl);
            });

            // TaskHistory configuration
            modelBuilder.Entity<TaskHistory>(entity =>
            {
                entity.HasIndex(e => e.TaskId);
                entity.HasIndex(e => e.Status);
            });
        }
    }
}
