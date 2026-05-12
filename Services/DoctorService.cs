using ClinicApp.Data;
using ClinicApp.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ClinicApp.Services
{
    public interface IDoctorService
    {
        Task<List<DoctorSummaryViewModel>> GetAllDoctorsAsync();
        Task<List<DoctorSummaryViewModel>> GetDoctorsBySpecialtyAsync(int specialtyId);
        Task<DoctorSummaryViewModel?> GetDoctorSummaryAsync(int doctorId);
        Task<int?> GetDoctorIdByUserIdAsync(string userId);
        Task<int?> GetPatientIdByUserIdAsync(string userId);
    }

    public class DoctorService : IDoctorService
    {
        private readonly ApplicationDbContext _context;

        public DoctorService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<DoctorSummaryViewModel>> GetAllDoctorsAsync()
        {
            return await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.Specialty)
                .Include(d => d.Appointments)
                .Where(d => d.IsActive)
                .Select(d => new DoctorSummaryViewModel
                {
                    Id = d.Id,
                    FullName = $"Dr. {d.User.FirstName} {d.User.LastName}",
                    Specialty = d.Specialty.Name,
                    SpecialtyIcon = d.Specialty.IconClass,
                    YearsOfExperience = d.YearsOfExperience,
                    ConsultationFee = d.ConsultationFee,
                    ProfileImageUrl = d.ProfileImageUrl,
                    Bio = d.Bio,
                    TotalAppointments = d.Appointments.Count
                })
                .ToListAsync();
        }

        public async Task<List<DoctorSummaryViewModel>> GetDoctorsBySpecialtyAsync(int specialtyId)
        {
            return await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.Specialty)
                .Where(d => d.SpecialtyId == specialtyId && d.IsActive)
                .Select(d => new DoctorSummaryViewModel
                {
                    Id = d.Id,
                    FullName = $"Dr. {d.User.FirstName} {d.User.LastName}",
                    Specialty = d.Specialty.Name,
                    SpecialtyIcon = d.Specialty.IconClass,
                    YearsOfExperience = d.YearsOfExperience,
                    ConsultationFee = d.ConsultationFee,
                    Bio = d.Bio
                })
                .ToListAsync();
        }

        public async Task<DoctorSummaryViewModel?> GetDoctorSummaryAsync(int doctorId)
        {
            return await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.Specialty)
                .Where(d => d.Id == doctorId)
                .Select(d => new DoctorSummaryViewModel
                {
                    Id = d.Id,
                    FullName = $"Dr. {d.User.FirstName} {d.User.LastName}",
                    Specialty = d.Specialty.Name,
                    SpecialtyIcon = d.Specialty.IconClass,
                    YearsOfExperience = d.YearsOfExperience,
                    ConsultationFee = d.ConsultationFee,
                    Bio = d.Bio
                })
                .FirstOrDefaultAsync();
        }

        public async Task<int?> GetDoctorIdByUserIdAsync(string userId)
        {
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);
            return doctor?.Id;
        }

        public async Task<int?> GetPatientIdByUserIdAsync(string userId)
        {
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            return patient?.Id;
        }
    }
}
