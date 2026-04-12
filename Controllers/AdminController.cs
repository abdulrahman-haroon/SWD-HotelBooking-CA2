using HotelBooking_CA2.Interfaces;
using HotelBooking_CA2.Models;
using HotelBooking_CA2.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;

namespace HotelBooking_CA2.Controllers
{
    public class AdminController : Controller
    {
        private readonly IRoomService _roomService;
        private readonly IBookingService _bookingService;
        private readonly IUserService _userService;
        private readonly SessionHelper _session;
        private readonly dynamic model;

        public AdminController(IRoomService roomService, IBookingService bookingService, IUserService userService, SessionHelper session)
        {
            _roomService = roomService;
            _bookingService = bookingService;
            _userService = userService;
            _session = session;
            model = new ExpandoObject();
        }

        private bool IsAdmin()
        {
            return _session.GetString("UserRole") == "Admin";
        }

        private IActionResult Unauthorized()
        {
            return View("Unauthorized");
        }

        public IActionResult Index()
        {
            if (!IsAdmin())
                return Unauthorized();

            var totalRooms = _roomService.GetAll().Count();
            var totalBookings = _bookingService.GetAll().Count();
            var totalUsers = _userService.GetAll().Count();

            model.TotalRooms = totalRooms;
            model.TotalBookings = totalBookings;
            model.TotalUsers = totalUsers;
            return View(model);
        }

        public IActionResult Rooms()
        {
            if (!IsAdmin())
                return Unauthorized();

            var rooms = _roomService.GetAll();
            return View(rooms);
        }

        [HttpGet]
        public IActionResult CreateRoom()
        {
            if (!IsAdmin())
                return Unauthorized();

            model.Error = (string)null;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateRoom(string roomNumber, string roomType, string description, decimal pricePerNight, int capacity, bool isAvailable, string imageUrl)
        {
            if (!IsAdmin())
                return Unauthorized();

            model.Error = (string)null;

            roomNumber = InputValidator.Sanitize(roomNumber);
            roomType = InputValidator.Sanitize(roomType);
            description = InputValidator.Sanitize(description);
            imageUrl = InputValidator.Sanitize(imageUrl);

            if (string.IsNullOrEmpty(roomNumber) || string.IsNullOrEmpty(roomType))
            {
                model.Error = "Room number and type are required.";
                return View(model);
            }

            if (!InputValidator.IsValidLength(roomNumber, 1, 10) || !InputValidator.IsValidLength(roomType, 2, 50))
            {
                model.Error = "Room number (max 10) and type (2-50 chars) must be valid lengths.";
                return View(model);
            }

            if (pricePerNight <= 0 || capacity <= 0)
            {
                model.Error = "Price and capacity must be greater than zero.";
                return View(model);
            }

            var room = new Room
            {
                RoomNumber = roomNumber,
                RoomType = roomType,
                Description = description ?? "",
                PricePerNight = pricePerNight,
                Capacity = capacity,
                IsAvailable = isAvailable,
                ImageUrl = imageUrl ?? ""
            };

            _roomService.Insert(room);
            _roomService.SaveChanges();

            return RedirectToAction("Rooms");
        }

        [HttpGet]
        public IActionResult EditRoom(int id)
        {
            if (!IsAdmin())
                return Unauthorized();

            var room = _roomService.GetById(id);
            if (room == null)
                return NotFound();

            model.Room = room;
            model.Error = (string)null;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditRoom(int id, string roomNumber, string roomType, string description, decimal pricePerNight, int capacity, bool isAvailable, string imageUrl)
        {
            if (!IsAdmin())
                return Unauthorized();

            var room = _roomService.GetById(id);
            if (room == null)
                return NotFound();

            model.Room = room;
            model.Error = (string)null;

            roomNumber = InputValidator.Sanitize(roomNumber);
            roomType = InputValidator.Sanitize(roomType);
            description = InputValidator.Sanitize(description);
            imageUrl = InputValidator.Sanitize(imageUrl);

            if (string.IsNullOrEmpty(roomNumber) || string.IsNullOrEmpty(roomType))
            {
                model.Error = "Room number and type are required.";
                return View(model);
            }

            if (!InputValidator.IsValidLength(roomNumber, 1, 10) || !InputValidator.IsValidLength(roomType, 2, 50))
            {
                model.Error = "Room number (max 10) and type (2-50 chars) must be valid lengths.";
                return View(model);
            }

            if (pricePerNight <= 0 || capacity <= 0)
            {
                model.Error = "Price and capacity must be greater than zero.";
                return View(model);
            }

            room.RoomNumber = roomNumber;
            room.RoomType = roomType;
            room.Description = description ?? "";
            room.PricePerNight = pricePerNight;
            room.Capacity = capacity;
            room.IsAvailable = isAvailable;
            room.ImageUrl = imageUrl ?? "";

            _roomService.Update(room);
            _roomService.SaveChanges();

            return RedirectToAction("Rooms");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteRoom(int id)
        {
            if (!IsAdmin())
                return Unauthorized();

            var result = _roomService.Delete(id);
            var saveResult = _roomService.SaveChanges();

            // return generic error message - do not expose internal details
            if (!string.IsNullOrEmpty(result) || !string.IsNullOrEmpty(saveResult))
                return RedirectToAction("Rooms");

            return RedirectToAction("Rooms");
        }

        public IActionResult Bookings()
        {
            if (!IsAdmin())
                return Unauthorized();

            var bookings = _bookingService.dbset()
                .Include(b => b.Room)
                .Include(b => b.User)
                .OrderByDescending(b => b.BookedAt)
                .ToList();

            return View(bookings);
        }

        [HttpGet]
        public IActionResult EditBooking(int id)
        {
            if (!IsAdmin())
                return Unauthorized();

            var booking = _bookingService.dbset()
                .Include(b => b.Room)
                .Include(b => b.User)
                .FirstOrDefault(b => b.Id == id);

            if (booking == null)
                return NotFound();

            model.Booking = booking;
            model.Error = (string)null;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditBooking(int id, DateTime checkIn, DateTime checkOut, string status)
        {
            if (!IsAdmin())
                return Unauthorized();

            var booking = _bookingService.dbset()
                .Include(b => b.Room)
                .Include(b => b.User)
                .FirstOrDefault(b => b.Id == id);

                if (booking == null)
                    return NotFound();

                model.Booking = booking;
                model.Error = (string)null;

                if (checkIn >= checkOut)
            {
                model.Error = "Check-out date must be after check-in date.";
                return View(model);
            }

            if (string.IsNullOrEmpty(status))
            {
                model.Error = "Status is required.";
                return View(model);
            }

            booking.CheckIn = checkIn;
            booking.CheckOut = checkOut;
            booking.Status = status;

            var nights = (checkOut - checkIn).Days;
            booking.TotalPrice = nights * booking.Room.PricePerNight;

            _bookingService.Update(booking);
            _bookingService.SaveChanges();

            return RedirectToAction("Bookings");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteBooking(int id)
        {
            if (!IsAdmin())
                return Unauthorized();

            _bookingService.Delete(id);
            _bookingService.SaveChanges();

            return RedirectToAction("Bookings");
        }

        public IActionResult Users()
        {
            if (!IsAdmin())
                return Unauthorized();

            var users = _userService.GetAll();
            return View(users);
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            if (!IsAdmin())
                return Unauthorized();

            model.Error = (string)null;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateUser(string fullName, string email, string password, string role)
        {
            if (!IsAdmin())
                return Unauthorized();

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

            if (role != "Admin" && role != "Guest")
            {
                model.Error = "Role must be Admin or Guest.";
                return View(model);
            }

            var existing = _userService.Find(u => u.Email == email).FirstOrDefault();
            if (existing != null)
            {
                model.Error = "A user with this email already exists.";
                return View(model);
            }

            var user = new User
            {
                FullName = fullName,
                Email = email,
                Password = BCrypt.Net.BCrypt.HashPassword(password),
                Role = role,
                CreatedAt = DateTime.Now
            };

            _userService.Insert(user);
            _userService.SaveChanges();

            return RedirectToAction("Users");
        }

        [HttpGet]
        public IActionResult EditUser(int id)
        {
            if (!IsAdmin())
                return Unauthorized();

            var user = _userService.GetById(id);
            if (user == null)
                return NotFound();

            model.User = user;
            model.Error = (string)null;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditUser(int id, string fullName, string email, string role)
        {
            if (!IsAdmin())
                return Unauthorized();

            var user = _userService.GetById(id);
            if (user == null)
                return NotFound();

            model.User = user;
            model.Error = (string)null;

            fullName = InputValidator.Sanitize(fullName);
            email = InputValidator.Sanitize(email);

            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email))
            {
                model.Error = "Name and email are required.";
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

            if (role != "Admin" && role != "Guest")
            {
                model.Error = "Role must be Admin or Guest.";
                return View(model);
            }

            var duplicate = _userService.Find(u => u.Email == email && u.Id != id).FirstOrDefault();
            if (duplicate != null)
            {
                model.Error = "Another user with this email already exists.";
                return View(model);
            }

            user.FullName = fullName;
            user.Email = email;
            user.Role = role;

            _userService.Update(user);
            _userService.SaveChanges();

            return RedirectToAction("Users");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteUser(int id)
        {
            if (!IsAdmin())
                return Unauthorized();

            _userService.Delete(id);
            _userService.SaveChanges();

            return RedirectToAction("Users");
        }
    }
}
