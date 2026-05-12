using ClinicApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ClinicApp.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            await context.Database.MigrateAsync();

            // Seed Roles
            string[] roles = { "Admin", "Doctor", "Patient" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Seed Specialties
            if (!await context.Specialties.AnyAsync())
            {
                var specialties = new List<Specialty>
                {
                    new() { Name = "General Practice", Description = "Primary healthcare and general medicine", IconClass = "fa-stethoscope" },
                    new() { Name = "Cardiology", Description = "Heart and cardiovascular system", IconClass = "fa-heartbeat" },
                    new() { Name = "Dermatology", Description = "Skin, hair, and nails", IconClass = "fa-hand-dots" },
                    new() { Name = "Neurology", Description = "Brain and nervous system", IconClass = "fa-brain" },
                    new() { Name = "Orthopedics", Description = "Bones, joints, and muscles", IconClass = "fa-bone" },
                    new() { Name = "Pediatrics", Description = "Children's health", IconClass = "fa-child" },
                };
                context.Specialties.AddRange(specialties);
                await context.SaveChangesAsync();
            }

            // Seed Admin
            if (await userManager.FindByEmailAsync("admin@clinic.com") == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = "admin@clinic.com",
                    Email = "admin@clinic.com",
                    FirstName = "System",
                    LastName = "Admin",
                    EmailConfirmed = true,
                    Gender = "Other",
                    DateOfBirth = new DateTime(1985, 1, 1)
                };
                await userManager.CreateAsync(admin, "Admin123!");
                await userManager.AddToRoleAsync(admin, "Admin");
            }

            // Seed Doctors
            var doctorSeeds = new[]
            {
                new { Email = "dr.smith@clinic.com", First = "James", Last = "Smith", SpecialtyId = 1, Fee = 80m, Exp = 12 },
                new { Email = "dr.johnson@clinic.com", First = "Sarah", Last = "Johnson", SpecialtyId = 2, Fee = 150m, Exp = 18 },
                new { Email = "dr.patel@clinic.com", First = "Priya", Last = "Patel", SpecialtyId = 3, Fee = 120m, Exp = 8 },
                new { Email = "dr.garcia@clinic.com", First = "Carlos", Last = "Garcia", SpecialtyId = 4, Fee = 200m, Exp = 22 },
            };

            foreach (var ds in doctorSeeds)
            {
                if (await userManager.FindByEmailAsync(ds.Email) == null)
                {
                    var user = new ApplicationUser
                    {
                        UserName = ds.Email,
                        Email = ds.Email,
                        FirstName = ds.First,
                        LastName = ds.Last,
                        EmailConfirmed = true,
                        Gender = "Other",
                        DateOfBirth = new DateTime(1975, 6, 15)
                    };
                    await userManager.CreateAsync(user, "Doctor123!");
                    await userManager.AddToRoleAsync(user, "Doctor");

                    var doctor = new Doctor
                    {
                        UserId = user.Id,
                        SpecialtyId = ds.SpecialtyId,
                        LicenseNumber = $"LIC-{Random.Shared.Next(10000, 99999)}",
                        YearsOfExperience = ds.Exp,
                        ConsultationFee = ds.Fee,
                        WorkingDays = "Mon,Tue,Wed,Thu,Fri",
                        WorkStartTime = new TimeSpan(9, 0, 0),
                        WorkEndTime = new TimeSpan(17, 0, 0),
                        SlotDurationMinutes = 30
                    };
                    context.Doctors.Add(doctor);
                }
            }

            // Seed a demo patient
            if (await userManager.FindByEmailAsync("patient@clinic.com") == null)
            {
                var patientUser = new ApplicationUser
                {
                    UserName = "patient@clinic.com",
                    Email = "patient@clinic.com",
                    FirstName = "John",
                    LastName = "Doe",
                    EmailConfirmed = true,
                    Gender = "Male",
                    DateOfBirth = new DateTime(1990, 3, 20)
                };
                await userManager.CreateAsync(patientUser, "Patient123!");
                await userManager.AddToRoleAsync(patientUser, "Patient");

                var patient = new Patient
                {
                    UserId = patientUser.Id,
                    BloodType = "A+",
                    Allergies = "None",
                    MedicalHistory = "No significant history"
                };
                context.Patients.Add(patient);
            }

            await context.SaveChangesAsync();
        }
    }
}
