﻿using System;
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
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Comments
        public async Task<IActionResult> Index()
        {
            return View(await _context.Comments.ToListAsync());
        }

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CommentID,CommentText,CommentDate,Image")] Comment comment)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (currentUser == null)
            {
                return Content("Error");
            }
            else
            {
                comment.ApplicationUser = currentUser;
            }

            var image = await _context.Images.FirstOrDefaultAsync(m => m.ImageID == comment.Image.ImageID);

            if (image == null)
            {
                return Content("Error");
            }
            else
            {
                comment.Image = image;
            }

            _context.Add(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction("View", "View", new { @id = comment.Image.ImageID });
        }


        // GET: Comments/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.Image)
                .Include(c => c.ApplicationUser)
                .FirstOrDefaultAsync(m => m.CommentID == id);

            if (comment == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            IList<string> currentUserRoles = null;
            if (currentUser != null)
            {
                currentUserRoles = await _userManager.GetRolesAsync(currentUser);
            }

            if (currentUser.UserName != comment.ApplicationUser.UserName && !currentUserRoles.Contains(Enums.Roles.Moderator.ToString()))
            {
                return RedirectToAction("View", "View", new { @id = comment.Image.ImageID });
            }

            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comment = await _context.Comments.Include(c => c.Image).FirstAsync(m => m.CommentID == id);
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction("View", "View", new { @id = comment.Image.ImageID });
        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.CommentID == id);
        }
    }
}
