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
        public IActionResult Index()
        {
            return View();
        }

        // GET: Upload/History
        public async Task<IActionResult> History()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            // Only retrieve images that belong to current user
            var history = await _context.Images.Where(i => i.ApplicationUser.UserName.Equals(user.UserName)).OrderByDescending(i => i.ImageUploadedDate).ToListAsync();

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
        public async Task<IActionResult> Create([Bind("ImageID,ImageTitle,ImageDescription,ImageMetadata,ImageTakenDate,ImageUploadedDate,ImageStatus,ImagePrivacy,ImageFile")] Image image)
        {
            if (ModelState.IsValid)
            {
                // Upload image to wwwroot folder
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string fileExtenstion = Path.GetExtension(image.ImageFile.FileName);
                string uniqueFileName = Guid.NewGuid().ToString() + fileExtenstion;
                string filePath = Path.Combine(wwwRootPath, "photo", uniqueFileName);
                
                //TODO Check size
                using (FileStream fileStream = new (filePath, FileMode.Create))
                {
                    await image.ImageFile.CopyToAsync(fileStream);
                }

                // Create image thumbnail
                int thumbnailSize = 300; // In px

                System.Drawing.Image sourceImage = System.Drawing.Image.FromFile(filePath);

                int width, height;

                if(sourceImage.Width >= sourceImage.Height)
                {
                    height = (int)((thumbnailSize * sourceImage.Height) / sourceImage.Width);
                    width = thumbnailSize;
                }
                else
                {
                    height = thumbnailSize;
                    width = (int)((thumbnailSize * sourceImage.Width) / sourceImage.Height);
                }
                
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
                return RedirectToAction(nameof(Index));
            }
            return View(image);
        }

        // GET: Upload/Edit/5
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
        public async Task<IActionResult> Edit(int id, [Bind("ImageID,ImageTitle,ImageDescription,ImageMetadata,ImageTakenDate,ImageUploadedDate,ImageStatus,ImagePrivacy")] Image image)
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

        // GET: Upload/Delete/5
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

        // POST: Upload/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var image = await _context.Images.FindAsync(id);

            // Delete the image from the folder
            if(image.ImagePath is not null)
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
