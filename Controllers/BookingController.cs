using HotelBooking_CA2.Interfaces;
using HotelBooking_CA2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;

namespace HotelBooking_CA2.Controllers
{
    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly IRoomService _roomService;
        private readonly dynamic model;

        public BookingController(IBookingService bookingService, IRoomService roomService)
        {
            _bookingService = bookingService;
            _roomService = roomService;
            model = new ExpandoObject();
        }

        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var bookings = _bookingService.dbset()
                .Include(b => b.Room)
                .Where(b => b.UserId == userId.Value)
                .ToList();
            return View(bookings);
        }

        [HttpGet]
        public IActionResult Create(int roomId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var room = _roomService.GetById(roomId);
            if (room == null)
                return NotFound();

            model.Error = (string)null;
            model.Room = room;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(int roomId, DateTime checkIn, DateTime checkOut)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var room = _roomService.GetById(roomId);
            if (room == null)
                return NotFound();

            model.Error = (string)null;
            model.Room = room;

            if (checkIn >= checkOut)
            {
                model.Error = "Check-out date must be after check-in date.";
                return View(model);
            }

            var nights = (checkOut - checkIn).Days;
            var totalPrice = nights * room.PricePerNight;

            var booking = new Booking
            {
                UserId = userId.Value,
                RoomId = roomId,
                CheckIn = checkIn,
                CheckOut = checkOut,
                TotalPrice = totalPrice,
                Status = "Confirmed",
                BookedAt = DateTime.Now
            };

            _bookingService.Insert(booking);
            _bookingService.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
