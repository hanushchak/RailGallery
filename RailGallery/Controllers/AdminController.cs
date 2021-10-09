using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RailGallery.Data;
using RailGallery.Models;
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
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(ILogger<AdminController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
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

        public async Task<IActionResult> Users(string username, string sortOrder)
        {
            ViewBag.SearchString = username;

            ViewBag.UsernameSort = String.IsNullOrEmpty(sortOrder) ? "username_desc" : "";
            ViewBag.EmailSort = sortOrder == "Email" ? "email_desc" : "Email";
            ViewBag.RegisteredSort = sortOrder == "Registered" ? "registered_desc" : "Registered";
            ViewBag.LastActiveSort = sortOrder == "LastActive" ? "lastactive_desc" : "LastActive";
            ViewBag.NumberPhotosSort = sortOrder == "NumberPhotos" ? "numberphotos_desc" : "NumberPhotos";
            ViewBag.NumberCommentsSort = sortOrder == "NumberComments" ? "numbercomments_desc" : "NumberComments";

            var users = await _userManager.Users.Include(u => u.Images).Include(u => u.Comments).ToListAsync();

            if (!String.IsNullOrEmpty(username))
            {
                users = users.Where(u => u.UserName.Contains(username) || u.Email.Contains(username)).ToList();
            }

            switch(sortOrder)
            {
                case "username_desc" :
                    users = users.OrderByDescending(u => u.UserName).ToList();
                    break;
                case "Email":
                    users = users.OrderBy(u => u.Email).ToList();
                    break;
                case "email_desc":
                    users = users.OrderByDescending(u => u.Email).ToList();
                    break;
                case "Registered":
                    users = users.OrderBy(u => u.RegisterationDate).ToList();
                    break;
                case "registered_desc":
                    users = users.OrderByDescending(u => u.RegisterationDate).ToList();
                    break;
                case "LastActive":
                    users = users.OrderBy(u => u.LastActivityDate).ToList();
                    break;
                case "lastactive_desc":
                    users = users.OrderByDescending(u => u.LastActivityDate).ToList();
                    break;
                case "NumberPhotos":
                    users = users.OrderBy(u => u.Images.Count).ToList();
                    break;
                case "numberphotos_desc":
                    users = users.OrderByDescending(u => u.Images.Count).ToList();
                    break;
                case "NumberComments":
                    users = users.OrderBy(u => u.Comments.Count).ToList();
                    break;
                case "numbercomments_desc":
                    users = users.OrderByDescending(u => u.Comments.Count).ToList();
                    break;
                default:
                    users = users.OrderBy(u => u.UserName).ToList();
                    break;
            }

            return View(users);
        }
    }
}
