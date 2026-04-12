using HotelBooking_CA2.Interfaces;
using HotelBooking_CA2.Models;
using HotelBooking_CA2.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Dynamic;

namespace HotelBooking_CA2.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly SessionHelper _session;
        private readonly dynamic model;

        public AccountController(IUserService userService, SessionHelper session)
        {
            _userService = userService;
            _session = session;
            model = new ExpandoObject();
        }

        [HttpGet]
        public IActionResult Login(bool rateLimited = false)
        {
            model.Error = rateLimited ? "Too many login attempts. Please wait a minute and try again." : (string)null;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EnableRateLimiting("LoginRateLimit")]
        public IActionResult Login(string email, string password)
        {

            model.Error = (string)null;

            email = InputValidator.Sanitize(email);

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                model.Error = "Please fill in all fields.";
                return View(model);
            }

            if (!InputValidator.IsValidEmail(email))
            {
                model.Error = "Please enter a valid email address.";
                return View(model);
            }

            // intentionally vulnerable - plain text password comparison, no parameterized query abstraction
            var user = _userService.Find(u => u.Email == email).FirstOrDefault();

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                model.Error = "Invalid email or password.";
                return View(model);
            }

            // store user info in session - encrypted via DataProtection
            _session.SetInt32("UserId", user.Id);
            _session.SetString("UserName", user.FullName);
            _session.SetString("UserRole", user.Role);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            
            model.Error = (string)null;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(string fullName, string email, string password, string confirmPassword)
        {
            
            model.Error = (string)null;

            fullName = InputValidator.Sanitize(fullName);
            email = InputValidator.Sanitize(email);

            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                model.Error = "All fields are required.";
                return View(model);
            }

            if (!InputValidator.IsValidLength(fullName, 2, 100))
            {
                model.Error = "Full name must be between 2 and 100 characters.";
                return View(model);
            }

            if (!InputValidator.IsValidEmail(email))
            {
                model.Error = "Please enter a valid email address.";
                return View(model);
            }

            if (password != confirmPassword)
            {
                model.Error = "Passwords do not match.";
                return View(model);
            }

            // check if email already exists
            var existing = _userService.Find(u => u.Email == email).FirstOrDefault();
            if (existing != null)
            {
                model.Error = "An account with this email already exists.";
                return View(model);
            }

            var user = new User
            {
                FullName = fullName,
                Email = email,
                Password = BCrypt.Net.BCrypt.HashPassword(password),
                Role = "Guest",
                CreatedAt = DateTime.Now
            };

            _userService.Insert(user);
            _userService.SaveChanges();

            // auto login after registration
            _session.SetInt32("UserId", user.Id);
            _session.SetString("UserName", user.FullName);
            _session.SetString("UserRole", user.Role);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            _session.Clear();
            return RedirectToAction("Login");
        }

        // one-time migration to hash existing plain text passwords
        public IActionResult MigratePasswords()
        {
            var users = _userService.GetAll();
            foreach (var user in users)
            {
                // skip if already hashed (BCrypt hashes start with $2)
                if (!user.Password.StartsWith("$2"))
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                    _userService.Update(user);
                }
            }
            _userService.SaveChanges();
            return Content("Passwords migrated successfully.");
        }

        // intentionally vulnerable - exposes session data for debugging, no auth check
        public IActionResult SessionDebug()
        {
            var data = new
            {
                UserId = _session.GetInt32("UserId"),
                UserName = _session.GetString("UserName"),
                UserRole = _session.GetString("UserRole")
            };
            return Json(data);
        }
    }
}
