using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RailGallery.Data;
using RailGallery.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace RailGallery.Controllers
{
    /// <summary>
    /// <c>Albums Controller</c>
    /// Contains methods that allow to create, display, update and delete user albums.
    /// 
    /// Author: Maksym Hanushchak
    /// </summary>
    public class AlbumsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context">Reference to the Context object.</param>
        /// <param name="userManager">Reference to the User Manager object.</param>
        public AlbumsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// HTTP GET method to retrieve a list of albums that belong to the user and pass it to the view. 
        /// 
        /// GET: [host]/Author/{username}/Albums
        /// </summary>
        /// <param name="username">The user's username.</param>
        /// <param name="page">Current page.</param>
        /// <returns></returns>
        [Route("Author/{username}/Albums")]
        public async Task<IActionResult> Index(string username, int? page)
        {
            if (username == null)
            {
                return NotFound();
            }

            // Retrieve the user
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return NotFound();
            }

            // Store username in a viewbag to preserve the values between different pages
            ViewBag.UserName = user.UserName;

            // Retrieve the albums that belong to the user
            var albums = await _context.Albums.Where(a => a.ApplicationUser == user).Include(a => a.ApplicationUser).Include(a => a.Images.OrderByDescending(i => i.ImageID)).ToListAsync();

            // Size of the list on one page
            int pageSize = 100;

            // Ternary operator to calculate the page number
            int pageNumber = (int)((!page.HasValue || page == 0) ? 1 : page);

            return View(albums.ToPagedList(pageNumber, pageSize)); // Redirect to the view Author/Albums
        }

        /// <summary>
        /// HTTP POST method that redirects to the create album view.
        /// 
        /// GET: [host]/Albums/Create
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// HTTP POST method that creates a new album and saves it in the database based on the received form submission values.
        /// 
        /// POST: [host]/Albums/Create{FormCollection}
        /// </summary>
        /// <param name="album">Album object with the values specified in the form.</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("AlbumID,AlbumTitle,AlbumPrivacy")] Album album)
        {
            if (ModelState.IsValid)
            {
                // Retrieve the current user and add it as the album's author
                album.ApplicationUser = await _userManager.GetUserAsync(HttpContext.User);

                // Save the album in the database
                _context.Add(album);
                await _context.SaveChangesAsync();

                // Redirect back to the Upload view
                return RedirectToAction("Index", "Upload");
            }
            return View(album);
        }

        /// <summary>
        /// HTTP GET method that redirects to the album edit view.
        /// 
        /// GET: [host]/Albums/Edit/{parameters}
        /// </summary>
        /// <param name="id">The id of the album to edit.</param>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Retrieve the currently authenticated user
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            if (currentUser == null)
            {
                return NotFound();
            }

            // Retrieve the roles of the currently authenticated user and save them to a list
            IList<string> currentUserRoles = await _userManager.GetRolesAsync(currentUser);

            // Retrieve the album to edit
            var album = await _context.Albums.Include(a => a.Images.OrderByDescending(i => i.ImageID)).Include(a => a.ApplicationUser).FirstAsync(a => a.AlbumID == id);

            // In case retrieval fails - display NotFound error
            if (album == null || currentUser == null || (album.ApplicationUser.UserName != currentUser.UserName && !currentUserRoles.Contains(Enums.Roles.Moderator.ToString())))
            {
                return NotFound();
            }

            // Store images that belong to the current user in the View Bag to display in the image select list on the edit page.
            ViewBag.Images = await _context.Images.Where(i => i.ApplicationUser.UserName == album.ApplicationUser.UserName).OrderByDescending(i => i.ImageID).Select(i =>
                                  new SelectListItem
                                  {
                                      Value = i.ImageID.ToString(),
                                      Text = (i.ImageStatus == Enums.Status.Published ? i.ImageTitle : i.ImageTitle + " (photo pending)"),
                                      Selected = _context.Images.Include(a => a.Albums).Where(a => a.ImageID == i.ImageID && a.Albums.Where(a => a.AlbumID == album.AlbumID).Any()).Any() ? true : false
                                  }).AsNoTracking().ToListAsync();

            return View(album);
        }

        /// <summary>
        /// HTTP POST method that is called after the Edit album form is submitted. Updates the album with the values specified in the form.
        /// 
        /// POST: [host]/Albums/Edit{FormCollection}
        /// </summary>
        /// <param name="id">The id of the album to edit.</param>
        /// <param name="album">The updated album object.</param>
        /// <param name="AlbumImages">The updated list of images that are in the album.</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("AlbumID,AlbumTitle,AlbumPrivacy")] Album album, string[]? AlbumImages)
        {
            if (id != album.AlbumID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Retrieve the user profile of the author of the album
                string username = (await _context.Albums.Include(a => a.ApplicationUser).AsNoTracking().FirstAsync(a => a.AlbumID == id)).ApplicationUser.UserName;

                // Retrieve the album that will be updated
                var albumToUpdate = await _context.Albums.Include(a => a.Images).FirstAsync(a => a.AlbumID == id);

                // Update the values in the retrieved album object with the new values
                albumToUpdate.AlbumTitle = album.AlbumTitle;
                albumToUpdate.AlbumPrivacy = album.AlbumPrivacy;

                // Retrieve all images that belong to the current user
                var images = await _context.Images.Include(i => i.ApplicationUser).Where(i => i.ApplicationUser.UserName == username).ToListAsync();

                // Loop through all of the images and remove them from the album
                foreach (Image image in images)
                {
                    albumToUpdate.Images.Remove(image);
                }

                // Loop through the new list of images and add them to the album
                foreach (string albumImage in AlbumImages)
                {
                    albumToUpdate.Images.Add(await _context.Images.FirstAsync(a => a.ImageID.ToString() == albumImage));
                }

                // Attempt to update and save the album in the database, display an error message on fail
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

                // Redirect back to the user profile view
                return RedirectToAction("Index", "Author", new { @username = username });
            }
            return View(album);
        }

        /// <summary>
        /// HTTP GET method that retrieves information about the album to be deleted and passes it to the view.
        /// 
        /// GET: [host]/Albums/Delete{parameters}
        /// </summary>
        /// <param name="id">The id of the album to be deleted.</param>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Retrieve the currently authenticated user
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            if (currentUser == null)
            {
                return NotFound();
            }

            // Retrieve the roles of the current user and store them in the list
            IList<string> currentUserRoles = await _userManager.GetRolesAsync(currentUser);

            // Retrieve the album object to be deleted
            var album = await _context.Albums.Include(a => a.Images).Include(a => a.ApplicationUser).FirstAsync(a => a.AlbumID == id);

            // If any of the above retrievals fails - display Not Found error message.
            if (album == null || currentUser == null || (album.ApplicationUser.UserName != currentUser.UserName && !currentUserRoles.Contains(Enums.Roles.Moderator.ToString())))
            {
                return NotFound();
            }

            return View(album);
        }

        /// <summary>
        /// HTTP POST method to delete an album after the successful form submission in the Delete view.
        /// 
        /// POST: [host]/Albums/DeleteConfirmed{FormCollection}
        /// </summary>
        /// <param name="id">The id of the album to be deleted.</param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Retrieve the album object to be deleted
            var album = await _context.Albums.FindAsync(id);

            // Retrieve the album author's username
            string username = (await _context.Albums.Include(a => a.ApplicationUser).AsNoTracking().FirstAsync(a => a.AlbumID == id)).ApplicationUser.UserName;

            // Remove the album from the database
            _context.Albums.Remove(album);
            await _context.SaveChangesAsync();

            // Redirect back to the user profile of the author.
            return RedirectToAction("Index", "Author", new { @username = username });
        }

        /// <summary>
        /// Private method to check if the album exits before attempting to edit it.
        /// </summary>
        /// <param name="id">The id of the album to check existence of.</param>
        /// <returns>True if album exists, False if album does not exist.</returns>
        private bool AlbumExists(int id)
        {
            return _context.Albums.Any(e => e.AlbumID == id);
        }
    }
}
