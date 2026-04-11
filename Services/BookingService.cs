using HotelBooking_CA2.Context;
using HotelBooking_CA2.Interfaces;
using HotelBooking_CA2.Models;

namespace OTA.BLL.Services
{

    public class BookingService : GenericRepository<Booking>, IBookingService
    {
        public BookingService(OTA_APP_DBContext context) : base(context)
        {
        }

    }
}
