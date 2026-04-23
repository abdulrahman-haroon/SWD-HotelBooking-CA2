# BedBank : Hotel Booking System - Option (B) Developed a basic web application

## 🎥 Video Demo

- [YouTube](https://youtu.be/YcbEOl_3mmQ)
- [OneDrive / Google Drive](https://studentncirl-my.sharepoint.com/:v:/g/personal/x24331244_student_ncirl_ie/IQA91P5w9DU2TIVF5i4s9JdvAagMhjvK_vtGu3qOGl6zevQ?nav=eyJyZWZlcnJhbEluZm8iOnsicmVmZXJyYWxBcHAiOiJPbmVEcml2ZUZvckJ1c2luZXNzIiwicmVmZXJyYWxBcHBQbGF0Zm9ybSI6IldlYiIsInJlZmVycmFsTW9kZSI6InZpZXciLCJyZWZlcnJhbFZpZXciOiJNeUZpbGVzTGlua0NvcHkifX0&e=1kgy6D)

BedBank is a vulnerable ASP.NET Core MVC web application

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

The repository contains **two separate versions** of the application:

| Folder | Description |
|---|---|
| [`Vulnerable Application`](Vulnerable%20Application/) | The original app with all 13 security vulnerabilities intact : used for Phase 1 testing and documentation |
| [`Secured Application`](Secured%20Application/) | The hardened version with all 13 vulnerabilities fixed : the final deliverable |
| [`Screenshots 1`](Screenshots%201/) | Before and after screenshot evidence for all vulnerabilities |

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

Both versions follow a **3-layer architecture** with separation of concerns:

```
WEB Layer (root)                → Controllers, Views, Helpers*, Program.cs
HotelBooking-CA2.BLL            → Interfaces, Services, Dependencies (Business Logic)
HotelBooking-CA2.DAL            → Models, DbContext (Data Access)
```

- **DAL** : Entity models (`User`, `Room`, `Booking`, `Review`) and EF Core `DbContext`
- **BLL** : Generic Repository Pattern (`IGenericRepository<T>` / `GenericRepository<T>`) with service interfaces and implementations for each entity
- **WEB** : MVC controllers, Razor views, and DI configuration

> \* The `Helpers/` folder (`SessionHelper`, `InputValidator`) only exists in the **Secured Application**.

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
- **SQL Injection Prevention** : All database queries use parameterized queries (`FromSqlInterpolated`)
- **Password Security** : BCrypt hashing with complexity requirements (8+ chars, mixed case, digit, special character)
- **Session Security** : Encrypted session data, HttpOnly/Secure/SameSite cookies, 15-minute timeout
- **CSRF Protection** : Anti-forgery tokens validated on every POST action
- **Input Validation** : Server-side HTML sanitization, email validation, and length checks
- **Rate Limiting** : Fixed window limiter blocks brute force login attempts (5 per minute)
- **Access Control** : Role-based admin authorization on all admin endpoints with 403 response
- **Information Security** : Generic error messages prevent leakage of internal details

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

2. **Choose which version to run**
   ```bash
   # For the secured (fixed) version:
   cd "Secured Application"

   # For the vulnerable (original) version:
   cd "Vulnerable Application"
   ```

3. **Set up the database**

   Option A : Run the included SQL script in SSMS:
   ```
   Open BedBank_Database_Script_With_Data.sql (in the repo root) in SQL Server Management Studio and execute it.
   This creates the database with seed data.
   ```

   Option B : Use EF Core migrations:
   ```bash
   dotnet ef database update --project HotelBooking-CA2.DAL
   ```

4. **Restore packages and run**
   ```bash
   dotnet restore
   dotnet run
   ```

5. Open `https://localhost:7183` in your browser.

---

## Database Setup

The application uses **SQL Server LocalDB** with the following connection string in `appsettings.json`:

```
Server=(localdb)\MSSQLLocalDB;Database=BedBankDb;Trusted_Connection=True;
```

EF Core Code First is used : the database is created automatically on first run via migrations.

---

## Default Accounts

### Vulnerable Application

| Name | Email | Password |
|---|---|---|
| System Admin Vulnerable | admin_vuln@BedBank.com | admin123 |
| Abdul Rahman Haroon | x24331244_vuln@student.ncirl.ie | Test123 |

> ⚠️ Passwords are stored in **plain text** in this version (intentional vulnerability).

### Secured Application

| Name | Email | Password |
|---|---|---|
| Admin | admin@BedBank.com | admin123 (hashed with BCrypt in DB) |
| Guest | x24331244@student.ncirl.ie | Test123 (hashed with BCrypt in DB) |

> Passwords are hashed using BCrypt. Use the **Register** page to create new accounts, or use the **Admin panel** to create users with specific roles.

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
4. Click **Book This Room**, select check-in/check-out dates, and confirm : the total price is calculated automatically
5. View your bookings anytime via **My Bookings** in the navbar

### Leaving Reviews
1. Go to a room's **Details** page
2. Scroll down to the **Leave a Review** section (must be logged in)
3. Select a rating (1–5) and write a comment, then submit

### Admin Panel
1. Log in with an Admin account
2. The **Admin** dropdown appears in the navbar with:
   - **Dashboard** : Overview of total rooms, bookings, and users
   - **Manage Rooms** : Create, edit, and delete rooms
   - **All Bookings** : View, edit status/dates, and delete bookings
   - **Manage Users** : Create, edit roles, and delete users
3. Non-admin users attempting to access admin URLs will see a **403 Access Denied** page

---

## Vulnerabilities & Fixes

The application was built in two phases. All screenshot evidence is in the [`Screenshots 1`](Screenshots%201/) folder.

### Phase 1 : Intentionally Vulnerable

13 vulnerabilities were introduced across 6 OWASP categories:

| # | Vulnerability | OWASP Category | Severity | Evidence (Before Fix) |
|---|---|---|---|---|
| 1 | Plain text password storage | A02 : Cryptographic Failures | Critical | [Screenshot 3](Screenshots%201/3%20Plain%20Text%20Password%20Storage.png), [Screenshot 4](Screenshots%201/4%20Plain%20Text%20Password%20Storage.png) |
| 2 | Plain text password comparison | A02 : Cryptographic Failures | Critical | [Screenshot 5](Screenshots%201/5%20Plain%20Text%20Password%20Comparison.png) |
| 3 | Passwords visible in admin panel | A02 : Cryptographic Failures | High | [Screenshot 6](Screenshots%201/6%20Passwords%20Displayed%20in%20Admin%20Panel.png) |
| 4 | SQL injection in room search | A03 : Injection | Critical | [Screenshot 7](Screenshots%201/7%20SQL%20Injection%20(Room%20Search).png), [Screenshot 8](Screenshots%201/8%20SQL%20Injection%20(Room%20Search).png) |
| 5 | No CSRF protection on forms | A01 : Broken Access Control | High | [Screenshot 9](Screenshots%201/9%20%20No%20CSRF%20Protection.png), [Screenshot 10](Screenshots%201/10%20No%20CSRF%20Protection.png) |
| 6 | No input validation / XSS | A03 : Injection | High | [Screenshot 14](Screenshots%201/14%20No%20Input%20Validation%20%20XSS.png), [Screenshot 15](Screenshots%201/15%20No%20Input%20Validation%20%20XSS.png) |
| 7 | Insecure session configuration | A07 : Security Misconfiguration | Medium | [Screenshot 16](Screenshots%201/16%20Insecure%20Session%20Configuration.png) |
| 8 | Unencrypted session data | A02 : Cryptographic Failures | High | [Screenshot 17](Screenshots%201/17%20Session%20Data%20Stored%20in%20Plain%20Text.png), [Screenshot 18](Screenshots%201/18%20Session%20Data%20Stored%20in%20Plain%20Text.png) |
| 9 | No rate limiting on login | A07 : Security Misconfiguration | Medium | [Screenshot 20](Screenshots%201/20%20No%20Brute%20Force%20%20or%20Rate%20Limiting.png) |
| 10 | User enumeration via error messages | A01 : Broken Access Control | Medium | [Screenshot 21](Screenshots%201/21%20User%20Enumeration.png), [Screenshot 22](Screenshots%201/22%20User%20Enumeration.png) |
| 11 | Delete actions without admin check | A01 : Broken Access Control | Critical | [Screenshot 23](Screenshots%201/23%20Privilege%20Escalation%20%E2%80%94%20Delete%20Without%20Admin%20Check.png), [Screenshot 24](Screenshots%201/24%20Privilege%20Escalation%20%E2%80%94%20Delete%20Without%20Admin%20Check.png) |
| 12 | Exception details leaked to users | A05 : Security Misconfiguration | Medium | [Screenshot 26](Screenshots%201/26%20Information%20Leakage%20via%20Exception%20Details.png), [Screenshot 27](Screenshots%201/27%20-%20Exposing%20Information%20Leakage%20via%20Exception%20Details.png) |
| 13 | No password complexity requirements | A07 : Security Misconfiguration | Medium | [Screenshot 28](Screenshots%201/28%20-%20No%20Password%20Complexity%20Requirements.png) |

### Phase 2 : Fixes Applied

Each vulnerability was fixed in a separate Git commit:

| # | Fix | Implementation | Evidence (After Fix) |
|---|---|---|---|
| 1-3 | **Password hashing** | BCrypt.Net-Next : `HashPassword()` on register, `Verify()` on login, removed password column from admin view | [Screenshot 29](Screenshots%201/29%20%20Plain%20Text%20Password%20Storage.png), [Screenshot 30](Screenshots%201/30%20Plain%20Text%20Comparison.png), [Screenshot 31](Screenshots%201/31%20Passwords%20Displayed%20in%20Admin%20Panel%20Removed.png) |
| 4 | **Parameterized SQL** | Changed `FromSqlRaw` with string concatenation to `FromSqlInterpolated` | [Screenshot 32](Screenshots%201/32%20SQL%20Injection%20(Room%20Search)%20Fix.png), [Screenshot 33](Screenshots%201/33%20SQL%20Injection%20(Room%20Search)%20Fix.png) |
| 5 | **CSRF protection** | Added `[ValidateAntiForgeryToken]` to all 11 `[HttpPost]` actions | [Screenshot 34](Screenshots%201/34%20No%20CSRF%20Protection%20Fix.png), [Screenshot 35](Screenshots%201/35%20No%20CSRF%20Protection%20Fix.png) |
| 6 | **Input validation** | Created `InputValidator` helper : `Sanitize()` strips HTML, `IsValidEmail()`, `IsValidLength()` | [Screenshot 36](Screenshots%201/36%20No%20Input%20Validation%20(Fix).png) |
| 7 | **Session hardening** | `HttpOnly=true`, `SecurePolicy=Always`, `SameSite=Strict`, 15-min timeout, custom cookie name | [Screenshot 39](Screenshots%201/39%20Insecure%20Session%20Configuration%20(Fix).png), [Screenshot 40](Screenshots%201/40%20Insecure%20Session%20Configuration%20(Fix).png) |
| 8 | **Session encryption** | Created `SessionHelper` using ASP.NET Core Data Protection API to encrypt/decrypt all session values | [Screenshot 41](Screenshots%201/41%20Session%20Data%20Stored%20in%20Plain%20Text%20(fix).png) |
| 9 | **Rate limiting** | ASP.NET Core Fixed Window Rate Limiter : 5 login attempts per minute per client | [Screenshot 42](Screenshots%201/42%20No%20Brute%20Force%20%20Rate%20Limiting%20(fix).png), [Screenshot 43](Screenshots%201/43%20No%20Brute%20Force%20%20Rate%20Limiting%20(fix).png) |
| 10 | **User enumeration fix** | Generic error messages + dummy BCrypt hash on failed lookup to prevent timing attacks | [Screenshot 45](Screenshots%201/45%20%20User%20Enumeration%20(fix).png) |
| 11 | **Admin authorization** | Restored `IsAdmin()` check on all delete actions, returns 403 Unauthorized view | [Screenshot 46](Screenshots%201/46%20Privilege%20Escalation%20(fix).png), [Screenshot 47](Screenshots%201/47%20Privilege%20Escalation%20(fix).png) |
| 12 | **Error detail removal** | Replaced `ex.ToString()` in GenericRepository with generic error messages | [Screenshot 48](Screenshots%201/48%20Information%20Leakage%20via%20Exception%20Details.png) |
| 13 | **Password complexity** | `IsStrongPassword()` : minimum 8 chars, uppercase, lowercase, digit, special character | [Screenshot 49](Screenshots%201/49%20No%20Password%20Complexity%20Requirements%20.png) |

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
1. **Phase 1 (Before Fix)** : Reproduce and document the vulnerability with screenshots
2. **Phase 2 (After Fix)** : Verify the fix blocks the attack, capture evidence

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
| Weak password registration | "123" accepted as password | Rejected : requires 8+ chars with mixed case, digit, special char |
| User enumeration | "Email already exists" on register | Generic "Unable to create account" message |

---

## Git History

The Git history is structured to show the progression from vulnerable to secure:

1. Initial project setup and 3-layer architecture
2. Feature implementation (rooms, bookings, reviews, admin panel)
3. Intentional vulnerabilities introduced
4. Sequential fixes : one commit per vulnerability

---

## Project Structure

```
SWD-HotelBooking-CA2/
├── Vulnerable Application/              (Phase 1 : with all 13 vulnerabilities)
│   ├── Controllers/
│   │   ├── HomeController.cs
│   │   ├── AccountController.cs         (plain text passwords, no rate limiting)
│   │   ├── RoomController.cs            (SQL injection via FromSqlRaw)
│   │   ├── BookingController.cs         (no session encryption)
│   │   └── AdminController.cs           (no admin check on deletes, leaks exceptions)
│   ├── Views/
│   │   ├── Home/                        (Index, Privacy)
│   │   ├── Account/                     (Login, Register : no CSRF tokens)
│   │   ├── Room/                        (Index, Details)
│   │   ├── Booking/                     (Index, Create)
│   │   ├── Admin/                       (Dashboard, CRUD views : shows passwords)
│   │   └── Shared/                      (_Layout)
│   ├── HotelBooking-CA2.BLL/            (Business Logic Layer)
│   │   ├── Interfaces/                  (IGenericRepository, IRoom/Booking/User/ReviewService)
│   │   ├── Services/                    (GenericRepository : returns ex.ToString())
│   │   └── Dependencies/               (ServicesDependency : DI registration)
│   ├── HotelBooking-CA2.DAL/            (Data Access Layer)
│   │   ├── Models/                      (User, Room, Booking, Review)
│   │   └── Context/                     (OTA_APP_DBContext)
│   ├── Program.cs                       (no session hardening, no rate limiter)
│   └── appsettings.json
│
├── Secured Application/                 (Phase 2 : all vulnerabilities fixed)
│   ├── Controllers/
│   │   ├── HomeController.cs
│   │   ├── AccountController.cs         (BCrypt, rate limiting, enumeration fix)
│   │   ├── RoomController.cs            (parameterized SQL, input validation)
│   │   ├── BookingController.cs         (session encryption, date validation)
│   │   └── AdminController.cs           (admin auth on all actions, CSRF)
│   ├── Helpers/
│   │   ├── InputValidator.cs            (sanitize, email, length, password strength)
│   │   └── SessionHelper.cs             (Data Protection API encryption)
│   ├── Views/
│   │   ├── Home/                        (Index, Privacy, NotFound)
│   │   ├── Account/                     (Login, Register)
│   │   ├── Room/                        (Index, Details)
│   │   ├── Booking/                     (Index, Create)
│   │   ├── Admin/                       (Dashboard, CRUD views, Unauthorized)
│   │   └── Shared/                      (_Layout, _ValidationScriptsPartial)
│   ├── HotelBooking-CA2.BLL/            (Business Logic Layer)
│   │   ├── Interfaces/                  (IGenericRepository, IRoom/Booking/User/ReviewService)
│   │   ├── Services/                    (GenericRepository, service implementations)
│   │   └── Dependencies/               (ServicesDependency : DI registration)
│   ├── HotelBooking-CA2.DAL/            (Data Access Layer)
│   │   ├── Models/                      (User, Room, Booking, Review)
│   │   └── Context/                     (OTA_APP_DBContext)
│   ├── Program.cs                       (Session hardening, rate limiter, DI config)
│   └── appsettings.json
│
├── Screenshots 1/                       (Before & after evidence for all 13 vulnerabilities)
├── BedBank_Database_Script_With_Data.sql (Database creation script with seed data)
└── README.md
```

---

## Author

**Abdul Rahman Haroon**
National College of Ireland : Secure Web Development CA2

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

- [OWASP Top 10 (2021)](https://owasp.org/www-project-top-ten/) : Classification framework for all 13 vulnerabilities
- [ASP.NET Core Data Protection API](https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/) : Used for session value encryption
- [ASP.NET Core Rate Limiting](https://learn.microsoft.com/en-us/aspnet/core/performance/rate-limit) : Built-in fixed window rate limiter
- [OWASP Cheat Sheet: Session Management](https://cheatsheetseries.owasp.org/cheatsheets/Session_Management_Cheat_Sheet.html) : Session hardening guidance
- [OWASP Cheat Sheet: Authentication](https://cheatsheetseries.owasp.org/cheatsheets/Authentication_Cheat_Sheet.html) : Password hashing and user enumeration prevention
- [Unsplash](https://unsplash.com/) : Room images used in the application
