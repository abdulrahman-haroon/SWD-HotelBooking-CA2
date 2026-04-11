using HotelBooking_CA2.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking_CA2.Controllers
{
    public class RoomController : Controller
    {
        private readonly IRoomService _roomService;

        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        public IActionResult Index()
        {
            var rooms = _roomService.Find(r => r.IsAvailable);
            return View(rooms);
        }
    }
}
