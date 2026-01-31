using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Waiter.Data.Models;

namespace Waiter.Data.Configurations
{
    /// <summary>
    /// Entity Framework Core configuration for AppPackageLaunchSettings.
    /// </summary>
    public class AppPackageLaunchSettingsConfiguration : IEntityTypeConfiguration<AppPackageLaunchSettings>
    {
        public void Configure(EntityTypeBuilder<AppPackageLaunchSettings> builder)
        {
            builder.ToTable("AppPackageLaunchSettings");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ExecutablePath)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.WorkingDirectory)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.ProcessName)
                .HasMaxLength(100);

            builder.Property(x => x.SaveDataPath)
                .HasMaxLength(500);

            // Unique index on AppPackageId (one-to-one relationship)
            builder.HasIndex(x => x.AppPackageId)
                .IsUnique();
        }
    }
}
