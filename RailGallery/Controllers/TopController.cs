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
    public class TopController : Controller
    {

        private readonly ILogger<TopController> _logger;
        private readonly ApplicationDbContext _context;

        public TopController(ILogger<TopController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index(string time, int? page)
        {
            List<Image> images = null;

            DateTime currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));

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
            else
            {
                return NotFound();
            }

            ViewBag.Time = time;

            int pageSize = 15;

            int pageNumber = (int)((!page.HasValue || page == 0) ? 1 : page);

            return View(images.ToPagedList(pageNumber, pageSize));
        }
    }
}
