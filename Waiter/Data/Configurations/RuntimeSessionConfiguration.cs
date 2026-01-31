using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Waiter.Data.Models;

namespace Waiter.Data.Configurations
{
    /// <summary>
    /// Entity Framework Core configuration for RuntimeSession.
    /// </summary>
    public class RuntimeSessionConfiguration : IEntityTypeConfiguration<RuntimeSession>
    {
        public void Configure(EntityTypeBuilder<RuntimeSession> builder)
        {
            builder.ToTable("RuntimeSessions");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.StartTime)
                .IsRequired();

            // Indexes for common queries
            builder.HasIndex(x => x.AppPackageId);
            builder.HasIndex(x => x.DeviceId);
            builder.HasIndex(x => x.Status);

            // Ignore computed properties
            builder.Ignore(x => x.Duration);
            builder.Ignore(x => x.IsRunning);
            builder.Ignore(x => x.IsAbnormalExit);
        }
    }
}
