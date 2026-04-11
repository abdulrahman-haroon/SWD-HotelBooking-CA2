using HotelBooking_CA2.Interfaces;
using HotelBooking_CA2.Models;
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
        private readonly dynamic model;

        public AdminController(IRoomService roomService, IBookingService bookingService, IUserService userService)
        {
            _roomService = roomService;
            _bookingService = bookingService;
            _userService = userService;
            model = new ExpandoObject();
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("UserRole") == "Admin";
        }

        public IActionResult Index()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

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
                return RedirectToAction("Login", "Account");

            var rooms = _roomService.GetAll();
            return View(rooms);
        }

        [HttpGet]
        public IActionResult CreateRoom()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            model.Error = (string)null;
            return View(model);
        }

        [HttpPost]
        public IActionResult CreateRoom(string roomNumber, string roomType, string description, decimal pricePerNight, int capacity, bool isAvailable, string imageUrl)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            model.Error = (string)null;

            if (string.IsNullOrEmpty(roomNumber) || string.IsNullOrEmpty(roomType))
            {
                model.Error = "Room number and type are required.";
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
                return RedirectToAction("Login", "Account");

            var room = _roomService.GetById(id);
            if (room == null)
                return RedirectToAction("Rooms");

            model.Room = room;
            model.Error = (string)null;
            return View(model);
        }

        [HttpPost]
        public IActionResult EditRoom(int id, string roomNumber, string roomType, string description, decimal pricePerNight, int capacity, bool isAvailable, string imageUrl)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var room = _roomService.GetById(id);
            if (room == null)
                return RedirectToAction("Rooms");

            model.Room = room;
            model.Error = (string)null;

            if (string.IsNullOrEmpty(roomNumber) || string.IsNullOrEmpty(roomType))
            {
                model.Error = "Room number and type are required.";
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
        public IActionResult DeleteRoom(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            _roomService.Delete(id);
            _roomService.SaveChanges();

            return RedirectToAction("Rooms");
        }

        public IActionResult Bookings()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var bookings = _bookingService.dbset()
                .Include(b => b.Room)
                .Include(b => b.User)
                .OrderByDescending(b => b.BookedAt)
                .ToList();

            return View(bookings);
        }
    }
}
