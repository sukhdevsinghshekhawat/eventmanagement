using System.ComponentModel.DataAnnotations;

namespace MyMvcApp.Models
{
    public class EventDepartment
    {
        public int EventId { get; set; }

        public Event Event { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Department { get; set; } = string.Empty;
    }
}