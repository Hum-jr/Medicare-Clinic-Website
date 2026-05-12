<<<<<<< HEAD
# 🏥 MediCare – Clinic Appointment Management System

An ASP.NET Core 8 MVC application for managing clinic appointments with role-based access for Patients, Doctors, and Admins.

---

## 🚀 Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- SQL Server (LocalDB is fine — ships with Visual Studio)
- Visual Studio 2022 or VS Code

---

## ⚙️ Setup Steps

### 1. Open the project
```
Open ClinicApp.sln or the folder in Visual Studio / VS Code
```

### 2. Configure the database
The default connection string in `appsettings.json` uses SQL Server LocalDB:
```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ClinicAppDb;Trusted_Connection=True"
```
Change this if you're using a full SQL Server instance.

### 3. Apply migrations & seed the database
Open **Package Manager Console** (Tools → NuGet → Package Manager Console) and run:
```powershell
Update-Database
```
The app will auto-seed roles, specialties, and demo accounts on first run.

### 4. Run the application
```
dotnet run
```
Or press **F5** in Visual Studio.

---

## 🔑 Demo Accounts

| Role    | Email                    | Password     |
|---------|--------------------------|--------------|
| Admin   | admin@clinic.com         | Admin123!    |
| Doctor  | dr.smith@clinic.com      | Doctor123!   |
| Doctor  | dr.johnson@clinic.com    | Doctor123!   |
| Patient | patient@clinic.com       | Patient123!  |

---

## 🗂️ Project Structure

```
ClinicApp/
├── Controllers/
│   ├── HomeController.cs          # Public pages
│   ├── AccountController.cs       # Login / Register / Logout
│   ├── AppointmentsController.cs  # Patient appointment booking
│   ├── DoctorsController.cs       # Doctor dashboard & schedule
│   └── AdminController.cs         # Admin panel
│
├── Models/
│   ├── ApplicationUser.cs         # Extended Identity user
│   ├── Patient.cs
│   ├── Doctor.cs
│   ├── Specialty.cs
│   └── Appointment.cs             # Includes AppointmentStatus enum
│
├── ViewModels/
│   └── ViewModels.cs              # All view models in one file
│
├── Services/
│   ├── AppointmentService.cs      # Booking logic, slot generation
│   └── DoctorService.cs           # Doctor queries
│
├── Data/
│   ├── ApplicationDbContext.cs    # EF Core DbContext
│   └── DbSeeder.cs                # Seed roles, doctors, patients
│
├── Views/
│   ├── Home/                      # Landing page, doctor listing
│   ├── Account/                   # Login, Register, AccessDenied
│   ├── Appointments/              # Index, Book, Details (Patient)
│   ├── Doctors/                   # Dashboard, Schedule (Doctor)
│   ├── Admin/                     # Dashboard, Appointments, Doctors, Patients
│   └── Shared/                    # _Layout, _ValidationScriptsPartial
│
├── wwwroot/
│   ├── css/site.css               # Custom clinic theme
│   └── js/site.js
│
├── Migrations/                    # EF Core migration files
├── appsettings.json
└── Program.cs
```

---

## ✅ Features by Role

### 🙍 Patient
- Register and log in
- Browse all doctors by specialty or search
- Book appointments with available time slots (AJAX-loaded)
- View upcoming and past appointments
- Cancel upcoming appointments

### 👨‍⚕️ Doctor
- Dashboard showing today's appointments and stats
- Daily schedule view with date picker
- Mark appointments as completed
- Add doctor notes to appointments

### 🛠️ Admin
- System-wide dashboard with KPIs
- View all appointments with status filter
- Manage doctors (add, toggle active status)
- View all patients

---

## 🛠️ Tech Stack

| Layer        | Technology                          |
|--------------|-------------------------------------|
| Framework    | ASP.NET Core 8 MVC                  |
| ORM          | Entity Framework Core 8             |
| Database     | SQL Server / LocalDB                |
| Auth         | ASP.NET Core Identity               |
| UI           | Bootstrap 5, Font Awesome 6         |
| Font         | Plus Jakarta Sans (Google Fonts)    |
| Validation   | jQuery Unobtrusive Validation       |

---

## 📝 Notes for Class

- **MVC Pattern**: Each entity has its own Controller; Views are separated per role area
- **Service Layer**: Business logic is in `Services/`, not in controllers
- **Repository-like pattern**: DbContext is injected into services via DI
- **Role-based Authorization**: `[Authorize(Roles = "...")]` on all protected controllers
- **Seeding**: `DbSeeder.cs` runs on startup to create demo data automatically
=======
# Medicare-Clinic-Website
A class project made in ASP.NET
>>>>>>> d693f371fad0a116e48c53073d82d061e3622226
