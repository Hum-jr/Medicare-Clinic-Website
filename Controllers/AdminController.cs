using ClinicApp.Data;
using ClinicApp.Models;
using ClinicApp.Services;
using ClinicApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAppointmentService _appointmentService;
        private readonly IDoctorService _doctorService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(
            ApplicationDbContext context,
            IAppointmentService appointmentService,
            IDoctorService doctorService,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _appointmentService = appointmentService;
            _doctorService = doctorService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var now = DateTime.Now;
            var allAppointments = await _appointmentService.GetAllAppointmentsAsync();

            var vm = new AdminDashboardViewModel
            {
                TotalPatients = await _context.Patients.CountAsync(),
                TotalDoctors = await _context.Doctors.CountAsync(),
                TodayAppointments = allAppointments.Count(a => a.AppointmentDate.Date == now.Date),
                PendingAppointments = allAppointments.Count(a => a.Status == AppointmentStatus.Pending || a.Status == AppointmentStatus.Confirmed),
                CompletedThisMonth = allAppointments.Count(a =>
                    a.Status == AppointmentStatus.Completed &&
                    a.AppointmentDate.Month == now.Month &&
                    a.AppointmentDate.Year == now.Year),
                Doctors = await _doctorService.GetAllDoctorsAsync(),
                RecentAppointments = allAppointments.Take(10).ToList()
            };

            return View(vm);
        }

        public async Task<IActionResult> Appointments(string? status)
        {
            var appointments = await _appointmentService.GetAllAppointmentsAsync();

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<AppointmentStatus>(status, out var s))
                appointments = appointments.Where(a => a.Status == s).ToList();

            ViewBag.CurrentStatus = status;
            return View(appointments);
        }

        public async Task<IActionResult> Doctors()
        {
            var doctors = await _doctorService.GetAllDoctorsAsync();
            return View(doctors);
        }

        [HttpGet]
        public async Task<IActionResult> CreateDoctor()
        {
            var vm = new CreateDoctorViewModel
            {
                Specialties = await _context.Specialties.ToListAsync()
            };
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDoctor(CreateDoctorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Specialties = await _context.Specialties.ToListAsync();
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Gender = model.Gender,
                DateOfBirth = model.DateOfBirth,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, "Doctor123!");
            if (!result.Succeeded)
            {
                foreach (var e in result.Errors)
                    ModelState.AddModelError(string.Empty, e.Description);
                model.Specialties = await _context.Specialties.ToListAsync();
                return View(model);
            }

            await _userManager.AddToRoleAsync(user, "Doctor");

            var doctor = new Doctor
            {
                UserId = user.Id,
                SpecialtyId = model.SpecialtyId,
                LicenseNumber = model.LicenseNumber,
                YearsOfExperience = model.YearsOfExperience,
                ConsultationFee = model.ConsultationFee,
                Bio = model.Bio,
                WorkingDays = "Mon,Tue,Wed,Thu,Fri",
                WorkStartTime = new TimeSpan(9, 0, 0),
                WorkEndTime = new TimeSpan(17, 0, 0),
                SlotDurationMinutes = 30
            };

            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Dr. {model.FirstName} {model.LastName} added successfully. Default password: Doctor123!";
            return RedirectToAction("Doctors");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleDoctorStatus(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor != null)
            {
                doctor.IsActive = !doctor.IsActive;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Doctors");
        }

        public async Task<IActionResult> Patients()
        {
            var patients = await _context.Patients
                .Include(p => p.User)
                .Include(p => p.Appointments)
                .Select(p => new
                {
                    p.Id,
                    Name = p.User.FirstName + " " + p.User.LastName,
                    p.User.Email,
                    p.User.PhoneNumber,
                    p.BloodType,
                    AppointmentCount = p.Appointments.Count,
                    p.User.CreatedAt
                })
                .ToListAsync();

            return View(patients);
        }
    }
}
