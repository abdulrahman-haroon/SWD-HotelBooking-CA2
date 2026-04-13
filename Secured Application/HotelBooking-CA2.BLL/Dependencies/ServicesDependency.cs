using HotelBooking_CA2.Context;
using HotelBooking_CA2.Interfaces;
using HotelBooking_CA2.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HotelBooking_CA2.Dependencies
{
    public static class ServicesDependency
    {
        public static void AddServicesDependency(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<OTA_APP_DBContext>(opts => opts.UseSqlServer(configuration.GetConnectionString("BedBankDb"), providerOptions => providerOptions.EnableRetryOnFailure()));

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IRoomService, RoomService>();
            services.AddScoped<IReviewService, ReviewService>();
        }
    }
}
