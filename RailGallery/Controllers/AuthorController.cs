using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RailGallery.Data;
using RailGallery.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace RailGallery.Controllers
{
    public class AuthorController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthorController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // GET: AuthorController
        [Route("Author/{username}")]
        public async Task<ActionResult> Index(string? username)
        {
            if (username == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return NotFound();
            }

            dynamic authorModel = new ExpandoObject();

            authorModel.User = user;

            authorModel.RecentPhotos = await _context.Images
                .Where(i => i.ApplicationUser.UserName.Equals(user.UserName) && i.ImageStatus == Enums.Status.Published && i.ImagePrivacy == Enums.Privacy.Public)
                .OrderByDescending(i => i.ImageUploadedDate)
                .Take(15)
                .Include(c => c.Comments)
                .Include(c => c.Likes)
                .AsNoTracking()
                .ToListAsync();

            return View(authorModel);
        }

        // GET: AuthorController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AuthorController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AuthorController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AuthorController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AuthorController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AuthorController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AuthorController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
