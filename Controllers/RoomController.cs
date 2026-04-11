using HotelBooking_CA2.Interfaces;
using HotelBooking_CA2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;

namespace HotelBooking_CA2.Controllers
{
    public class RoomController : Controller
    {
        private readonly IRoomService _roomService;
        private readonly IReviewService _reviewService;
        private readonly dynamic model;

        public RoomController(IRoomService roomService, IReviewService reviewService)
        {
            _roomService = roomService;
            _reviewService = reviewService;
            model = new ExpandoObject();
        }

        public IActionResult Index()
        {
            var rooms = _roomService.Find(r => r.IsAvailable);
            return View(rooms);
        }

        public IActionResult Details(int id)
        {
            var room = _roomService.GetById(id);
            if (room == null)
                return RedirectToAction("Index");

            var reviews = _reviewService.dbset()
                .Include(r => r.User)
                .Where(r => r.RoomId == id)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

            model.Room = room;
            model.Reviews = reviews;
            model.Error = (string)null;
            return View(model);
        }

        [HttpPost]
        public IActionResult AddReview(int roomId, int rating, string comment)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var room = _roomService.GetById(roomId);
            if (room == null)
                return RedirectToAction("Index");

            if (rating < 1 || rating > 5 || string.IsNullOrEmpty(comment))
            {
                var reviews = _reviewService.dbset()
                    .Include(r => r.User)
                    .Where(r => r.RoomId == roomId)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToList();

                model.Room = room;
                model.Reviews = reviews;
                model.Error = "Please provide a rating (1-5) and a comment.";
                return View("Details", model);
            }

            var review = new Review
            {
                UserId = userId.Value,
                RoomId = roomId,
                Rating = rating,
                Comment = comment,
                CreatedAt = DateTime.Now
            };

            _reviewService.Insert(review);
            _reviewService.SaveChanges();

            return RedirectToAction("Details", new { id = roomId });
        }
    }
}
