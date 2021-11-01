using Microsoft.AspNetCore.Authorization;
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
    /// <c>Comments Controller</c>
    /// Contains a method that allows to create and delete comments and calls the corresponding views to display the comments.
    /// 
    /// Author: Maksym Hanushchak
    /// </summary>
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Class constructor.
        /// </summary>
        /// <param name="context">Reference to the Context object</param>
        /// <param name="userManager">Reference to the User Manager object</param>
        public CommentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// HTTP POST method to create a new comment and save it in the database. This method is called upon submission of the comments form.
        /// 
        /// POST: [host]/Comments/Create{FormCollection}
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CommentID,CommentText,CommentImageID")] Comment comment)
        {
            // Check if the submitted form data is valid
            if (ModelState.IsValid)
            {
                // Retrieve the photo to which the comment is posted and reference it in the new comment object
                comment.Image = await _context.Images.FirstOrDefaultAsync(i => i.ImageID == Convert.ToInt32(comment.CommentImageID));
                
                // Retrieve the currently authenticated user
                var currentUser = await _userManager.GetUserAsync(HttpContext.User);
                
                // If the user retrieval fails - display error
                if (currentUser == null)
                {
                    return Content("Error");
                }
                else // Save a reference to the comment author in the new comment object
                {
                    comment.ApplicationUser = currentUser;
                }

                // Retrieve the image to which the comment is added
                var image = await _context.Images.FirstOrDefaultAsync(m => m.ImageID == comment.Image.ImageID);

                // Retrieve the photo to which the comment is posted and reference it in the new comment object
                if (image == null)
                {
                    return Content("Error");
                }
                else
                {
                    comment.Image = image;
                }

                // Save the new comment in the database
                _context.Add(comment);
                await _context.SaveChangesAsync();

                // Redirect back to the image view
                return RedirectToAction("View", "View", new { @id = comment.Image.ImageID });
            }
            return Content("Error");
        }


        /// <summary>
        /// HTTP GET method that displays the comment removal page with a confirmation button and comment information.
        /// 
        /// GET: [host]/Cooments/Delete{id parameter}
        /// </summary>
        /// <param name="id">The id of the comment to delete.</param>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Retrieve the comment to be deleted and its metadata
            var comment = await _context.Comments
                .Include(c => c.Image)
                .Include(c => c.ApplicationUser)
                .FirstOrDefaultAsync(m => m.CommentID == id);

            if (comment == null)
            {
                return NotFound();
            }

            // Retrieve the currently authentcated user
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            // Declare a list to store user roles
            IList<string> currentUserRoles = null;

            if (currentUser != null)
            {
                // Retrieve the roles of the current user
                currentUserRoles = await _userManager.GetRolesAsync(currentUser);
            }

            // If the user has the permission to remove the comment, redirect to the comment deletion view
            if (currentUser.UserName != comment.ApplicationUser.UserName && !currentUserRoles.Contains(Enums.Roles.Moderator.ToString()))
            {
                return RedirectToAction("View", "View", new { @id = comment.Image.ImageID });
            }

            return View(comment);
        }

        /// <summary>
        /// HTTP POST method that is called upon the submission of the comment deletion form.
        /// Deletes the requested comment object from the database.
        /// 
        /// POST: [host]/Cooments/Delete{id parameter}
        /// </summary>
        /// <param name="id">The id of the comment to delete.</param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Retrieve the comment to be deleted
            var comment = await _context.Comments.Include(c => c.Image).FirstAsync(m => m.CommentID == id);

            // Remove the comment from the database
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            // Redirect back to the photo view
            return RedirectToAction("View", "View", new { @id = comment.Image.ImageID });
        }
    }
}
