using HotelBooking_CA2.Context;
using HotelBooking_CA2.Interfaces;
using HotelBooking_CA2.Models;

namespace OTA.BLL.Services
{

    public class ReviewService : GenericRepository<Review>, IReviewService
    {
        public ReviewService(OTA_APP_DBContext context) : base(context)
        {
        }

    }
}
