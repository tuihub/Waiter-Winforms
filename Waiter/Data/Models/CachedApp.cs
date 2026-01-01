using System.ComponentModel.DataAnnotations;

namespace Waiter.Data.Models
{
    /// <summary>
    /// Cached app information for offline viewing.
    /// </summary>
    public class CachedApp
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(4096)]
        public string? Description { get; set; }

        [MaxLength(128)]
        public string? Developer { get; set; }

        [MaxLength(128)]
        public string? Publisher { get; set; }

        public int AppType { get; set; }

        /// <summary>
        /// Server URL this cache belongs to.
        /// </summary>
        [Required]
        [MaxLength(512)]
        public string ServerUrl { get; set; } = string.Empty;

        public DateTime CachedAt { get; set; } = DateTime.UtcNow;
    }
}
