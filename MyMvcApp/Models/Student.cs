
using System.ComponentModel.DataAnnotations;

namespace MyMvcApp.Models
{
    public class Student
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = "";
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Course is required")]
        public string Course { get; set; } = "";
    }
}
