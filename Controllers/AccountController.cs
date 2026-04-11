using HotelBooking_CA2.Interfaces;
using HotelBooking_CA2.Models;
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
        public IActionResult Login(string email, string password)
        {
            
            model.Error = (string)null;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                model.Error = "Please fill in all fields.";
                return View(model);
            }

            // intentionally vulnerable - plain text password comparison, no parameterized query abstraction
            var user = _userService.Find(u => u.Email == email && u.Password == password).FirstOrDefault();

            if (user == null)
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
        public IActionResult Register(string fullName, string email, string password, string confirmPassword)
        {
            
            model.Error = (string)null;

            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                model.Error = "All fields are required.";
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
                Password = password, // storing plain text - vulnerable
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
    }
}
