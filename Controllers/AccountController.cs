using HotelBooking_CA2.Interfaces;
using HotelBooking_CA2.Models;
using HotelBooking_CA2.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Dynamic;

namespace HotelBooking_CA2.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly dynamic model;

        public AccountController(IUserService userService)
        {
            _userService = userService;
            model = new ExpandoObject();
        }

        [HttpGet]
        public IActionResult Login()
        {
            model.Error = (string)null;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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

            // store user info in session - no encryption, plain values
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserName", user.FullName);
            HttpContext.Session.SetString("UserRole", user.Role);

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
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserName", user.FullName);
            HttpContext.Session.SetString("UserRole", user.Role);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
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
                UserId = HttpContext.Session.GetInt32("UserId"),
                UserName = HttpContext.Session.GetString("UserName"),
                UserRole = HttpContext.Session.GetString("UserRole")
            };
            return Json(data);
        }
    }
}
