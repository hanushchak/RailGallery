using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RailGallery.Data;
using RailGallery.Models;
using RailGallery.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace RailGallery.Controllers
{
    [Authorize]
    public class UploadController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        private readonly UserManager<ApplicationUser> _userManager;

        public UploadController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _webHostEnvironment = hostEnvironment;
            _userManager = userManager;

        }

        // GET: Upload
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            ViewBag.Categories = await _context.Categories.Select(c =>
                                  new SelectListItem
                                  {
                                      Value = c.CategoryID.ToString(),
                                      Text = c.CategoryTitle
                                  }).AsNoTracking().ToListAsync();

            ViewBag.Albums = await _context.Albums.Where(a => a.ApplicationUser.UserName == currentUser.UserName).Select(a =>
                                  new SelectListItem
                                  {
                                      Value = a.AlbumID.ToString(),
                                      Text = a.AlbumTitle
                                  }).AsNoTracking().ToListAsync();

            ViewBag.Locomotives = await _context.Locomotives.Select(l =>
                                  new SelectListItem
                                  {
                                      Value = l.LocomotiveID.ToString(),
                                      Text = l.LocomotiveModel + " (built: " + l.LocomotiveBuilt.ToShortDateString() + ")"
                                  }).AsNoTracking().ToListAsync();

            ViewBag.Locations = await _context.Locations.Select(l =>
                                 new SelectListItem
                                 {
                                     Value = l.LocationID.ToString(),
                                     Text = l.LocationName
                                 }).AsNoTracking().ToListAsync();

            return View();
        }

        // GET: Upload/History
        public async Task<IActionResult> History()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            // Only retrieve images that belong to current user
            var history = await _context.Images.Where(i => i.ApplicationUser.UserName.Equals(user.UserName)).OrderByDescending(i => i.ImageUploadedDate).AsNoTracking().ToListAsync();

            return View(history);
        }

        // GET: Upload/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // POST: Upload/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ImageID,ImageTitle,ImageDescription,ImageTakenDate,ImageUploadedDate,ImageStatus,ImagePrivacy,ImageFile,ImageCategoryID,ImageLocomotiveID,ImageLocationID, Albums")] Image image, string[]? ImageAlbums)
        {
            if (ModelState.IsValid)
            {
                image.Location = await _context.Locations.FirstOrDefaultAsync(l => l.LocationID == Convert.ToInt32(image.ImageLocationID));
                image.Category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryID == Convert.ToInt32(image.ImageCategoryID));
                image.Locomotive = await _context.Locomotives.FirstOrDefaultAsync(l => l.LocomotiveID == Convert.ToInt32(image.ImageLocomotiveID));

                // Add albums
                if(ImageAlbums.Length > 0)
                {
                    foreach(string id in ImageAlbums)
                    {
                        image.Albums.Add(await _context.Albums.FirstOrDefaultAsync(a => a.AlbumID == Convert.ToInt32(id)));
                    }
                }

                // Upload image to wwwroot folder
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string fileExtenstion = Path.GetExtension(image.ImageFile.FileName);
                string uniqueFileName = Guid.NewGuid().ToString() + fileExtenstion;
                string filePath = Path.Combine(wwwRootPath, "photo", uniqueFileName);
                

                using (FileStream fileStream = new (filePath, FileMode.Create))
                {
                    await image.ImageFile.CopyToAsync(fileStream);
                }

                if(Path.GetExtension(filePath).ToLowerInvariant() != ".jpg")
                {
                    System.IO.File.Delete(filePath);
                    return Content("Non-image files are not supported.");
                }

                System.Drawing.Image sourceImage = System.Drawing.Image.FromFile(filePath);

                int sourceWidth = sourceImage.Width;
                int sourceHeight = sourceImage.Height;
                int maximumAllowedSize = 1380;

                if(sourceWidth > maximumAllowedSize || sourceHeight > maximumAllowedSize)
                {
                    int newWidth = maximumAllowedSize;
                    int newHeight = maximumAllowedSize;

                    if(sourceWidth > sourceHeight)
                    {

                        newHeight = (newHeight * sourceHeight) / sourceWidth;
                    }
                    else
                    {
                        newWidth = (newWidth * sourceWidth) / sourceHeight;
                    }

                    System.Drawing.Bitmap newImage = new System.Drawing.Bitmap(newWidth, newHeight);

                    using (System.Drawing.Graphics gr = System.Drawing.Graphics.FromImage(newImage))
                    {
                        gr.SmoothingMode = SmoothingMode.Default;
                        gr.InterpolationMode = InterpolationMode.Default;
                        gr.PixelOffsetMode = PixelOffsetMode.Default;
                        gr.DrawImage(sourceImage, new System.Drawing.Rectangle(0, 0, newWidth, newHeight));
                    }

                    if (System.IO.File.Exists(filePath))
                    {
                        sourceImage.Dispose();
                        System.IO.File.Delete(filePath);
                    }

                    newImage.Save(filePath);
                    sourceImage = System.Drawing.Image.FromFile(filePath);
                    newImage.Dispose();

                }

                // Create image thumbnail
                int thumbnailSize = 300;

                int height = (int)((thumbnailSize * sourceImage.Height) / sourceImage.Width);
                int width = thumbnailSize;
                
                System.Drawing.Bitmap thumbnailImage = new System.Drawing.Bitmap(width, height);
                using (System.Drawing.Graphics gr = System.Drawing.Graphics.FromImage(thumbnailImage))
                {
                    gr.SmoothingMode = SmoothingMode.Default;
                    gr.InterpolationMode = InterpolationMode.Default;
                    gr.PixelOffsetMode = PixelOffsetMode.Default;
                    gr.DrawImage(sourceImage, new System.Drawing.Rectangle(0, 0, width, height));
                    sourceImage.Dispose();
                }

                thumbnailImage.Save(Path.Combine(wwwRootPath, "photo/thumbnail", "s_" + uniqueFileName));
                thumbnailImage.Dispose();

                // Save image to database
                image.ImagePath = uniqueFileName;

                // Get current user
                var currentUser = await _userManager.GetUserAsync(HttpContext.User);
                var currentUserRoles = await _userManager.GetRolesAsync(currentUser);
                
                // If current user is Moderator or the image is private, publish the poto without moderation
                if(currentUserRoles.Contains(Enums.Roles.Moderator.ToString()) || image.ImagePrivacy == Enums.Privacy.Private)
                {
                    image.ImageStatus = Enums.Status.Published;
                }

                // Add author
                image.ApplicationUser = currentUser;

                _context.Add(image);
                await _context.SaveChangesAsync();
                return RedirectToAction("View", "View", new { @id = image.ImageID });
            }
            return Content("Error");
        }

        // GET: Upload/Edit/5
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

        // POST: Upload/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Moderator")]
        public async Task<IActionResult> Edit(int id, [Bind("ImageID,ImageTitle,ImageDescription,ImageStatus,ImagePrivacy")] Image image)
        {
            if (id != image.ImageID)
            {
                return NotFound();
            }

            string newTitle = image.ImageTitle;
            string newDescription = image.ImageDescription;
            Status newStatus = image.ImageStatus;
            Privacy newPrivacy = image.ImagePrivacy;

            var oldImage = await _context.Images.FindAsync(id);
            
            if(oldImage.ImageTitle != newTitle) { oldImage.ImageTitle = newTitle; }
            if (oldImage.ImageDescription != newDescription) { oldImage.ImageDescription = newDescription; }
            if (oldImage.ImageStatus != newStatus) { oldImage.ImageStatus = newStatus; }
            if (oldImage.ImagePrivacy != newPrivacy) { oldImage.ImagePrivacy = newPrivacy; }

            image = oldImage;

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
            return RedirectToAction("View", "View", new { @id = image.ImageID });
        }

        // GET: Upload/Delete/5
        [Authorize(Roles = "Moderator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var image = await _context.Images.Include(c => c.ApplicationUser)
                .FirstOrDefaultAsync(m => m.ImageID == id);
            if (image == null)
            {
                return NotFound();
            }

            return View(image);
        }

        // POST: Upload/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Moderator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var image = await _context.Images.FindAsync(id);
            string imagePath = image.ImagePath;

            _context.Comments.RemoveRange(await _context.Comments.Include(i => i.Image).Where(c => c.Image.ImageID == image.ImageID).ToListAsync());
            _context.Favorites.RemoveRange(await _context.Favorites.Include(i => i.Image).Where(c => c.Image.ImageID == image.ImageID).ToListAsync());
            _context.Likes.RemoveRange(await _context.Likes.Include(i => i.Image).Where(c => c.Image.ImageID == image.ImageID).ToListAsync());
            _context.Images.Remove(image);

            await _context.SaveChangesAsync();
            
            // Delete the image from the folder
            if (imagePath is not null)
            {
                string imageFile = Path.Combine(_webHostEnvironment.WebRootPath, "photo", image.ImagePath);
                if (System.IO.File.Exists(imageFile))
                {
                    System.IO.File.Delete(imageFile);
                }

                string imageThumbnail = Path.Combine(_webHostEnvironment.WebRootPath, "photo/thumbnail", "s_" + image.ImagePath);
                if (System.IO.File.Exists(imageThumbnail))
                {
                    System.IO.File.Delete(imageThumbnail);
                }
            }

            return RedirectToAction("Index", "Home");
        }

        private bool ImageExists(int id)
        {
            return _context.Images.Any(e => e.ImageID == id);
        }
    }
}
