using ClinicApp.Data;
using ClinicApp.Models;
using ClinicApp.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ClinicApp.Services
{
    public interface IAppointmentService
    {
        Task<List<AppointmentDetailsViewModel>> GetPatientAppointmentsAsync(string userId);
        Task<List<AppointmentDetailsViewModel>> GetDoctorAppointmentsAsync(int doctorId, DateTime? date = null);
        Task<List<AppointmentDetailsViewModel>> GetAllAppointmentsAsync();
        Task<AppointmentDetailsViewModel?> GetAppointmentByIdAsync(int id);
        Task<List<string>> GetAvailableTimeSlotsAsync(int doctorId, DateTime date);
        Task<(bool Success, string Message)> BookAppointmentAsync(int patientId, BookAppointmentViewModel model);
        Task<bool> CancelAppointmentAsync(int appointmentId, string reason);
        Task<bool> UpdateAppointmentAsync(UpdateAppointmentNotesViewModel model);
        Task<bool> ConfirmAppointmentAsync(int appointmentId);
    }

    public class AppointmentService : IAppointmentService
    {
        private readonly ApplicationDbContext _context;

        public AppointmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<AppointmentDetailsViewModel>> GetPatientAppointmentsAsync(string userId)
        {
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient == null) return new();

            return await _context.Appointments
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .Include(a => a.Doctor).ThenInclude(d => d.Specialty)
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .Where(a => a.PatientId == patient.Id)
                .OrderByDescending(a => a.AppointmentDate)
                .Select(a => MapToViewModel(a))
                .ToListAsync();
        }

        public async Task<List<AppointmentDetailsViewModel>> GetDoctorAppointmentsAsync(int doctorId, DateTime? date = null)
        {
            var query = _context.Appointments
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .Include(a => a.Doctor).ThenInclude(d => d.Specialty)
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .Where(a => a.DoctorId == doctorId);

            if (date.HasValue)
                query = query.Where(a => a.AppointmentDate.Date == date.Value.Date);

            return await query.OrderBy(a => a.AppointmentDate).ThenBy(a => a.StartTime)
                .Select(a => MapToViewModel(a))
                .ToListAsync();
        }

        public async Task<List<AppointmentDetailsViewModel>> GetAllAppointmentsAsync()
        {
            return await _context.Appointments
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .Include(a => a.Doctor).ThenInclude(d => d.Specialty)
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .OrderByDescending(a => a.AppointmentDate)
                .Select(a => MapToViewModel(a))
                .ToListAsync();
        }

        public async Task<AppointmentDetailsViewModel?> GetAppointmentByIdAsync(int id)
        {
            var a = await _context.Appointments
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .Include(a => a.Doctor).ThenInclude(d => d.Specialty)
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .FirstOrDefaultAsync(a => a.Id == id);

            return a == null ? null : MapToViewModel(a);
        }

        public async Task<List<string>> GetAvailableTimeSlotsAsync(int doctorId, DateTime date)
        {
            var doctor = await _context.Doctors.FindAsync(doctorId);
            if (doctor == null) return new();

            // Check if doctor works on that day
            var dayName = date.DayOfWeek.ToString()[..3]; // Mon, Tue, etc.
            if (doctor.WorkingDays != null && !doctor.WorkingDays.Contains(dayName))
                return new();

            // Generate all slots
            var slots = new List<string>();
            var current = doctor.WorkStartTime;
            while (current < doctor.WorkEndTime)
            {
                slots.Add(current.ToString(@"hh\:mm"));
                current = current.Add(TimeSpan.FromMinutes(doctor.SlotDurationMinutes));
            }

            // Remove booked slots
            var booked = await _context.Appointments
                .Where(a => a.DoctorId == doctorId
                    && a.AppointmentDate.Date == date.Date
                    && a.Status != AppointmentStatus.Cancelled)
                .Select(a => a.StartTime)
                .ToListAsync();

            slots = slots.Where(s => !booked.Any(b => b.ToString(@"hh\:mm") == s)).ToList();
            return slots;
        }

        public async Task<(bool Success, string Message)> BookAppointmentAsync(int patientId, BookAppointmentViewModel model)
        {
            var doctor = await _context.Doctors.FindAsync(model.DoctorId);
            if (doctor == null) return (false, "Doctor not found.");

            if (!TimeSpan.TryParse(model.SelectedTimeSlot, out var startTime))
                return (false, "Invalid time slot.");

            var endTime = startTime.Add(TimeSpan.FromMinutes(doctor.SlotDurationMinutes));

            // Double-check slot availability
            var conflict = await _context.Appointments.AnyAsync(a =>
                a.DoctorId == model.DoctorId
                && a.AppointmentDate.Date == model.AppointmentDate.Date
                && a.StartTime == startTime
                && a.Status != AppointmentStatus.Cancelled);

            if (conflict) return (false, "This slot is no longer available. Please choose another.");

            var appointment = new Appointment
            {
                PatientId = patientId,
                DoctorId = model.DoctorId,
                AppointmentDate = model.AppointmentDate.Date,
                StartTime = startTime,
                EndTime = endTime,
                ReasonForVisit = model.ReasonForVisit,
                Status = AppointmentStatus.Confirmed
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            return (true, "Appointment booked successfully!");
        }

        public async Task<bool> CancelAppointmentAsync(int appointmentId, string reason)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment == null) return false;

            appointment.Status = AppointmentStatus.Cancelled;
            appointment.CancellationReason = reason;
            appointment.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAppointmentAsync(UpdateAppointmentNotesViewModel model)
        {
            var appointment = await _context.Appointments.FindAsync(model.AppointmentId);
            if (appointment == null) return false;

            appointment.DoctorNotes = model.DoctorNotes;
            appointment.Status = model.Status;
            appointment.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ConfirmAppointmentAsync(int appointmentId)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment == null) return false;

            appointment.Status = AppointmentStatus.Confirmed;
            appointment.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        private static AppointmentDetailsViewModel MapToViewModel(Appointment a) => new()
        {
            Id = a.Id,
            PatientName = a.Patient?.User?.FullName ?? "Unknown",
            DoctorName = $"Dr. {a.Doctor?.User?.FullName ?? "Unknown"}",
            Specialty = a.Doctor?.Specialty?.Name ?? "",
            AppointmentDate = a.AppointmentDate,
            StartTime = a.StartTime,
            EndTime = a.EndTime,
            Status = a.Status,
            ReasonForVisit = a.ReasonForVisit,
            DoctorNotes = a.DoctorNotes,
            ConsultationFee = a.Doctor?.ConsultationFee ?? 0,
            CreatedAt = a.CreatedAt
        };
    }
}
