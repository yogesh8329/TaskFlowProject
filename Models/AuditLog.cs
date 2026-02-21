using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Api.Models
{
    public enum AuditAction
    {
        Create = 1,
        Update = 2,
        Delete = 3,
        LoginSuccess = 4,
        LoginFailed = 5
    }

    public class AuditLog
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        [Required]
        public AuditAction Action { get; set; }

        [Required]
        [MaxLength(100)]
        public string EntityName { get; set; } = null!;

        [Required]
        public int EntityId { get; set; }

        public string? OldValues { get; set; }

        public string? NewValues { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}