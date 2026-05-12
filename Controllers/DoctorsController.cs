using ClinicApp.Models;
using ClinicApp.Services;
using ClinicApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ClinicApp.Controllers
{
    [Authorize(Roles = "Doctor")]
    public class DoctorsController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IDoctorService _doctorService;
        private readonly UserManager<ApplicationUser> _userManager;

        public DoctorsController(
            IAppointmentService appointmentService,
            IDoctorService doctorService,
            UserManager<ApplicationUser> userManager)
        {
            _appointmentService = appointmentService;
            _doctorService = doctorService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Dashboard()
        {
            var userId = _userManager.GetUserId(User)!;
            var doctorId = await _doctorService.GetDoctorIdByUserIdAsync(userId);
            if (doctorId == null) return RedirectToAction("Index", "Home");

            var user = await _userManager.GetUserAsync(User);
            var today = DateTime.Today;
            var allAppointments = await _appointmentService.GetDoctorAppointmentsAsync(doctorId.Value);
            var todayAppts = allAppointments.Where(a => a.AppointmentDate.Date == today).ToList();

            var vm = new DoctorDashboardViewModel
            {
                DoctorName = user?.FullName ?? "Doctor",
                Specialty = (await _doctorService.GetDoctorSummaryAsync(doctorId.Value))?.Specialty ?? "",
                TodayAppointments = todayAppts,
                UpcomingAppointments = allAppointments
                    .Where(a => a.AppointmentDate.Date > today && a.Status == AppointmentStatus.Confirmed)
                    .Take(5).ToList(),
                TotalPatients = allAppointments.Select(a => a.PatientName).Distinct().Count(),
                CompletedToday = todayAppts.Count(a => a.Status == AppointmentStatus.Completed),
                PendingToday = todayAppts.Count(a => a.Status == AppointmentStatus.Confirmed || a.Status == AppointmentStatus.Pending)
            };

            return View(vm);
        }

        public async Task<IActionResult> Schedule(DateTime? date)
        {
            var userId = _userManager.GetUserId(User)!;
            var doctorId = await _doctorService.GetDoctorIdByUserIdAsync(userId);
            if (doctorId == null) return RedirectToAction("Index", "Home");

            var selectedDate = date ?? DateTime.Today;
            var appointments = await _appointmentService.GetDoctorAppointmentsAsync(doctorId.Value, selectedDate);

            var vm = new DoctorScheduleViewModel
            {
                DoctorId = doctorId.Value,
                Appointments = appointments,
                SelectedDate = selectedDate
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> AppointmentDetails(int id)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
            if (appointment == null) return NotFound();
            return View(appointment);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateNotes(UpdateAppointmentNotesViewModel model)
        {
            var success = await _appointmentService.UpdateAppointmentAsync(model);
            TempData[success ? "Success" : "Error"] = success
                ? "Appointment updated successfully."
                : "Could not update appointment.";
            return RedirectToAction("Schedule");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkComplete(int id)
        {
            var model = new UpdateAppointmentNotesViewModel
            {
                AppointmentId = id,
                Status = AppointmentStatus.Completed
            };
            await _appointmentService.UpdateAppointmentAsync(model);
            TempData["Success"] = "Appointment marked as completed.";
            return RedirectToAction("Schedule");
        }
    }
}
