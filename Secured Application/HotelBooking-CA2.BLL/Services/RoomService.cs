using HotelBooking_CA2.Context;
using HotelBooking_CA2.Interfaces;
using HotelBooking_CA2.Models;

namespace HotelBooking_CA2.Services
{

    public class RoomService : GenericRepository<Room>, IRoomService
    {
        public RoomService(OTA_APP_DBContext context) : base(context)
        {
        }

    }
}
