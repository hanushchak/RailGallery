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

namespace RailGallery.Controllers
{
    public class LikeFavoriteController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public LikeFavoriteController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        [HttpPost]
        [Authorize]
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
