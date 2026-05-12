using ClinicApp.Services;
using ClinicApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ClinicApp.Models;

namespace ClinicApp.Controllers
{
    [Authorize(Roles = "Patient")]
    public class AppointmentsController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IDoctorService _doctorService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AppointmentsController(
            IAppointmentService appointmentService,
            IDoctorService doctorService,
            UserManager<ApplicationUser> userManager)
        {
            _appointmentService = appointmentService;
            _doctorService = doctorService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User)!;
            var appointments = await _appointmentService.GetPatientAppointmentsAsync(userId);
            var now = DateTime.Now;

            var vm = new AppointmentListViewModel
            {
                Upcoming = appointments.Where(a => a.AppointmentDate >= now.Date
                    && a.Status != AppointmentStatus.Cancelled
                    && a.Status != AppointmentStatus.Completed).ToList(),
                Past = appointments.Where(a => a.AppointmentDate < now.Date
                    || a.Status == AppointmentStatus.Completed
                    || a.Status == AppointmentStatus.Cancelled).ToList()
            };
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Book(int? doctorId)
        {
            var vm = new BookAppointmentViewModel
            {
                AvailableDoctors = await _doctorService.GetAllDoctorsAsync(),
                DoctorId = doctorId ?? 0,
                AppointmentDate = DateTime.Today.AddDays(1)
            };

            if (doctorId.HasValue)
            {
                vm.SelectedDoctor = await _doctorService.GetDoctorSummaryAsync(doctorId.Value);
                vm.AvailableTimeSlots = await _appointmentService.GetAvailableTimeSlotsAsync(doctorId.Value, vm.AppointmentDate);
            }

            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(BookAppointmentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AvailableDoctors = await _doctorService.GetAllDoctorsAsync();
                model.AvailableTimeSlots = await _appointmentService.GetAvailableTimeSlotsAsync(model.DoctorId, model.AppointmentDate);
                return View(model);
            }

            var userId = _userManager.GetUserId(User)!;
            var patientId = await _doctorService.GetPatientIdByUserIdAsync(userId);
            if (patientId == null)
            {
                TempData["Error"] = "Patient profile not found.";
                return RedirectToAction("Index");
            }

            var (success, message) = await _appointmentService.BookAppointmentAsync(patientId.Value, model);
            if (success)
            {
                TempData["Success"] = message;
                return RedirectToAction("Index");
            }

            TempData["Error"] = message;
            model.AvailableDoctors = await _doctorService.GetAllDoctorsAsync();
            model.AvailableTimeSlots = await _appointmentService.GetAvailableTimeSlotsAsync(model.DoctorId, model.AppointmentDate);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
            if (appointment == null) return NotFound();
            return View(appointment);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id, string reason)
        {
            var success = await _appointmentService.CancelAppointmentAsync(id, reason);
            TempData[success ? "Success" : "Error"] = success ? "Appointment cancelled." : "Could not cancel appointment.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<JsonResult> GetTimeSlots(int doctorId, string date)
        {
            if (!DateTime.TryParse(date, out var parsedDate))
                return Json(new List<string>());

            var slots = await _appointmentService.GetAvailableTimeSlotsAsync(doctorId, parsedDate);
            return Json(slots);
        }
    }
}
