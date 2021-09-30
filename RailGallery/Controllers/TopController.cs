using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RailGallery.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RailGallery.Models;

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

        public async Task<IActionResult> Index(string? time)
        {
            List<Image> images = null;

            if (String.IsNullOrEmpty(time) || time.ToLower() == "24hour")
            {
                images = await _context.Images
                .Where(i => i.ImageStatus == Enums.Status.Published && i.ImagePrivacy != Enums.Privacy.Private)
                /*.Take(24)*/
                .OrderByDescending(i => i.ImageUploadedDate)
                .Include(c => c.Likes)
                .Include(c => c.Comments)
                .Include(c => c.ApplicationUser)
                .AsNoTracking()
                .ToListAsync();
            } else if (time.ToLower() == "week") 
            {
                images = await _context.Images
                .Where(i => i.ImageStatus == Enums.Status.Published && i.ImagePrivacy != Enums.Privacy.Private)
                /*.Take(24)*/
                .OrderByDescending(i => i.ImageUploadedDate)
                .Include(c => c.Likes)
                .Include(c => c.Comments)
                .Include(c => c.ApplicationUser)
                .AsNoTracking()
                .ToListAsync();
            } else if (time.ToLower() == "month")
            {
                images = await _context.Images
                .Where(i => i.ImageStatus == Enums.Status.Published && i.ImagePrivacy != Enums.Privacy.Private)
                /*.Take(24)*/
                .OrderByDescending(i => i.ImageUploadedDate)
                .Include(c => c.Likes)
                .Include(c => c.Comments)
                .Include(c => c.ApplicationUser)
                .AsNoTracking()
                .ToListAsync();
            } else
            {
                return NotFound();
            }   

            return View(images);
        }
    }
}
