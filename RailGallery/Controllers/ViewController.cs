using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RailGallery.Data;
using RailGallery.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RailGallery.Controllers
{
    public class ViewController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ViewController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: View/5
        [Route("View/{id}")]
        public async Task<IActionResult> View(int? id)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            IList<string> currentUserRoles = null;
            if (currentUser != null)
            {
                currentUserRoles = await _userManager.GetRolesAsync(currentUser);
            }

            if (id == null)
            {
                return NotFound();
            }

            var image = await _context.Images
                .Include(m => m.Comments.OrderBy(c => c.CommentDate)).ThenInclude(c => c.ApplicationUser)
                .Include(m => m.Likes)
                .Include(m => m.Albums).ThenInclude(u => u.ApplicationUser)
                .Include(m => m.Favorites)
                .Include(m => m.Category)
                .Include(m => m.ApplicationUser)
                .Include(m => m.Location)
                .Include(m => m.Locomotive)
                .Include(m => m.ImageViews)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ImageID == id);

            bool userLoggedIn = currentUser != null;
            bool imagePending = image.ImageStatus == Enums.Status.Pending;
            bool imageIsPrivate = image.ImagePrivacy == Enums.Privacy.Private;
            bool userIsAuthor = userLoggedIn && image.ApplicationUser.UserName.Equals(currentUser.UserName);
            bool userIsModerator = userLoggedIn && currentUserRoles.Contains(Enums.Roles.Moderator.ToString());
            bool imageRejected = image.ImageStatus == Enums.Status.Rejected;

            // Only view if has the permissions
            if ((image == null) ||
                    (imagePending && !(userIsAuthor || userIsModerator)) ||
                    (imageRejected && !(userIsAuthor || userIsModerator)) ||
                    (imageIsPrivate && !userIsAuthor))
            {
                return NotFound();
            }

            ViewBag.isPending = imagePending;
            ViewBag.isPrivate = imageIsPrivate;
            ViewBag.isRejected = imageRejected;
            ViewBag.isLiked = false;
            ViewBag.isFavorited = false;

            if (currentUser != null)
            {
                if (_context.Likes.Any(l => l.ApplicationUser.UserName == currentUser.UserName && l.Image.ImageID == image.ImageID))
                {
                    ViewBag.isLiked = true;
                }
                if (_context.Favorites.Any(f => f.ApplicationUser.UserName == currentUser.UserName && f.Image.ImageID == image.ImageID))
                {
                    ViewBag.isFavorited = true;
                }
            }

            DateTime currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
            ImageView imageView = new ImageView { DateViewed = currentTime, ImageId = image.ImageID };
            await _context.ImageViews.AddAsync(imageView);
            await _context.SaveChangesAsync();

            return View(image);
        }
    }
}
