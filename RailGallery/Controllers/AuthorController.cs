using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RailGallery.Data;
using RailGallery.Models;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace RailGallery.Controllers
{
    /// <summary>
    /// <c>Author Controller</c>
    /// Contains a method that allows to retrieve public user information to display on the profile page.
    /// 
    /// Author: Maksym Hanushchak
    /// </summary>
    public class AuthorController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context">Reference to the Context object</param>
        /// <param name="userManager">Reference to the User Manager object</param>
        public AuthorController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        /// <summary>
        /// HTTP GET method to retrieve public profile information based on the passed username parameter.
        /// 
        /// GET: [host]/Author/{username}
        /// </summary>
        /// <param name="username">The username of the user profile to retrieve.</param>
        /// <returns></returns>
        [Route("Author/{username}")]
        public async Task<ActionResult> Index(string? username)
        {
            if (username == null)
            {
                return NotFound();
            }

            // Find the user in the database using the User Manager
            var user = await _userManager.FindByNameAsync(username);

            // If the user is not found, display the not found error
            if (user == null)
            {
                return NotFound();
            }

            // Create ExpandoObject that will be used to pass recent photos and albums of the user to the view
            dynamic authorModel = new ExpandoObject();

            // Add user profile to the dynamic model
            authorModel.User = user;

            // Retrieve 10 of the user's most recent published photos and add them to the dynamic model
            authorModel.RecentPhotos = await _context.Images
                .Where(i => i.ApplicationUser.UserName.Equals(user.UserName) && i.ImageStatus == Enums.Status.Published && i.ImagePrivacy == Enums.Privacy.Public)
                .OrderByDescending(i => i.ImageUploadedDate)
                .Take(10)
                .Include(c => c.Comments)
                .Include(c => c.Likes)
                .AsNoTracking()
                .ToListAsync();

            // Retrieve 10 of the user's most recent public albums and add them to the dynamic model
            authorModel.RecentAlbums = await _context.Albums
                .Where(a => a.ApplicationUser.UserName.Equals(user.UserName))
                .OrderByDescending(a => a.AlbumID)
                .Take(10)
                .Include(a => a.Images.OrderByDescending(i => i.ImageID))
                .Include(a => a.ApplicationUser)
                .AsNoTracking()
                .ToListAsync();

            // Redirect to the Author view and pass the dynamic model with the retrieved information
            return View(authorModel);
        }
    }
}
