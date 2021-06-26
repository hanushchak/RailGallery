using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RailGallery.Data;
using RailGallery.Models;

namespace RailGallery.Controllers
{
    //[Authorize]
    public class UploadController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UploadController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _webHostEnvironment = hostEnvironment;
        }

        // GET: Upload
        public async Task<IActionResult> Index()
        {
            return View();
        }

        // GET: Upload/History
        public async Task<IActionResult> History()
        {
            return View(await _context.Images.ToListAsync());
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
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await image.ImageFile.CopyToAsync(fileStream);
                }

                image.ImagePath = uniqueFileName;

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
                string file = Path.Combine(_webHostEnvironment.WebRootPath, "photo", image.ImagePath);
                if (System.IO.File.Exists(file))
                {
                    System.IO.File.Delete(file);
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
