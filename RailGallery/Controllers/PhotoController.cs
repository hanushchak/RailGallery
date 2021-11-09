using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RailGallery.Data;
using System.IO;
using System.Threading.Tasks;


namespace RailGallery.Controllers
{
    /// <summary>
    /// <c>API Photo Controller</c>
    /// 
    /// This controller contains methods that retrieve the requested photo file from the file system and return it. Called from within other Controller methods.
    /// 
    /// Author: Maksym Hanushchak
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PhotoController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        /// <summary>
        /// Class constructor.
        /// </summary>
        /// <param name="context">Reference to the current context.</param>
        /// <param name="hostEnvironment">Reference to the current host's file system.</param>
        public PhotoController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _webHostEnvironment = hostEnvironment;
        }

        /// <summary>
        /// GET HTTP method that allows to retrieve the photo file from the file system upon receiving the image path.
        /// 
        /// GET: [host]/api/Photo/GetPhoto{imagePath parameter}
        /// </summary>
        /// <param name="ImagePath">The path to the image file.</param>
        /// <returns>The requested Image jpg file.</returns>
        [HttpGet("{ImagePath}")]
        public async Task<IActionResult> GetPhoto(string? ImagePath)
        {
            // If the path is not requested, return the Not Found error.
            if (ImagePath == null)
            {
                return NotFound();
            }

            // Retrieve the image that references the requested path
            Models.Image image = await _context.Images
                .FirstOrDefaultAsync(m => m.ImagePath == ImagePath);

            // If the image is not found in the database, return the Not Found error
            if (image == null)
            {
                return NotFound();
            }

            // Get the host file system's root
            string wwwRootPath = _webHostEnvironment.WebRootPath;

            // Generate a file path to the requested image
            string filePath = Path.Combine(wwwRootPath, "photo", image.ImagePath);

            // Create an image file and read the image from the file system
            byte[] imageFile = System.IO.File.ReadAllBytes(filePath);

            // Send back the image jpg file
            return File(imageFile, "image/jpeg");
        }

        /// <summary>
        /// GET HTTP method that allows to retrieve the photo thumbnail file from the file system upon receiving the image path.
        /// 
        /// GET: [host]/api/Photo/GetThumbnail{imagePath parameter}
        /// </summary>
        /// <param name="ImagePath">The path to the image file.</param>
        /// <returns>The requested Image jpg thumbnail file.</returns>
        [HttpGet("{ImagePath}/Thumbnail")]
        public async Task<IActionResult> GetThumbnail(string? ImagePath)
        {
            // If the path is not requested, return the Not Found error.
            if (ImagePath == null)
            {
                return NotFound();
            }

            // Retrieve the image that references the requested path
            Models.Image image = await _context.Images
                .FirstOrDefaultAsync(m => m.ImagePath == ImagePath);

            // If the image is not found in the database, return the Not Found error
            if (image == null)
            {
                return NotFound();
            }

            // Get the host file system's root
            string wwwRootPath = _webHostEnvironment.WebRootPath;

            // Generate a file path to the requested image
            string filePath = Path.Combine(wwwRootPath, "photo/thumbnail", "s_" + image.ImagePath);

            // Create an image file and read the image from the file system
            byte[] imageFile = System.IO.File.ReadAllBytes(filePath);

            // Send back the image jpg file
            return File(imageFile, "image/jpeg");
        }
    }
}
