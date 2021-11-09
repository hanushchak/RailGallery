using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RailGallery.Data;
using RailGallery.Enums;
using RailGallery.Models;
using System;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RailGallery.Controllers
{
    /// <summary>
    /// <c>Upload Controller</c>
    /// Contains methods that allows to upload, delete, edit and view the upload history of the previously uploaded photos by the current user.
    /// 
    /// Author: Maksym Hanushchak 
    /// </summary>
    [Authorize]
    public class UploadController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Class constructor.
        /// </summary>
        /// <param name="context">Reference to the current context.</param>
        /// <param name="hostEnvironment">Reference to the current web host.</param>
        /// <param name="userManager">Reference to the User Manager object.</param>
        public UploadController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _webHostEnvironment = hostEnvironment;
            _userManager = userManager;

        }

        /// <summary>
        /// HTTP GET method to retrieve the requered information to display in the Upload form and redirect to the Upload page.
        /// 
        /// GET: [host]/Upload
        /// </summary>
        /// <returns>Redirect to the Upload photo page view.</returns>
        public async Task<IActionResult> Index()
        {
            // Retrieve the currently authenticated user
            ApplicationUser currentUser = await _userManager.GetUserAsync(HttpContext.User);

            // Retrieve the categories to display in the upload form select list
            ViewBag.Categories = await _context.Categories.Select(c =>
                                  new SelectListItem
                                  {
                                      Value = c.CategoryID.ToString(),
                                      Text = c.CategoryTitle
                                  }).AsNoTracking().ToListAsync();

            // Retrieve albums that belong to the current user to display in the upload form select list
            ViewBag.Albums = await _context.Albums.Where(a => a.ApplicationUser.UserName == currentUser.UserName).Select(a =>
                                  new SelectListItem
                                  {
                                      Value = a.AlbumID.ToString(),
                                      Text = a.AlbumTitle
                                  }).AsNoTracking().ToListAsync();

            // Retrieve the locomotives to display in the upload form select list
            ViewBag.Locomotives = await _context.Locomotives.Select(l =>
                                  new SelectListItem
                                  {
                                      Value = l.LocomotiveID.ToString(),
                                      Text = l.LocomotiveModel + " (built: " + l.LocomotiveBuilt.ToShortDateString() + ")"
                                  }).AsNoTracking().ToListAsync();

            // Retrieve the locations to display in the upload form select list
            ViewBag.Locations = await _context.Locations.Select(l =>
                                 new SelectListItem
                                 {
                                     Value = l.LocationID.ToString(),
                                     Text = l.LocationName
                                 }).AsNoTracking().ToListAsync();

            // Redirect to the Upload page view
            return View();
        }

        /// <summary>
        /// HTTP GET method to display the upload history of the currently authenticated user
        /// 
        /// GET: [host]/Upload/History
        /// </summary>
        /// <returns>Redirects to the Upload History view.</returns>
        public async Task<IActionResult> History()
        {
            // Retrieve the currently authenticated user
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

            // Only retrieve the uploaded images that belong to current user
            System.Collections.Generic.List<Image> history = await _context.Images.Where(i => i.ApplicationUser.UserName.Equals(user.UserName)).OrderByDescending(i => i.ImageUploadedDate).AsNoTracking().ToListAsync();

            // Redirect to the Upload History view
            return View(history);
        }

        /// <summary>
        /// HTTP POST method that upon the Upload Photo form submission creates a new image object to store in the database and uploads the image file to the file system.
        /// Validates the submission parameters and the file size. Validates and resizes oversized photos.
        /// 
        /// POST: [host]/Upload/Create{FormCollection}
        /// </summary>
        /// <param name="image">New image object that will be uploaded.</param>
        /// <param name="ImageAlbums">List of albums to include the new photo in.</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ImageID,ImageTitle,ImageDescription,ImageTakenDate,ImageUploadedDate,ImageStatus,ImagePrivacy,ImageFile,ImageCategoryID,ImageLocomotiveID,ImageLocationID, Albums")] Image image, string[]? ImageAlbums)
        {
            // Check for validity of the submitted form parameters
            if (ModelState.IsValid)
            {
                // Store the reference to the selected location in the image being uploaded
                image.Location = await _context.Locations.FirstOrDefaultAsync(l => l.LocationID == Convert.ToInt32(image.ImageLocationID));

                // Store the reference to the selected category in the image being uploaded
                image.Category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryID == Convert.ToInt32(image.ImageCategoryID));

                // Store the reference to the selected locomotive in the image being uploaded
                image.Locomotive = await _context.Locomotives.FirstOrDefaultAsync(l => l.LocomotiveID == Convert.ToInt32(image.ImageLocomotiveID));

                // Add references to the selected albums to the image being uploaded
                if (ImageAlbums.Length > 0)
                {
                    foreach (string id in ImageAlbums)
                    {
                        image.Albums.Add(await _context.Albums.FirstOrDefaultAsync(a => a.AlbumID == Convert.ToInt32(id)));
                    }
                }

                // Get the current host's root folder path
                string wwwRootPath = _webHostEnvironment.WebRootPath;

                // Get the file extension of the image being uploaded
                string fileExtenstion = Path.GetExtension(image.ImageFile.FileName);

                // Generate and Prepend a GUID value to the extension (to avoid duplicate file names in the uploads folder)
                string uniqueFileName = Guid.NewGuid().ToString() + fileExtenstion;

                // Concatinate the final file path to the image
                string filePath = Path.Combine(wwwRootPath, "photo", uniqueFileName);

                // Use File Stream to create the image file in the uploads folder
                using (FileStream fileStream = new(filePath, FileMode.Create))
                {
                    await image.ImageFile.CopyToAsync(fileStream);
                }

                // Check the extension of the uploaded image. If it's not ".jpg" then remove the image and display the error message
                if (Path.GetExtension(filePath).ToLowerInvariant() != ".jpg")
                {
                    System.IO.File.Delete(filePath);
                    return Content("Non-image files are not supported.");
                }

                // Create an Image representation of the uploaded .jpg file to check the image size
                System.Drawing.Image sourceImage = System.Drawing.Image.FromFile(filePath);

                // Get the uploaded image dimensions
                int sourceWidth = sourceImage.Width;
                int sourceHeight = sourceImage.Height;

                // Set the maximum allowed length of the longest side of the image being uploaded (in pixels)
                int maximumAllowedSize = 1380;

                // If the uploaded image is larger than the maximum allowed size...
                if (sourceWidth > maximumAllowedSize || sourceHeight > maximumAllowedSize)
                {
                    // Initialize the new lenght and height to the maximum allowed value initially
                    int newWidth = maximumAllowedSize;
                    int newHeight = maximumAllowedSize;

                    // If the image is horizontal...
                    if (sourceWidth > sourceHeight)
                    {
                        // Use the formula to set the new height to preserve the original aspect ratio
                        newHeight = (newHeight * sourceHeight) / sourceWidth;
                    }
                    // If the image is vertical...
                    else
                    {
                        // Use the formula to set the new width to preserve the original aspect ratio
                        newWidth = (newWidth * sourceWidth) / sourceHeight;
                    }

                    // Create a new Image object with the new width and height that respect the aspect ratio of the original uploaded image
                    System.Drawing.Bitmap newImage = new System.Drawing.Bitmap(newWidth, newHeight);

                    // Resize the uploaded image using the Graphics library and the new height and width
                    using (System.Drawing.Graphics gr = System.Drawing.Graphics.FromImage(newImage))
                    {
                        gr.SmoothingMode = SmoothingMode.Default;
                        gr.InterpolationMode = InterpolationMode.Default;
                        gr.PixelOffsetMode = PixelOffsetMode.Default;
                        gr.DrawImage(sourceImage, new System.Drawing.Rectangle(0, 0, newWidth, newHeight));
                    }

                    // Dispose of the old image file in the file system
                    if (System.IO.File.Exists(filePath))
                    {
                        sourceImage.Dispose();
                        System.IO.File.Delete(filePath);
                    }

                    // Save the new resized image in the file system
                    newImage.Save(filePath);
                    sourceImage = System.Drawing.Image.FromFile(filePath);
                    newImage.Dispose();

                }

                // Set the maximum size of the longest side of the image thumbnail (in pixels)
                int thumbnailSize = 300;

                // Calculate the height of the thumbnail based on its widht (preserves the original aspect ratio)
                int height = (thumbnailSize * sourceImage.Height) / sourceImage.Width;
                int width = thumbnailSize;

                // Resize the original image to create a Thumbnail Image object using the Graphics library
                System.Drawing.Bitmap thumbnailImage = new System.Drawing.Bitmap(width, height);
                using (System.Drawing.Graphics gr = System.Drawing.Graphics.FromImage(thumbnailImage))
                {
                    gr.SmoothingMode = SmoothingMode.Default;
                    gr.InterpolationMode = InterpolationMode.Default;
                    gr.PixelOffsetMode = PixelOffsetMode.Default;
                    gr.DrawImage(sourceImage, new System.Drawing.Rectangle(0, 0, width, height));
                    sourceImage.Dispose();
                }

                // Save the thumbnail to the file system and clear the program memory
                thumbnailImage.Save(Path.Combine(wwwRootPath, "photo/thumbnail", "s_" + uniqueFileName));
                thumbnailImage.Dispose();

                // Save the image file path in the image object to be saved in the database
                image.ImagePath = uniqueFileName;

                // Get the currently authenticated user and their roles
                ApplicationUser currentUser = await _userManager.GetUserAsync(HttpContext.User);
                System.Collections.Generic.IList<string> currentUserRoles = await _userManager.GetRolesAsync(currentUser);

                // If current user is Moderator or the image is private, publish the poto without moderation
                if (currentUserRoles.Contains(Enums.Roles.Moderator.ToString()) || image.ImagePrivacy == Enums.Privacy.Private)
                {
                    // Set the image status to published without need for moderation
                    image.ImageStatus = Enums.Status.Published;
                }

                // Store the reference of the image author in the image object
                image.ApplicationUser = currentUser;

                // Save the new image object in the database
                _context.Add(image);
                await _context.SaveChangesAsync();

                // Redirect to the view of the uploaded image
                return RedirectToAction("View", "View", new { @id = image.ImageID });
            }
            return Content("Error");
        }

        /// <summary>
        /// HTTP GET method that displays the Edit Photo page.
        /// 
        /// 
        /// GET: [host]/Upload/Edit{id parameter}
        /// </summary>
        /// <param name="id">ID of the photo to be edited.</param>
        /// <returns>Redirects to the Edit View.</returns>
        [Authorize(Roles = "Moderator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Retrieve the image object from the database
            Image image = await _context.Images.FindAsync(id);

            if (image == null)
            {
                return NotFound();
            }

            // Redirect to the Edit Photo view
            return View(image);
        }

        /// <summary>
        /// HTTP POST method that is called upon the submission of the Photo Edit form. 
        /// Allows to edit information and metadata of the requested photo.
        /// 
        /// </summary>
        /// <param name="id">ID of the photo to be edited.</param>
        /// <param name="image">The updated image object submitted with the form.</param>
        /// <returns>Redirects back to the image view.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Moderator")]
        public async Task<IActionResult> Edit(int id, [Bind("ImageID,ImageTitle,ImageDescription,ImageStatus,ImagePrivacy")] Image image)
        {
            if (id != image.ImageID)
            {
                return NotFound();
            }

            // Store the new title, description, status and privacy of the image being updated
            string newTitle = image.ImageTitle;
            string newDescription = image.ImageDescription;
            Status newStatus = image.ImageStatus;
            Privacy newPrivacy = image.ImagePrivacy;

            // Retrieve the current image object from the database
            Image oldImage = await _context.Images.FindAsync(id);

            // Change the fields being updated in the old image object
            if (oldImage.ImageTitle != newTitle) { oldImage.ImageTitle = newTitle; }
            if (oldImage.ImageDescription != newDescription) { oldImage.ImageDescription = newDescription; }
            if (oldImage.ImageStatus != newStatus) { oldImage.ImageStatus = newStatus; }
            if (oldImage.ImagePrivacy != newPrivacy) { oldImage.ImagePrivacy = newPrivacy; }

            // Swap submitted image with the image with the updated information
            image = oldImage;

            // Save the update image in the database
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

            // Redirect back to the Image View
            return RedirectToAction("View", "View", new { @id = image.ImageID });
        }

        /// <summary>
        /// HTTP GET method that allows to retrieve the information and display the Delete Confirmation page for the requested image.
        /// </summary>
        /// <param name="id">ID of the photo to be deleted.</param>
        /// <returns>Redirects to the Delete Photo view.</returns>
        [Authorize(Roles = "Moderator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Retrieve the image object from the database
            Image image = await _context.Images.Include(c => c.ApplicationUser)
                .FirstOrDefaultAsync(m => m.ImageID == id);

            if (image == null)
            {
                return NotFound();
            }

            // Redirect to the Delete Confirmation view
            return View(image);
        }

        /// <summary>
        /// HTTP POST method that allows to delete the image from the database and the corresponding files in the file system upon 
        /// the submission of the Delete Confirmation form in the view.
        /// </summary>
        /// <param name="id">ID of the image to be deleted.</param>
        /// <returns>Redirects back to the Home page view.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Moderator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Retrieve the image object to be deleted
            Image image = await _context.Images.FindAsync(id);

            // Store the path of the image file
            string imagePath = image.ImagePath;

            // Remove all records in Comments, Favorites, Likes tables that reference the image to be deleted
            _context.Comments.RemoveRange(await _context.Comments.Include(i => i.Image).Where(c => c.Image.ImageID == image.ImageID).ToListAsync());
            _context.Favorites.RemoveRange(await _context.Favorites.Include(i => i.Image).Where(c => c.Image.ImageID == image.ImageID).ToListAsync());
            _context.Likes.RemoveRange(await _context.Likes.Include(i => i.Image).Where(c => c.Image.ImageID == image.ImageID).ToListAsync());

            // Lastly, remve the image object from the database
            _context.Images.Remove(image);

            // Save the changes in the context
            await _context.SaveChangesAsync();

            // Delete the image files from the folder in the file system
            if (imagePath is not null)
            {
                // Get full path to the image file
                string imageFile = Path.Combine(_webHostEnvironment.WebRootPath, "photo", image.ImagePath);

                // If the image file exists in the file system, delete it
                if (System.IO.File.Exists(imageFile))
                {
                    System.IO.File.Delete(imageFile);
                }

                // Get full path to the image thumbnail file in the file system
                string imageThumbnail = Path.Combine(_webHostEnvironment.WebRootPath, "photo/thumbnail", "s_" + image.ImagePath);

                // If the image thumbnail file exists in the file system, delete it
                if (System.IO.File.Exists(imageThumbnail))
                {
                    System.IO.File.Delete(imageThumbnail);
                }
            }

            // Redirect to the Home view
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Private method to check if the requested image object exists in the database.
        /// </summary>
        /// <param name="id">ID of the image to check.</param>
        /// <returns>Boolean (true if exists, false if does not)</returns>
        private bool ImageExists(int id)
        {
            return _context.Images.Any(e => e.ImageID == id);
        }
    }
}
