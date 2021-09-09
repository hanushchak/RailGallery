using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RailGallery.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RailGallery.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhotoController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PhotoController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _webHostEnvironment = hostEnvironment;
        }

        // GET Thumbnail
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPhoto(int? id)
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


            string wwwRootPath = _webHostEnvironment.WebRootPath;
            string filePath = Path.Combine(wwwRootPath, "photo", image.ImagePath);

            Byte[] imageFile = System.IO.File.ReadAllBytes(filePath);
            return File(imageFile, "image/jpeg");
        }

        // GET Thumbnail
        [HttpGet("{id}/Thumbnail")]
        public async Task<IActionResult> GetThumbnail(int? id)
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


            string wwwRootPath = _webHostEnvironment.WebRootPath;
            string filePath = Path.Combine(wwwRootPath, "photo/thumbnail", "s_" + image.ImagePath);

            Byte[] imageFile = System.IO.File.ReadAllBytes(filePath);
            return File(imageFile, "image/jpeg");
        }
    }
}
