using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Waiter.Data.Models;

namespace Waiter.Data.Configurations
{
    /// <summary>
    /// Entity Framework Core configuration for CachedUpload.
    /// </summary>
    public class CachedUploadConfiguration : IEntityTypeConfiguration<CachedUpload>
    {
        public void Configure(EntityTypeBuilder<CachedUpload> builder)
        {
            builder.ToTable("CachedUploads");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.FilePath)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.Metadata)
                .IsRequired();

            builder.Property(x => x.LastError)
                .HasMaxLength(1000);

            // Indexes for common queries
            builder.HasIndex(x => x.RuntimeSessionId);
            builder.HasIndex(x => x.ExpiresAt);
        }
    }
}
