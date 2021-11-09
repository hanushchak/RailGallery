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
    /// <summary>
    /// <c>View Controller</c>
    /// Contains a method that allows to retrieve information and the file of the reuqested photo and pass it to the Photo View page to display.
    /// 
    /// Author: Maksym Hanushchak
    /// </summary>
    public class ViewController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Class constructor.
        /// </summary>
        /// <param name="context">Reference to the current context.</param>
        /// <param name="userManager">Reference to the current User Manager object.</param>
        public ViewController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// HTTP GET method that retrieves information about the requested image and the file and redirects to the Photo View to display it.
        /// 
        /// GET: [host]/View{id parameter}
        /// </summary>
        /// <param name="id">ID of the photo to view.</param>
        /// <returns>Redirects to the View page of the requested photo.</returns>
        [Route("View/{id}")]
        public async Task<IActionResult> View(int? id)
        {
            // Try to retrieve the currently athenticated user
            ApplicationUser currentUser = await _userManager.GetUserAsync(HttpContext.User);

            // Retrieve a list of user roles of the current user if authenticated
            IList<string> currentUserRoles = null;
            if (currentUser != null)
            {
                currentUserRoles = await _userManager.GetRolesAsync(currentUser);
            }

            if (id == null)
            {
                return NotFound();
            }

            // Retrieve the requested image object and all corresponding objects that are related to this image
            Image image = await _context.Images
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

            // Boolean markers to indicate different conditions
            bool userLoggedIn = currentUser != null;
            bool imagePending = image.ImageStatus == Enums.Status.Pending;
            bool imageIsPrivate = image.ImagePrivacy == Enums.Privacy.Private;
            bool userIsAuthor = userLoggedIn && image.ApplicationUser.UserName.Equals(currentUser.UserName);
            bool userIsModerator = userLoggedIn && currentUserRoles.Contains(Enums.Roles.Moderator.ToString());
            bool imageRejected = image.ImageStatus == Enums.Status.Rejected;

            // Use the boolean markers above to check if the user has the permission to view the requested image
            // - Image does not exist - Display error
            // - Image is Pending and the user is not the author nor a moderator - Display error
            // - Image is Rejected and the user is not the author nor a moderator - Display error
            // - Image is Private and the user is not the author - Display error
            if ((image == null) ||
                    (imagePending && !(userIsAuthor || userIsModerator)) ||
                    (imageRejected && !(userIsAuthor || userIsModerator)) ||
                    (imageIsPrivate && !userIsAuthor))
            {
                return NotFound();
            }

            // Pass the boolean markers to the view for conditional rendering
            ViewBag.isPending = imagePending;
            ViewBag.isPrivate = imageIsPrivate;
            ViewBag.isRejected = imageRejected;
            ViewBag.isLiked = false;
            ViewBag.isFavorited = false;

            // Check if the current user has liked and/or favorited the image and pass the results as a boolean value to the view using a ViewBag
            if (currentUser != null)
            {
                // Check if the current user has liked the viewed image
                if (_context.Likes.Any(l => l.ApplicationUser.UserName == currentUser.UserName && l.Image.ImageID == image.ImageID))
                {
                    ViewBag.isLiked = true;
                }
                // Check if the current user has favorited the viewed image
                if (_context.Favorites.Any(f => f.ApplicationUser.UserName == currentUser.UserName && f.Image.ImageID == image.ImageID))
                {
                    ViewBag.isFavorited = true;
                }
            }

            // Retrieve the current date time in the EST time zone
            DateTime currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));

            // Create a new Image View object
            ImageView imageView = new ImageView { DateViewed = currentTime, ImageId = image.ImageID };

            // Add the new Image View object to the dtabase - to keep track of the number of image views
            await _context.ImageViews.AddAsync(imageView);
            await _context.SaveChangesAsync();

            // Redirect to the Photo View page
            return View(image);
        }
    }
}
