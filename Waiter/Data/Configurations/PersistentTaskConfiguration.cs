using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Waiter.Data.Models;

namespace Waiter.Data.Configurations
{
    /// <summary>
    /// Entity Framework Core configuration for the PersistentTask entity.
    /// </summary>
    public class PersistentTaskConfiguration : IEntityTypeConfiguration<PersistentTask>
    {
        public void Configure(EntityTypeBuilder<PersistentTask> builder)
        {
            builder.ToTable("PersistentTasks");

            builder.HasKey(e => e.Id);

            // Unique constraint on TaskId
            builder.HasIndex(e => e.TaskId)
                .IsUnique();

            // Unique constraint on TaskKey (prevents duplicates)
            builder.HasIndex(e => e.TaskKey)
                .IsUnique();

            // Index for querying by status
            builder.HasIndex(e => e.Status);

            // Index for querying non-completed tasks on startup
            builder.HasIndex(e => new { e.Status, e.CreatedAt });
        }
    }
}
