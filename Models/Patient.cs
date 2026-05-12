using System.ComponentModel.DataAnnotations;

namespace ClinicApp.Models
{
    public class Patient
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        public string? BloodType { get; set; }
        public string? Allergies { get; set; }
        public string? MedicalHistory { get; set; }
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone { get; set; }

        // Navigation
        public ApplicationUser User { get; set; } = null!;
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
