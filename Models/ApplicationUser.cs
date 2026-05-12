using Microsoft.AspNetCore.Identity;

namespace ClinicApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string? Address { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string FullName => $"{FirstName} {LastName}";

        // Navigation
        public Patient? Patient { get; set; }
        public Doctor? Doctor { get; set; }
    }
}
