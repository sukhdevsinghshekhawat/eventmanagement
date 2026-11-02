
using System.ComponentModel.DataAnnotations;

namespace MyMvcApp.Models
{
    public class Event
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Venue { get; set; } = string.Empty;

        [Required]
        public DateTime EventDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? CreatedByUserId { get; set; }

        public ICollection<EventDepartment> TargetDepartments { get; set; } = new List<EventDepartment>();

        public ICollection<EventApplication> Applications { get; set; } = new List<EventApplication>();
    }
}
