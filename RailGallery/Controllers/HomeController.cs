using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RailGallery.Data;
using RailGallery.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace RailGallery.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            dynamic homeModel = new ExpandoObject();

            // Most recent images
            homeModel.LasestImages = _context.Images
                .Where(i => i.ImageStatus == Enums.Status.Published && i.ImagePrivacy != Enums.Privacy.Private)
                .OrderByDescending(i => i.ImageUploadedDate)
                .Take(20)
                .Include(c => c.Comments)
                .Include(c => c.Likes)
                .Include(c => c.ApplicationUser)
                .AsNoTracking();

            homeModel.Top24HoursImages = _context.Images
                .Where(i => i.ImageStatus == Enums.Status.Published && i.ImagePrivacy != Enums.Privacy.Private)
                .OrderByDescending(i => i.ImageUploadedDate)
                .Take(10)
                .Include(c => c.Comments)
                .Include(c => c.Likes)
                .Include(c => c.ApplicationUser)
                .AsNoTracking();

            return View(homeModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
