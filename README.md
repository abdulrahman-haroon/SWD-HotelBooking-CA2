# BedBank — Hotel Booking System

BedBank is a vulnerable ASP.NET Core MVC web application built for the **NCI Secure Web Development CA2 assignment (Option B)**. The project demonstrates common web security vulnerabilities, documents them against OWASP standards, and then applies fixes systematically.

## Table of Contents

- [About the Project](#about-the-project)
- [Tech Stack](#tech-stack)
- [Architecture](#architecture)
- [Features and Security Objectives](#features-and-security-objectives)
- [Getting Started](#getting-started)
- [Database Setup](#database-setup)
- [Default Accounts](#default-accounts)
- [Usage Guidelines](#usage-guidelines)
- [Vulnerabilities & Fixes](#vulnerabilities--fixes)
- [Security Improvements Summary](#security-improvements-summary)
- [Testing Process](#testing-process)
- [Git History](#git-history)
- [Project Structure](#project-structure)
- [Contributions and References](#contributions-and-references)

---

## About the Project

BedBank is a hotel room booking system where guests can browse rooms, make bookings, and leave reviews. Admins can manage rooms, bookings, and users through admin panel.

The application was first built with **13 intentional security vulnerabilities** across OWASP categories. Each vulnerability was documented with reproduction steps and screenshots. In Phase 2, all vulnerabilities were fixed one-by-one with Git commits.

---

## Tech Stack

| Component | Technology |
|---|---|
| Framework | ASP.NET Core MVC (.NET 10) |
| Language | C# 14 |
| Database | SQL Server LocalDB |
| ORM | Entity Framework Core 10 |
| Password Hashing | BCrypt.Net-Next 4.1.0 |
| Session Encryption | ASP.NET Core Data Protection API |
| Frontend | Bootstrap 5, Razor Views |

---

## Architecture

The solution follows a **3-layer architecture** with separation of concerns:

```
HotelBooking-CA2 (WEB)          → Controllers, Views, Helpers, Program.cs
HotelBooking-CA2.BLL            → Interfaces, Services, Dependencies (Business Logic)
HotelBooking-CA2.DAL            → Models, DbContext (Data Access)
```

- **DAL** — Entity models (`User`, `Room`, `Booking`, `Review`) and EF Core `DbContext`
- **BLL** — Generic Repository Pattern (`IGenericRepository<T>` / `GenericRepository<T>`) with service interfaces and implementations for each entity
- **WEB** — MVC controllers, Razor views, helper classes (`SessionHelper`, `InputValidator`), and DI configuration

---

## Features and Security Objectives

### Guest Features
- Browse available rooms with search functionality
- View room details with images and reviews
- Book rooms with date selection and price calculation
- Leave reviews and ratings on rooms
- Register and login with session-based authentication

### Admin Features
- Dashboard with room, booking, and user counts
- Full CRUD for rooms (with image URLs)
- Manage all bookings (edit status, dates, recalculate price)
- Manage users (create, edit roles, delete)

### Security Objectives
- **SQL Injection Prevention** — All database queries use parameterized queries (`FromSqlInterpolated`)
- **Password Security** — BCrypt hashing with complexity requirements (8+ chars, mixed case, digit, special character)
- **Session Security** — Encrypted session data, HttpOnly/Secure/SameSite cookies, 15-minute timeout
- **CSRF Protection** — Anti-forgery tokens validated on every POST action
- **Input Validation** — Server-side HTML sanitization, email validation, and length checks
- **Rate Limiting** — Fixed window limiter blocks brute force login attempts (5 per minute)
- **Access Control** — Role-based admin authorization on all admin endpoints with 403 response
- **Information Security** — Generic error messages prevent leakage of internal details

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [SQL Server LocalDB](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb) (included with Visual Studio)
- Visual Studio 2022/2026 or VS Code

### Run the Application

1. **Clone the repository**
   ```bash
   git clone https://github.com/abdulrahman-haroon/SWD-HotelBooking-CA2.git
   cd SWD-HotelBooking-CA2
   ```

2. **Restore packages**
   ```bash
   dotnet restore
   ```

3. **Apply database migrations**
   ```bash
   dotnet ef database update --project HotelBooking-CA2.DAL
   or 
   execute BedBank_Database_Script_With_Data.sql in SSMS to create the database with seed data.
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

5. Open `https://localhost:7183` in your browser.

---

## Database Setup

The application uses **SQL Server LocalDB** with the following connection string in `appsettings.json`:

```
Server=(localdb)\MSSQLLocalDB;Database=BedBankDb;Trusted_Connection=True;
```

EF Core Code First is used — the database is created automatically on first run via migrations.

---

## Default Accounts

| Role | Email | Password |
|---|---|---|
| Admin | admin@BedBank.com | admin123 (this is hashed in db) |
| Guest | x24331244@student.ncirl.ie | Test123 (this is hashed in db) |

> After running the app, use the **Register** page to create accounts, or use the **Admin panel** to create users with specific roles.

---

## Usage Guidelines

### Registration and Login
1. Navigate to **Register** from the top navbar
2. Fill in your name, email, and a strong password (8+ characters with uppercase, lowercase, digit, and special character)
3. After registration you are automatically logged in
4. To log out, click **Logout** in the top-right navbar

### Browsing and Booking Rooms
1. Click **Rooms** in the navbar to view all available rooms
2. Use the **search bar** to filter rooms by name, type, or description
3. Click **Details** on a room card to see full details, images, and reviews
4. Click **Book This Room**, select check-in/check-out dates, and confirm — the total price is calculated automatically
5. View your bookings anytime via **My Bookings** in the navbar

### Leaving Reviews
1. Go to a room's **Details** page
2. Scroll down to the **Leave a Review** section (must be logged in)
3. Select a rating (1–5) and write a comment, then submit

### Admin Panel
1. Log in with an Admin account
2. The **Admin** dropdown appears in the navbar with:
   - **Dashboard** — Overview of total rooms, bookings, and users
   - **Manage Rooms** — Create, edit, and delete rooms
   - **All Bookings** — View, edit status/dates, and delete bookings
   - **Manage Users** — Create, edit roles, and delete users
3. Non-admin users attempting to access admin URLs will see a **403 Access Denied** page

---

## Vulnerabilities & Fixes

The application was built in two phases:

### Phase 1 — Intentionally Vulnerable

13 vulnerabilities were introduced across 6 OWASP categories:

| # | Vulnerability | OWASP Category | Severity | File(s) |
|---|---|---|---|---|
| 1 | Plain text password storage | A02 — Cryptographic Failures | Critical | AccountController.cs |
| 2 | Plain text password comparison | A02 — Cryptographic Failures | Critical | AccountController.cs |
| 3 | Passwords visible in admin panel | A02 — Cryptographic Failures |  High | Admin/Users.cshtml |
| 4 | SQL injection in room search | A03 — Injection | Critical | RoomController.cs |
| 5 | No CSRF protection on forms | A01 — Broken Access Control |  High | All controllers |
| 6 | No input validation or sanitization | A03 — Injection |  High | All controllers |
| 7 | Insecure session configuration | A07 — Security Misconfiguration | Medium | Program.cs |
| 8 | Unencrypted session data | A02 — Cryptographic Failures |  High | All controllers |
| 9 | No rate limiting on login | A07 — Security Misconfiguration | Medium | AccountController.cs |
| 10 | User enumeration via error messages | A01 — Broken Access Control | Medium | AccountController.cs |
| 11 | Delete actions without admin check | A01 — Broken Access Control | Critical | AdminController.cs |
| 12 | Exception details leaked to users | A05 — Security Misconfiguration | Medium | GenericRepository.cs |
| 13 | No password complexity requirements | A07 — Security Misconfiguration | Medium | AccountController.cs |

### Phase 2 — Fixes Applied

Each vulnerability was fixed in a separate Git commit:

| # | Fix | Implementation |
|---|---|---|
| 1-3 | **Password hashing** | BCrypt.Net-Next — `HashPassword()` on register, `Verify()` on login, removed password column from admin view |
| 4 | **Parameterized SQL** | Changed `FromSqlRaw` with string concatenation to `FromSqlInterpolated` |
| 5 | **CSRF protection** | Added `[ValidateAntiForgeryToken]` to all 11 `[HttpPost]` actions |
| 6 | **Input validation** | Created `InputValidator` helper — `Sanitize()` strips HTML, `IsValidEmail()`, `IsValidLength()` |
| 7 | **Session hardening** | `HttpOnly=true`, `SecurePolicy=Always`, `SameSite=Strict`, 15-min timeout, custom cookie name |
| 8 | **Session encryption** | Created `SessionHelper` using ASP.NET Core Data Protection API to encrypt/decrypt all session values |
| 9 | **Rate limiting** | ASP.NET Core Fixed Window Rate Limiter — 5 login attempts per minute per client |
| 10 | **User enumeration fix** | Generic error messages + dummy BCrypt hash on failed lookup to prevent timing attacks |
| 11 | **Admin authorization** | Restored `IsAdmin()` check on all delete actions, returns 403 Unauthorized view |
| 12 | **Error detail removal** | Replaced `ex.ToString()` in GenericRepository with generic error messages |
| 13 | **Password complexity** | `IsStrongPassword()` — minimum 8 chars, uppercase, lowercase, digit, special character |

---

## Security Improvements Summary

The following security layers were implemented to harden the application:

| Layer | Implementation | OWASP Coverage |
|---|---|---|
| **Authentication** | BCrypt password hashing, password complexity enforcement, rate limiting (5 attempts/min) | A02, A07 |
| **Authorization** | `IsAdmin()` role check on all admin actions, 403 Unauthorized view for non-admin users | A01 |
| **Input Security** | `InputValidator` helper with HTML tag stripping, email regex validation, length constraints | A03 |
| **SQL Injection Prevention** | `FromSqlInterpolated` with parameterized queries replacing raw string concatenation | A03 |
| **Session Security** | Data Protection API encryption, HttpOnly + Secure + SameSite=Strict cookies, 15-min idle timeout | A02, A07 |
| **CSRF Protection** | `[ValidateAntiForgeryToken]` on all 11 POST actions across 4 controllers | A01 |
| **Error Handling** | Generic error messages in repository layer and controllers, no stack traces exposed | A05 |
| **User Privacy** | Generic login/register error messages, timing-safe dummy BCrypt hash to prevent enumeration | A01 |

---

## Testing Process

### Testing Approach

Each vulnerability was tested in two phases:
1. **Phase 1 (Before Fix)** — Reproduce and document the vulnerability with screenshots
2. **Phase 2 (After Fix)** — Verify the fix blocks the attack, capture evidence

### Testing Tools Used

| Tool | Purpose |
|---|---|
| **Browser DevTools** | Inspecting session cookies (HttpOnly, Secure, SameSite flags), network requests, and response headers |
| **PowerShell / cURL** | Automated testing of rate limiting (sending rapid POST requests to login endpoint) |
| **SQL Injection Payloads** | Manual injection via search bar (e.g. `%' OR 1=1 --`, `%' UNION SELECT...`) to test parameterized queries |
| **Direct URL Manipulation** | Accessing admin endpoints (`/Admin/DeleteRoom`, `/Admin/DeleteUser`) as a Guest user to test authorization |
| **CSRF Testing** | Submitting POST forms without anti-forgery tokens to verify rejection |

### Key Testing Findings

| Test | Before Fix | After Fix |
|---|---|---|
| SQL injection in search | `%' OR 1=1 --` returned all rooms | Query is parameterized, payload treated as literal text |
| Brute force login | Unlimited attempts allowed | Blocked after 5 attempts per minute with friendly error |
| Admin delete as Guest | POST to `/Admin/DeleteRoom/1` succeeded | Returns 403 Access Denied page |
| Session cookie inspection | Cookie accessible via JavaScript, no Secure flag | HttpOnly=true, Secure=true, SameSite=Strict |
| Exception details on error | Full stack trace with DB schema shown to user | Generic "An error occurred" message |
| Weak password registration | "123" accepted as password | Rejected — requires 8+ chars with mixed case, digit, special char |
| User enumeration | "Email already exists" on register | Generic "Unable to create account" message |

---

## Git History

The Git history is structured to show the progression from vulnerable to secure:

1. Initial project setup and 3-layer architecture
2. Feature implementation (rooms, bookings, reviews, admin panel)
3. Intentional vulnerabilities introduced
4. Sequential fixes — one commit per vulnerability

---

## Project Structure

```
SWD-HotelBooking-CA2/
├── HotelBooking-CA2/                    (WEB Layer)
│   ├── Controllers/
│   │   ├── HomeController.cs
│   │   ├── AccountController.cs
│   │   ├── RoomController.cs
│   │   ├── BookingController.cs
│   │   └── AdminController.cs
│   ├── Helpers/
│   │   ├── InputValidator.cs
│   │   └── SessionHelper.cs
│   ├── Views/
│   │   ├── Home/          (Index, Privacy, NotFound)
│   │   ├── Account/       (Login, Register)
│   │   ├── Room/          (Index, Details)
│   │   ├── Booking/       (Index, Create)
│   │   ├── Admin/         (Dashboard, CRUD views, Unauthorized)
│   │   └── Shared/        (_Layout, _ValidationScriptsPartial)
│   ├── Program.cs
│   └── appsettings.json
├── HotelBooking-CA2.BLL/               (Business Logic Layer)
│   ├── Interfaces/
│   │   ├── IGenericRepository.cs
│   │   ├── IRoomService.cs
│   │   ├── IBookingService.cs
│   │   ├── IUserService.cs
│   │   └── IReviewService.cs
│   ├── Services/
│   │   ├── GenericRepository.cs
│   │   ├── RoomService.cs
│   │   ├── BookingService.cs
│   │   ├── UserService.cs
│   │   └── ReviewService.cs
│   └── Dependencies/
│       └── ServicesDependency.cs
├── HotelBooking-CA2.DAL/               (Data Access Layer)
│   ├── Models/
│   │   ├── User.cs
│   │   ├── Room.cs
│   │   ├── Booking.cs
│   │   └── Review.cs
│   └── Context/
│       └── OTA_APP_DBContext.cs
└── README.md
```

---

## Author

**Abdul Rahman Haroon**
National College of Ireland — Secure Web Development CA2

---

## Contributions and References

### Frameworks and Libraries

| Library | Version | Purpose | Link |
|---|---|---|---|
| ASP.NET Core MVC | .NET 10 | Web application framework | [docs.microsoft.com](https://learn.microsoft.com/en-us/aspnet/core/) |
| Entity Framework Core | 10.0.5 | Object-Relational Mapper (ORM) | [docs.microsoft.com](https://learn.microsoft.com/en-us/ef/core/) |
| BCrypt.Net-Next | 4.1.0 | Password hashing library | [nuget.org](https://www.nuget.org/packages/BCrypt.Net-Next/) |
| Bootstrap | 5.x | CSS framework for responsive UI | [getbootstrap.com](https://getbootstrap.com/) |

### References

- [OWASP Top 10 (2021)](https://owasp.org/www-project-top-ten/) — Classification framework for all 13 vulnerabilities
- [ASP.NET Core Data Protection API](https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/) — Used for session value encryption
- [ASP.NET Core Rate Limiting](https://learn.microsoft.com/en-us/aspnet/core/performance/rate-limit) — Built-in fixed window rate limiter
- [OWASP Cheat Sheet: Session Management](https://cheatsheetseries.owasp.org/cheatsheets/Session_Management_Cheat_Sheet.html) — Session hardening guidance
- [OWASP Cheat Sheet: Authentication](https://cheatsheetseries.owasp.org/cheatsheets/Authentication_Cheat_Sheet.html) — Password hashing and user enumeration prevention
- [Unsplash](https://unsplash.com/) — Room images used in the application