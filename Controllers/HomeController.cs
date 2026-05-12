using ClinicApp.Data;
using ClinicApp.Services;
using ClinicApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IDoctorService _doctorService;

        public HomeController(ApplicationDbContext context, IDoctorService doctorService)
        {
            _context = context;
            _doctorService = doctorService;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new HomeViewModel
            {
                Specialties = await _context.Specialties.ToListAsync(),
                FeaturedDoctors = (await _doctorService.GetAllDoctorsAsync()).Take(4).ToList(),
                TotalPatients = await _context.Patients.CountAsync(),
                TotalDoctors = await _context.Doctors.CountAsync(),
                TotalAppointments = await _context.Appointments.CountAsync()
            };
            return View(vm);
        }

        public async Task<IActionResult> Doctors(int? specialtyId, string? search)
        {
            var doctors = specialtyId.HasValue
                ? await _doctorService.GetDoctorsBySpecialtyAsync(specialtyId.Value)
                : await _doctorService.GetAllDoctorsAsync();

            if (!string.IsNullOrWhiteSpace(search))
                doctors = doctors.Where(d => d.FullName.Contains(search, StringComparison.OrdinalIgnoreCase)
                    || d.Specialty.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

            ViewBag.Specialties = await _context.Specialties.ToListAsync();
            ViewBag.SelectedSpecialty = specialtyId;
            ViewBag.Search = search;
            return View(doctors);
        }

        public IActionResult Privacy() => View();
    }
}
