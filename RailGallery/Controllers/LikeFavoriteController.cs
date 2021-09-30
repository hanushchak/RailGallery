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
    [Authorize]
    public class LikeFavoriteController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public LikeFavoriteController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Route("Saved/{type}")]
        public async Task<ActionResult> Index(string type, int? page)
        {
            List<Image> images = null;
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            if (String.IsNullOrEmpty(type) || type.ToLower() == "liked")
            {
                ViewBag.Title = "Photos You've Liked";
                images = await _context.Images
                .Where(i => i.ImageStatus == Enums.Status.Published && i.ImagePrivacy != Enums.Privacy.Private && i.Likes.Where(l=>l.ApplicationUser.UserName == currentUser.UserName).Any())
                .OrderByDescending(i => i.Likes.FirstOrDefault().LikeID)
                .Include(c => c.Comments)
                .Include(c => c.Likes)
                .Include(c => c.ApplicationUser)
                .AsNoTracking()
                .ToListAsync();
            }
            else if (type.ToLower() == "favorite")
            {
                ViewBag.Title = "Your Favoirte Photos";
                images = await _context.Images
                .Where(i => i.ImageStatus == Enums.Status.Published && i.ImagePrivacy != Enums.Privacy.Private && i.Favorites.Where(l => l.ApplicationUser.UserName == currentUser.UserName).Any())
                .OrderByDescending(i => i.Favorites.FirstOrDefault().FavoriteID)
                .Include(c => c.Comments)
                .Include(c => c.Likes)
                .Include(c => c.ApplicationUser)
                .AsNoTracking()
                .ToListAsync();
            }
            else
            {
                return NotFound();
            }

            ViewBag.Action = type;

            int pageSize = 15; // TODO

            int pageNumber = (int)((!page.HasValue || page == 0) ? 1 : page);

            return View(images.ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LikeOrFavorite([Bind("action,ImageID")] IFormCollection collection)
        {
            string imageID = collection["ImageID"];
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            if (String.IsNullOrEmpty(imageID) || currentUser == null)
            {
                return Content("Error");
            }

            var image = await _context.Images.FirstOrDefaultAsync(m => m.ImageID.ToString() == imageID);

            if (collection["action"] == "Like")
            {
                if(_context.Likes.Any(l => l.ApplicationUser.UserName == currentUser.UserName && l.Image.ImageID == image.ImageID))
                {
                    var existingLike = await _context.Likes.FirstOrDefaultAsync(l => l.ApplicationUser.UserName == currentUser.UserName && l.Image.ImageID == image.ImageID);
                    _context.Likes.Remove(existingLike);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("View", "View", new { @id = image.ImageID });
                } 
                else
                {
                    Like like = new Like
                    {
                        ApplicationUser = currentUser,
                        Image = image
                    };

                    _context.Add(like);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("View", "View", new { @id = image.ImageID });
                }
            }

            if (collection["action"] == "Favorite")
            {
                if (_context.Favorites.Any(f => f.ApplicationUser.UserName == currentUser.UserName && f.Image.ImageID == image.ImageID))
                {
                    var existingFavorite = await _context.Favorites.FirstOrDefaultAsync(f => f.ApplicationUser.UserName == currentUser.UserName && f.Image.ImageID == image.ImageID);
                    _context.Favorites.Remove(existingFavorite);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("View", "View", new { @id = image.ImageID });

                }
                else
                {
                    Favorite favorite = new Favorite
                    {
                        ApplicationUser = currentUser,
                        Image = image
                    };

                    _context.Add(favorite);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("View", "View", new { @id = image.ImageID });
                }
            }
            return Content("Error");
        }
    }
}
