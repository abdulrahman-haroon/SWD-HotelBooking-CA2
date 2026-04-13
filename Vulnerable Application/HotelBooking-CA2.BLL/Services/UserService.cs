using HotelBooking_CA2.Context;
using HotelBooking_CA2.Interfaces;
using HotelBooking_CA2.Models;

namespace HotelBooking_CA2.Services
{

    public class UserService : GenericRepository<User>, IUserService
    {
        public UserService(OTA_APP_DBContext context) : base(context)
        {
        }

    }
}
