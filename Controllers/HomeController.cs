using HotelBooking_CA2.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HotelBooking_CA2.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Route("/Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            ViewData["StatusCode"] = statusCode;

            if (statusCode == 404)
            {
                ViewData["Message"] = "The page you are looking for could not be found.";
            }
            else
            {
                ViewData["Message"] = "Something went wrong. Please try again later.";
            }

            return View("NotFound");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
