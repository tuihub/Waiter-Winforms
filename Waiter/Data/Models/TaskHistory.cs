using System.ComponentModel.DataAnnotations;

namespace Waiter.Data.Models
{
    /// <summary>
    /// Stores background task history.
    /// </summary>
    public class TaskHistory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(64)]
        public string TaskId { get; set; } = string.Empty;

        [Required]
        [MaxLength(256)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(32)]
        public string TaskType { get; set; } = string.Empty;

        [MaxLength(32)]
        public string Status { get; set; } = string.Empty;

        [MaxLength(1024)]
        public string? StatusMessage { get; set; }

        public double Progress { get; set; }

        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
