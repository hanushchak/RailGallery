using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RailGallery.Data;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace RailGallery.Controllers
{
    [Authorize(Roles = "Moderator")]
    public class AdminController : Controller
    {

        private readonly ILogger<AdminController> _logger;
        private readonly ApplicationDbContext _context;

        public AdminController(ILogger<AdminController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            dynamic adminModel = new ExpandoObject();

            // Most recent images
            adminModel.PendingImages = _context.Images
                .Where(i => i.ImageStatus == Enums.Status.Pending && i.ImagePrivacy != Enums.Privacy.Private)
                .OrderBy(i => i.ImageUploadedDate)
                .Take(15)
                .Include(c => c.ApplicationUser)
                .AsNoTracking();

            adminModel.RecentComments = _context.Comments
                .OrderByDescending(c => c.CommentDate)
                .Take(15)
                .Include(c => c.ApplicationUser)
                .Include(c => c.Image)
                .AsNoTracking();

            return View(adminModel);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReviewPhoto([Bind("decision,ImageID")]IFormCollection collection)
        {
            string imageID = collection["ImageID"];

            if (String.IsNullOrEmpty(imageID))
            {
                return Content("Error");
            }
            
            var image = await _context.Images.FirstOrDefaultAsync(m => m.ImageID.ToString() == imageID);

            if (collection["decision"] == "Reject")
            {
                image.ImageStatus = Enums.Status.Rejected;
                _context.Update(image);
                await _context.SaveChangesAsync();

                return RedirectToAction("View", "View", new { @id = image.ImageID });
            }

            if (collection["decision"] == "Approve")
            {
                image.ImageStatus = Enums.Status.Published;
                _context.Update(image);
                await _context.SaveChangesAsync();

                return RedirectToAction("View", "View", new { @id = image.ImageID });
            }


            return Content("Error");
        }
    }
}
