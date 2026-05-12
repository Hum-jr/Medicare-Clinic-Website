using System.ComponentModel.DataAnnotations;

namespace ClinicApp.Models
{
    public class Doctor
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int SpecialtyId { get; set; }

        public string LicenseNumber { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public int YearsOfExperience { get; set; }
        public decimal ConsultationFee { get; set; }
        public string? ProfileImageUrl { get; set; }

        // Work schedule (bitmask or JSON)
        public string? WorkingDays { get; set; } // e.g., "Mon,Tue,Wed,Thu,Fri"
        public TimeSpan WorkStartTime { get; set; } = new TimeSpan(9, 0, 0);
        public TimeSpan WorkEndTime { get; set; } = new TimeSpan(17, 0, 0);
        public int SlotDurationMinutes { get; set; } = 30;

        public bool IsActive { get; set; } = true;

        // Navigation
        public ApplicationUser User { get; set; } = null!;
        public Specialty Specialty { get; set; } = null!;
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
