using System.ComponentModel.DataAnnotations;

namespace Waiter.Data.Models
{
    /// <summary>
    /// Cached app category information.
    /// </summary>
    public class CachedAppCategory
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Server URL this cache belongs to.
        /// </summary>
        [Required]
        [MaxLength(512)]
        public string ServerUrl { get; set; } = string.Empty;

        public DateTime CachedAt { get; set; } = DateTime.UtcNow;
    }
}
