using Microsoft.AspNetCore.Identity;

namespace MyMvcApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }

        // Primary department for Teacher/Student users.
        public string? Department { get; set; }

        public bool IsActive { get; set; } = true;
    }
}