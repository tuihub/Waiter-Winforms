using System.ComponentModel.DataAnnotations;

namespace Waiter.Data.Models
{
    /// <summary>
    /// Stores user authentication credentials and tokens.
    /// </summary>
    public class UserCredential
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// JWT Access Token for API authentication.
        /// </summary>
        [MaxLength(4096)]
        public string? AccessToken { get; set; }

        /// <summary>
        /// JWT Refresh Token for obtaining new access tokens.
        /// </summary>
        [MaxLength(4096)]
        public string? RefreshToken { get; set; }

        /// <summary>
        /// Server URL this credential is associated with.
        /// </summary>
        [Required]
        [MaxLength(512)]
        public string ServerUrl { get; set; } = string.Empty;

        /// <summary>
        /// Last time the tokens were updated.
        /// </summary>
        public DateTime? LastTokenUpdate { get; set; }

        /// <summary>
        /// Whether this is the active/default credential.
        /// </summary>
        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
