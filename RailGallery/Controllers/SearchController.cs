using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RailGallery.Data;
using RailGallery.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

#nullable enable

namespace RailGallery.Controllers
{
    /// <summary>
    /// <c>Search Controller</c>
    /// Contains methods that allow to filter and search photos and return the list in the view.
    /// 
    /// Author: Maksym Hanushchak
    /// </summary>
    public class SearchController : Controller
    {
        private readonly ILogger<SearchController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Class constructor.
        /// </summary>
        /// <param name="logger">Reference to the Logger object</param>
        /// <param name="context">Reference to the current Context</param>
        /// <param name="userManager">Reference to the User Manager object</param>
        public SearchController(ILogger<SearchController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }


        /// <summary>
        /// HTTP GET method that displays the Search view with the form upon calling.
        /// 
        /// GET: [host]/Search
        /// </summary>
        /// <returns>Redirects to the Search form view.</returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// HTTP GET method that retrieves the photos upon a search request and displays them on the Search Results view page.
        /// 
        /// GET: [host]/Search/Results/{search parameters}
        /// </summary>
        /// <param name="page">The page number of page to display.</param>
        /// <param name="ImageTitle">The image title to search.</param>
        /// <param name="ImageAuthor">The image title to search</param>
        /// <param name="ImageDescription">The image description to search.</param>
        /// <param name="LocomotiveModel">The image locomotive to search.</param>
        /// <param name="LocomotiveBuilt">The image locomotive built date to search.</param>
        /// <param name="ImageAlbum">The album name where to search images.</param>
        /// <param name="ImageCategory">The image category to search.</param>
        /// <param name="ImageTakenDate">The image date taken to search.</param>
        /// <param name="ImageLocation">The image location to search.</param>
        /// <returns></returns>
        public async Task<IActionResult> Results(int? page,
                                    string? ImageTitle,
                                    string? ImageAuthor,
                                    string? ImageDescription,
                                    string? LocomotiveModel,
                                    string? LocomotiveBuilt,
                                    string? ImageAlbum,
                                    string? ImageCategory,
                                    string? ImageTakenDate,
                                    string? ImageLocation)
        {

            // Retrieve the currently authenticated user
            ApplicationUser? currentUser = await _userManager.GetUserAsync(HttpContext.User);

            // Delare a list that will store the current user's roles
            IList<string> currentUserRoles = null;

            if (currentUser != null)
            {
                // Retireve the roles of the current user
                currentUserRoles = await _userManager.GetRolesAsync(currentUser);
            }

            // Boolean to indicate if the user is logged in at the moment
            bool userLoggedIn = currentUser != null;

            // Boolean to indicate if the user has the moderator role
            bool userIsModerator = userLoggedIn && currentUserRoles.Contains(Enums.Roles.Moderator.ToString());

            // Retrieve a list of images to search among
            IQueryable<Image>? model = _context.Images
            .Where(i => !((i.ImageStatus == Enums.Status.Pending) || (i.ImageStatus == Enums.Status.Rejected) || (i.ImagePrivacy == Enums.Privacy.Private && !(userLoggedIn && i.ApplicationUser.UserName.Equals(currentUser.UserName)))))
            .OrderByDescending(i => i.ImageUploadedDate)
            .Include(c => c.Comments)
            .Include(c => c.Likes)
            .Include(c => c.ApplicationUser)
            .Include(c => c.Category)
            .Include(c => c.Location)
            .Include(c => c.Locomotive)
            .Include(c => c.Albums)
            .AsNoTracking();

            // If the Image Title search string is received, filter the image list using this parameter
            if (!string.IsNullOrEmpty(ImageTitle))
            {
                // Filter the list of images using the receinved parameter
                model = model.Where(i => i.ImageTitle.Contains(ImageTitle));
                // Store the search parameter in the view bag to persist in the input field of the search form
                ViewBag.ImageTitle = ImageTitle;
            }

            // For the remaining IF statements below, please refer to the above description of the similar code ^

            if (!string.IsNullOrEmpty(ImageAuthor))
            {
                model = model.Where(i => i.ApplicationUser.UserName.Contains(ImageAuthor));
                ViewBag.ImageAuthor = ImageAuthor;
            }
            if (!string.IsNullOrEmpty(ImageDescription))
            {
                model = model.Where(i => i.ImageDescription.Contains(ImageDescription));
                ViewBag.ImageDescription = ImageDescription;
            }
            if (!string.IsNullOrEmpty(LocomotiveModel))
            {
                model = model.Where(i => i.Locomotive.LocomotiveModel.Contains(LocomotiveModel));
                ViewBag.LocomotiveModel = LocomotiveModel;
            }
            if (!string.IsNullOrEmpty(LocomotiveBuilt))
            {
                model = model.Where(i => i.Locomotive.LocomotiveBuilt.ToString().Contains(LocomotiveBuilt));
                ViewBag.LocomotiveBuilt = LocomotiveBuilt;
            }
            if (!string.IsNullOrEmpty(ImageAlbum))
            {
                model = model.Where(i => i.Albums.Where(a => a.AlbumTitle.Contains(ImageAlbum)).Any());
                ViewBag.ImageAlbum = ImageAlbum;
            }
            if (!string.IsNullOrEmpty(ImageCategory))
            {
                model = model.Where(i => i.Category.CategoryTitle.Contains(ImageCategory));
                ViewBag.ImageCategory = ImageCategory;
            }
            if (!string.IsNullOrEmpty(ImageTakenDate))
            {
                model = model.Where(i => i.ImageTakenDate.ToString().Contains(ImageTakenDate));
                ViewBag.ImageTakenDate = ImageTakenDate;
            }
            if (!string.IsNullOrEmpty(ImageLocation))
            {
                model = model.Where(i => i.Location.LocationName.Contains(ImageLocation));
                ViewBag.ImageLocation = ImageLocation;
            }

            // Set the maximum number of photos to display on a single page
            int pageSize = 15;

            // Calculate the current page number
            int pageNumber = (int)((!page.HasValue || page == 0) ? 1 : page);

            // Redirect to the Search Results View
            return View(model.ToPagedList(pageNumber, pageSize));
        }
    }
}
