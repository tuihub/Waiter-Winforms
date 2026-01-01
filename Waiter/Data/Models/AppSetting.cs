using System.ComponentModel.DataAnnotations;

namespace Waiter.Data.Models
{
    /// <summary>
    /// Stores application settings as key-value pairs.
    /// </summary>
    public class AppSetting
    {
        [Key]
        [MaxLength(128)]
        public string Key { get; set; } = string.Empty;

        [MaxLength(4096)]
        public string? Value { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
