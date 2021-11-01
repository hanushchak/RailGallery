using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RailGallery.Data;
using RailGallery.Models;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System;

namespace RailGallery.Controllers
{
    /// <summary>
    /// <c>Home Controller</c>
    /// Contains methods that allow to retrieve and display most popular and recent photos on the home page.
    /// 
    /// Author: Maksym Hanushchak
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Class constructor.
        /// </summary>
        /// <param name="logger">Reference to the Logger object.</param>
        /// <param name="context">Reference to the Context object.</param>
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// HTTP GET method that retrieves 20 most recent photos and 10 photos most viewed in past 24 hours and sends the data to the corresponding view.
        /// 
        /// /// GET: [host]/
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            // Create ExpandoObject dynamic model that will be used to pass most recent and most viewed photos to the view
            dynamic homeModel = new ExpandoObject();

            // Retrieve the current date and time in the EST timezone
            DateTime currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));

            // Retrieve 20 most recent photos and add them to the dynamic model
            homeModel.LasestImages = _context.Images
                .Where(i => i.ImageStatus == Enums.Status.Published && i.ImagePrivacy != Enums.Privacy.Private)
                .OrderByDescending(i => i.ImageUploadedDate)
                .Take(20)
                .Include(c => c.Comments)
                .Include(c => c.Likes)
                .Include(c => c.ApplicationUser)
                .AsNoTracking();

            // Retrieve 10 most viewed photos and add them to the dynamic model
            homeModel.Top24HoursImages = _context.Images
                .Where(i => i.ImageStatus == Enums.Status.Published && i.ImagePrivacy != Enums.Privacy.Private)
                .Include(i => i.ImageViews.Where(v => v.DateViewed >= currentTime.AddHours(-24)))
                .OrderByDescending(i => i.ImageViews.Where(v => v.DateViewed >= currentTime.AddHours(-24)).Count()).ThenByDescending(i => i.ImageUploadedDate)
                .Take(10)
                .Include(c => c.Comments)
                .Include(c => c.Likes)
                .Include(c => c.ApplicationUser)
                .AsNoTracking();

            // Redirect to the Home view and pass the dynamic model
            return View(homeModel);
        }

        /// <summary>
        /// This method is called any time an error occurs.
        /// </summary>
        /// <returns>Redirects to the error view model.</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
