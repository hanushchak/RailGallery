using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RailGallery.Data;
using RailGallery.Models;

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
                .Include(m => m.Comments)
                .Include(m => m.Likes)
                .Include(m => m.Albums)
                .Include(m => m.Favorites)
                .Include(m => m.Category)
                .Include(m => m.ApplicationUser)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ImageID == id);

            bool userLoggedIn = currentUser != null;
            bool imagePending = image.ImageStatus == Enums.Status.Pending;
            bool imageIsPrivate = image.ImagePrivacy == Enums.Privacy.Private;
            bool userIsAuthor = userLoggedIn && image.ApplicationUser.UserName.Equals(currentUser.UserName);
            bool userIsModerator = userLoggedIn && currentUserRoles.Contains(Enums.Roles.Moderator.ToString());

            // Only view if has the permissions
            if ((image == null) ||
                    (imagePending && !(userIsAuthor || userIsModerator)) ||
                    (imageIsPrivate && !userIsAuthor))
            {
                return NotFound();
            }

            return View(image);
        }

        // POST: View/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("View/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostComment([Bind("CommentText")] Comment comment)
        {
            if (!ModelState.IsValid)
            {
                return Content("Error");
            }
            comment.CommentDate = DateTime.UtcNow;
            _context.Add(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction("View", new { @id = 32 });
        }

        // GET: View/Edit/5
        [Authorize(Roles = "Moderator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var image = await _context.Images.FindAsync(id);
            if (image == null)
            {
                return NotFound();
            }
            return View(image);
        }

        // POST: View/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Moderator")]
        public async Task<IActionResult> Edit(int id, [Bind("ImageID,ImageTitle,ImageDescription,ImageMetadata,ImageTakenDate,ImageUploadedDate,ImageStatus,ImagePrivacy,ImagePath")] Image image)
        {
            if (id != image.ImageID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(image);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ImageExists(image.ImageID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(image);
        }

        // GET: View/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var image = await _context.Images
                .FirstOrDefaultAsync(m => m.ImageID == id);
            if (image == null)
            {
                return NotFound();
            }

            return View(image);
        }

        // POST: View/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var image = await _context.Images.FindAsync(id);
            _context.Images.Remove(image);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ImageExists(int id)
        {
            return _context.Images.Any(e => e.ImageID == id);
        }
    }
}
