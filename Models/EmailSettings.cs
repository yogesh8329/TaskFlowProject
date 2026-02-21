using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Api.Models
{
    public class EmailSettings
    {
        [Required]
        public string Host { get; set; } = string.Empty;

        [Range(1, 65535)]
        public int Port { get; set; }

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string From { get; set; } = string.Empty;
    }
}