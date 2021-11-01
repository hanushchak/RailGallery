using Microsoft.AspNetCore.Authorization;
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
using X.PagedList;

namespace RailGallery.Controllers
{
    /// <summary>
    /// <c>Like/Favorite Controller</c>
    /// Contains methods that allow add and remove photos to/from the Liked and Favorited collections of the user.
    /// 
    /// Author: Maksym Hanushchak
    /// </summary>
    [Authorize]
    public class LikeFavoriteController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Class constructor.
        /// </summary>
        /// <param name="context">Reference to the current context.</param>
        /// <param name="userManager">Reference to the User Manager object.</param>
        public LikeFavoriteController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// HTTP GET method that retrieves the user's liked and favorited photos and redirects the user to the view.
        /// 
        /// /// GET: [host]/Saved/{type parameter}
        /// </summary>
        /// <param name="type">Type of the photos to retrieve (liked or favorited).</param>
        /// <param name="page">The current page number.</param>
        /// <returns>Redirects to the view.</returns>
        [Route("Saved/{type}")]
        public async Task<ActionResult> Index(string type, int? page)
        {
            // Declare a list of Image objects to store the retrieved below photos
            List<Image> images = null;

            // Retrieve the currently authenticated user
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            // If the requested type are liked photos, retrieve the user's liked photos and store them in the list
            if (String.IsNullOrEmpty(type) || type.ToLower() == "liked")
            {
                // The title to display on the view page
                ViewBag.Title = "Photos You've Liked";
                // Retrieve the liked images of the current user
                images = await _context.Images
                .Where(i => i.ImageStatus == Enums.Status.Published && i.ImagePrivacy != Enums.Privacy.Private && i.Likes.Where(l => l.ApplicationUser.UserName == currentUser.UserName).Any())
                .OrderByDescending(i => i.Likes.FirstOrDefault().LikeID)
                .Include(c => c.Comments)
                .Include(c => c.Likes)
                .Include(c => c.ApplicationUser)
                .AsNoTracking()
                .ToListAsync();
            }
            // If the requested type are favorited photos, retrieve the user's favorited photos and store them in the list 
            else if (type.ToLower() == "favorite")
            {
                // The title to display on the view page
                ViewBag.Title = "Your Favoirte Photos";
                // Retrieve the favorited images of the current user
                images = await _context.Images
                .Where(i => i.ImageStatus == Enums.Status.Published && i.ImagePrivacy != Enums.Privacy.Private && i.Favorites.Where(l => l.ApplicationUser.UserName == currentUser.UserName).Any())
                .OrderByDescending(i => i.Favorites.FirstOrDefault().FavoriteID)
                .Include(c => c.Comments)
                .Include(c => c.Likes)
                .Include(c => c.ApplicationUser)
                .AsNoTracking()
                .ToListAsync();
            }
            // If none of these parameters matched, display the Not Found error
            else
            {
                return NotFound();
            }

            // Send back the type filter name
            ViewBag.Action = type;

            // Display 15 images on a single page
            int pageSize = 15;

            // Ternary operator to calculate the current page number
            int pageNumber = (int)((!page.HasValue || page == 0) ? 1 : page);

            // Redirect to the Liked/Favorited view
            return View(images.ToPagedList(pageNumber, pageSize));
        }

        /// <summary>
        /// HTTP POST method that allows to add and remove photos form the user's liked and favorited collections.
        /// Called upon clicking on the Like Photo / Favorite Photo buttons in the view.
        /// 
        /// /// POST: [host]/LikeFavorite/LikedFavorited{FormCollection}
        /// </summary>
        /// <param name="collection">The data submitted with the form.</param>
        /// <returns>Redirects to the view.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LikeOrFavorite([Bind("action,ImageID")] IFormCollection collection)
        {
            // Retrieve the ID of the image to be liked / favorited
            string imageID = collection["ImageID"];

            // Retireve the currently authenticated user
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            if (String.IsNullOrEmpty(imageID) || currentUser == null)
            {
                return Content("Error");
            }

            // Retrieve the Image object from the database
            var image = await _context.Images.FirstOrDefaultAsync(m => m.ImageID.ToString() == imageID);

            // If the submitted action is to like the image...
            if (collection["action"] == "Like")
            {
                // Check if the image has already been added to the liked collection of this user, if so - remove the image from the Liked collection
                if (_context.Likes.Any(l => l.ApplicationUser.UserName == currentUser.UserName && l.Image.ImageID == image.ImageID))
                {
                    // Find the like object in the database for this user and image
                    var existingLike = await _context.Likes.FirstOrDefaultAsync(l => l.ApplicationUser.UserName == currentUser.UserName && l.Image.ImageID == image.ImageID);
                    // Remove the like object from the database
                    _context.Likes.Remove(existingLike);
                    await _context.SaveChangesAsync();
                    // Redirect back to the view
                    return RedirectToAction("View", "View", new { @id = image.ImageID });
                }
                // If the image hasn't been liked already, add the like for the current user - image
                else
                {
                    // Create a new like object for the current user and image
                    Like like = new Like
                    {
                        ApplicationUser = currentUser,
                        Image = image
                    };

                    // Add the like object to the database
                    _context.Add(like);
                    await _context.SaveChangesAsync();
                    // Redirect back to the view
                    return RedirectToAction("View", "View", new { @id = image.ImageID });
                }
            }
            // If the submitted action is to favorite the image...
            if (collection["action"] == "Favorite")
            {
                // Check if the image has already been added to the favorites collection of this user, if so - remove the image from the Favorites collection
                if (_context.Favorites.Any(f => f.ApplicationUser.UserName == currentUser.UserName && f.Image.ImageID == image.ImageID))
                {
                    // Find the Favorite object in the database for this user and image
                    var existingFavorite = await _context.Favorites.FirstOrDefaultAsync(f => f.ApplicationUser.UserName == currentUser.UserName && f.Image.ImageID == image.ImageID);
                    // Remove the like object from the database
                    _context.Favorites.Remove(existingFavorite);
                    await _context.SaveChangesAsync();
                    // Redirect back to the view
                    return RedirectToAction("View", "View", new { @id = image.ImageID });

                }
                // If the image hasn't been favorited already, add the like for the current user - image
                else
                {
                    // Create a new Favorite object for the current user and image
                    Favorite favorite = new Favorite
                    {
                        ApplicationUser = currentUser,
                        Image = image
                    };
                    // Add the like object to the database
                    _context.Add(favorite);
                    await _context.SaveChangesAsync();
                    // Redirect back to the view
                    return RedirectToAction("View", "View", new { @id = image.ImageID });
                }
            }
            // If none of the conditional statements worked, return error message.
            return Content("Error");
        }
    }
}
