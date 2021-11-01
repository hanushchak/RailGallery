using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RailGallery.Data;
using RailGallery.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace RailGallery.Controllers
{
    /// <summary>
    /// <c>Top Controller</c>
    /// Contains a method that allows to retrieve a list of the most viewed photos in a specified time range.
    /// 
    /// Author: Maksym Hanushchak 
    /// </summary>
    public class TopController : Controller
    {

        private readonly ILogger<TopController> _logger;
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Class constructor.
        /// </summary>
        /// <param name="logger">Reference to the current Logger object.</param>
        /// <param name="context">Reference to the current context.</param>
        public TopController(ILogger<TopController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// HTTP GET method that generates a list of the most viewed photos in a specified time range upon calling.
        /// </summary>
        /// <param name="time">Time range (values: 24hours, week, month).</param>
        /// <param name="page">The page number of the page to display.</param>
        /// <returns></returns>
        public async Task<IActionResult> Index(string time, int? page)
        {
            // Create a list to store the images retrieved below
            List<Image> images = null;

            // Retrieve the current date and time in the EST time zone
            DateTime currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));

            // If the requested time range is 24 hours, retrieve the most viewed images in the past 24 hours from the current date
            if (String.IsNullOrEmpty(time) || time.ToLower() == "24hours")
            {
                images = await _context.Images
                .Where(i => i.ImageStatus == Enums.Status.Published && i.ImagePrivacy != Enums.Privacy.Private)
                .Include(i => i.ImageViews.Where(v => v.DateViewed >= currentTime.AddHours(-24)))
                .OrderByDescending(i => i.ImageViews.Where(v => v.DateViewed >= currentTime.AddHours(-24)).Count()).ThenByDescending(i => i.ImageUploadedDate)
                .Include(c => c.Likes)
                .Include(c => c.Comments)
                .Include(c => c.ApplicationUser)
                .AsNoTracking()
                .ToListAsync();
            }
            // If the requested time range is a week, retrieve the most viewed images in the past week from the current date
            else if (time.ToLower() == "week")
            {
                images = await _context.Images
                .Where(i => i.ImageStatus == Enums.Status.Published && i.ImagePrivacy != Enums.Privacy.Private)
                .Include(i => i.ImageViews.Where(v => v.DateViewed >= currentTime.AddHours(-168)))
                .OrderByDescending(i => i.ImageViews.Where(v => v.DateViewed >= currentTime.AddHours(-168)).Count()).ThenByDescending(i => i.ImageUploadedDate)
                .Include(c => c.Likes)
                .Include(c => c.Comments)
                .Include(c => c.ApplicationUser)
                .AsNoTracking()
                .ToListAsync();
            }
            // If the requested time range is a month, retrieve the most viewed images in the past month from the current date
            else if (time.ToLower() == "month")
            {
                images = await _context.Images
                .Where(i => i.ImageStatus == Enums.Status.Published && i.ImagePrivacy != Enums.Privacy.Private)
                .Include(i => i.ImageViews.Where(v => v.DateViewed >= currentTime.AddHours(-730)))
                .OrderByDescending(i => i.ImageViews.Where(v => v.DateViewed >= currentTime.AddHours(-730)).Count()).ThenByDescending(i => i.ImageUploadedDate)
                .Include(c => c.Likes)
                .Include(c => c.Comments)
                .Include(c => c.ApplicationUser)
                .AsNoTracking()
                .ToListAsync();
            }
            // If the parameter is not recognized, return the Not Found error
            else
            {
                return NotFound();
            }

            // Store the requested time range in a view bag to send back and display in the view
            ViewBag.Time = time;

            // Specify the number of images on a single page
            int pageSize = 15;

            // Calculate the current page number
            int pageNumber = (int)((!page.HasValue || page == 0) ? 1 : page);

            // Redirect to the view
            return View(images.ToPagedList(pageNumber, pageSize));
        }
    }
}
