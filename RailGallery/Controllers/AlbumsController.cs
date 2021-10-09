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
using X.PagedList;

namespace RailGallery.Controllers
{
    public class AlbumsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AlbumsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Albums
        [Route("Author/{username}/Albums")]
        public async Task<IActionResult> Index(string username, int? page)
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

            ViewBag.UserName = user.UserName;

            var albums = await _context.Albums.Where(a => a.ApplicationUser == user).Include(a => a.ApplicationUser).Include(a => a.Images).ToListAsync();

            int pageSize = 100; /*TODO*/

            int pageNumber = (int)((!page.HasValue || page == 0) ? 1 : page);

            return View(albums.ToPagedList(pageNumber, pageSize));
        }

        // GET: Albums/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Albums/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("AlbumID,AlbumTitle,AlbumPrivacy")] Album album)
        {
            if (ModelState.IsValid)
            {
                album.ApplicationUser = await _userManager.GetUserAsync(HttpContext.User);
                _context.Add(album);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Upload");
            }
            return View(album);
        }

        // GET: Albums/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            if (currentUser == null)
            {
                return NotFound();
            }

            IList<string> currentUserRoles = await _userManager.GetRolesAsync(currentUser);

            var album = await _context.Albums.Include(a => a.Images.OrderByDescending(i => i.ImageID)).Include(a => a.ApplicationUser).FirstAsync(a => a.AlbumID == id);

            if (album == null || currentUser == null || (album.ApplicationUser.UserName != currentUser.UserName && !currentUserRoles.Contains(Enums.Roles.Moderator.ToString())))
            {
                return NotFound();
            }

            ViewBag.Images = await _context.Images.Where(i => i.ApplicationUser.UserName == album.ApplicationUser.UserName).OrderByDescending(i => i.ImageID).Select(i =>
                                  new SelectListItem
                                  {
                                      Value = i.ImageID.ToString(),
                                      Text = i.ImageTitle,
                                      Selected = _context.Images.Include(a => a.Albums).Where(a => a.ImageID == i.ImageID && a.Albums.Where(a => a.AlbumID == album.AlbumID).Any()).Any() ? true : false
                                  }).AsNoTracking().ToListAsync();

            return View(album);
        }

        // POST: Albums/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("AlbumID,AlbumTitle,AlbumPrivacy")] Album album, string[]? AlbumImages)
        {
            if (id != album.AlbumID )
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                string username = (await _context.Albums.Include(a => a.ApplicationUser).AsNoTracking().FirstAsync(a => a.AlbumID == id)).ApplicationUser.UserName;

                var albumToUpdate = await _context.Albums.Include(a => a.Images).FirstAsync(a => a.AlbumID == id);

                albumToUpdate.AlbumTitle = album.AlbumTitle;
                albumToUpdate.AlbumPrivacy = album.AlbumPrivacy;
                
                var images = await _context.Images.Include(i=>i.ApplicationUser).Where(i => i.ApplicationUser.UserName == username).ToListAsync();


                foreach(Image image in images)
                {
                    albumToUpdate.Images.Remove(image);
                }

                foreach (string albumImage in AlbumImages)
                {
                    albumToUpdate.Images.Add(await _context.Images.FirstAsync(a => a.ImageID.ToString() == albumImage));
                }

                try
                {
                    _context.Update(albumToUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlbumExists(album.AlbumID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Author", new { @username = username });
            }
            return View(album);
        }

        // GET: Albums/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            if (currentUser == null)
            {
                return NotFound();
            }

            IList<string> currentUserRoles = await _userManager.GetRolesAsync(currentUser);

            var album = await _context.Albums.Include(a => a.Images).Include(a => a.ApplicationUser).FirstAsync(a => a.AlbumID == id);
            if (album == null || currentUser == null || (album.ApplicationUser.UserName != currentUser.UserName && !currentUserRoles.Contains(Enums.Roles.Moderator.ToString())))
            {
                return NotFound();
            }

            return View(album);
        }

        // POST: Albums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var album = await _context.Albums.FindAsync(id);
            string username = (await _context.Albums.Include(a => a.ApplicationUser).AsNoTracking().FirstAsync(a => a.AlbumID == id)).ApplicationUser.UserName;
            _context.Albums.Remove(album);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Author", new { @username = username });
        }

        private bool AlbumExists(int id)
        {
            return _context.Albums.Any(e => e.AlbumID == id);
        }
    }
}
