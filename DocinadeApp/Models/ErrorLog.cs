using System;
using System.ComponentModel.DataAnnotations;

namespace RubricasApp.Web.Models
{
    public class ErrorLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(500)]
        public string ControllerName { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string ActionName { get; set; } = string.Empty;

        [Required]
        public string ErrorMessage { get; set; } = string.Empty;

        public string? StackTrace { get; set; }

        public string? InnerException { get; set; }

        [MaxLength(500)]
        public string? UserId { get; set; }

        [MaxLength(500)]
        public string? UserName { get; set; }

        [MaxLength(2000)]
        public string? RequestPath { get; set; }

        [MaxLength(50)]
        public string? RequestMethod { get; set; }

        public string? RequestBody { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [MaxLength(100)]
        public string? IpAddress { get; set; }

        [MaxLength(1000)]
        public string? UserAgent { get; set; }
    }
}
