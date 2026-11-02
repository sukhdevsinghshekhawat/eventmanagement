using System.ComponentModel.DataAnnotations;

namespace MyMvcApp.Models
{
    public enum ApplicationStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2
    }

    public class EventApplication
    {
        public int Id { get; set; }

        [Required]
        public int EventId { get; set; }

        public Event Event { get; set; } = null!;

        [Required]
        public string ApplicantUserId { get; set; } = string.Empty;

        public ApplicationUser? Applicant { get; set; }

        [Required]
        [StringLength(150)]
        public string ApplicantName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(256)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Department { get; set; } = string.Empty;

        public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        public string? ReviewedByUserId { get; set; }

        public DateTime? ReviewedAt { get; set; }

        [StringLength(500)]
        public string? ReviewerNotes { get; set; }
    }
}