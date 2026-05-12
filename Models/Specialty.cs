using System.ComponentModel.DataAnnotations;

namespace ClinicApp.Models
{
    public class Specialty
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
        public string? IconClass { get; set; } // FontAwesome icon class

        // Navigation
        public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
    }
}
