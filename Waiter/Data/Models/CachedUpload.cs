using System.ComponentModel.DataAnnotations;

namespace Waiter.Data.Models
{
    /// <summary>
    /// Stores failed upload data locally for manual retry.
    /// </summary>
    public class CachedUpload
    {
        /// <summary>
        /// Primary key.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Foreign key to RuntimeSession. Nullable for future upload types.
        /// </summary>
        public long? RuntimeSessionId { get; set; }

        /// <summary>
        /// Type of upload (RuntimeData or SaveFile).
        /// </summary>
        public UploadType UploadType { get; set; }

        /// <summary>
        /// Absolute path to cached file in cache directory.
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// JSON-serialized metadata (upload request details).
        /// </summary>
        [Required]
        public string Metadata { get; set; } = string.Empty;

        /// <summary>
        /// UTC timestamp when the cache record was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Number of retry attempts. Range: 0-10.
        /// </summary>
        [Range(0, 10)]
        public int RetryCount { get; set; } = 0;

        /// <summary>
        /// Last error message from failed upload attempt.
        /// </summary>
        [MaxLength(1000)]
        public string? LastError { get; set; }

        /// <summary>
        /// UTC timestamp for auto-cleanup threshold. Default: 30 days from creation.
        /// </summary>
        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddDays(30);
    }
}
