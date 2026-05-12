using System.ComponentModel.DataAnnotations;
using ClinicApp.Models;

namespace ClinicApp.ViewModels
{
    // ─── Account ───────────────────────────────────────────────────────────────

    public class LoginViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required, MaxLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, DataType(DataType.Password), MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password), Compare("Password")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        public string Gender { get; set; } = string.Empty;

        [Required, DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        public string? PhoneNumber { get; set; }
    }

    // ─── Appointment ────────────────────────────────────────────────────────────

    public class BookAppointmentViewModel
    {
        [Required]
        [Display(Name = "Doctor")]
        public int DoctorId { get; set; }

        [Required]
        [Display(Name = "Appointment Date")]
        [DataType(DataType.Date)]
        public DateTime AppointmentDate { get; set; } = DateTime.Today.AddDays(1);

        [Required]
        [Display(Name = "Time Slot")]
        public string SelectedTimeSlot { get; set; } = string.Empty;

        [MaxLength(500)]
        [Display(Name = "Reason for Visit")]
        public string? ReasonForVisit { get; set; }

        // For view population
        public List<DoctorSummaryViewModel> AvailableDoctors { get; set; } = new();
        public List<string> AvailableTimeSlots { get; set; } = new();
        public DoctorSummaryViewModel? SelectedDoctor { get; set; }
    }

    public class AppointmentDetailsViewModel
    {
        public int Id { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public AppointmentStatus Status { get; set; }
        public string? ReasonForVisit { get; set; }
        public string? DoctorNotes { get; set; }
        public decimal ConsultationFee { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AppointmentListViewModel
    {
        public List<AppointmentDetailsViewModel> Upcoming { get; set; } = new();
        public List<AppointmentDetailsViewModel> Past { get; set; } = new();
    }

    public class UpdateAppointmentNotesViewModel
    {
        public int AppointmentId { get; set; }
        public string? DoctorNotes { get; set; }
        public AppointmentStatus Status { get; set; }
    }

    // ─── Doctor ─────────────────────────────────────────────────────────────────

    public class DoctorSummaryViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;
        public string? SpecialtyIcon { get; set; }
        public int YearsOfExperience { get; set; }
        public decimal ConsultationFee { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? Bio { get; set; }
        public int TotalAppointments { get; set; }
    }

    public class DoctorDashboardViewModel
    {
        public string DoctorName { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;
        public List<AppointmentDetailsViewModel> TodayAppointments { get; set; } = new();
        public List<AppointmentDetailsViewModel> UpcomingAppointments { get; set; } = new();
        public int TotalPatients { get; set; }
        public int CompletedToday { get; set; }
        public int PendingToday { get; set; }
    }

    public class DoctorScheduleViewModel
    {
        public int DoctorId { get; set; }
        public List<AppointmentDetailsViewModel> Appointments { get; set; } = new();
        public DateTime SelectedDate { get; set; } = DateTime.Today;
    }

    // ─── Admin ──────────────────────────────────────────────────────────────────

    public class AdminDashboardViewModel
    {
        public int TotalPatients { get; set; }
        public int TotalDoctors { get; set; }
        public int TodayAppointments { get; set; }
        public int PendingAppointments { get; set; }
        public int CompletedThisMonth { get; set; }
        public List<DoctorSummaryViewModel> Doctors { get; set; } = new();
        public List<AppointmentDetailsViewModel> RecentAppointments { get; set; } = new();
    }

    public class CreateDoctorViewModel
    {
        [Required, MaxLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Gender { get; set; } = string.Empty;

        [Required, DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [Display(Name = "Specialty")]
        public int SpecialtyId { get; set; }

        [Required]
        [Display(Name = "License Number")]
        public string LicenseNumber { get; set; } = string.Empty;

        [Display(Name = "Years of Experience")]
        public int YearsOfExperience { get; set; }

        [Display(Name = "Consultation Fee ($)")]
        public decimal ConsultationFee { get; set; }

        public string? Bio { get; set; }

        public List<Specialty> Specialties { get; set; } = new();
    }

    // ─── Home ───────────────────────────────────────────────────────────────────

    public class HomeViewModel
    {
        public List<Specialty> Specialties { get; set; } = new();
        public List<DoctorSummaryViewModel> FeaturedDoctors { get; set; } = new();
        public int TotalPatients { get; set; }
        public int TotalDoctors { get; set; }
        public int TotalAppointments { get; set; }
    }
}
