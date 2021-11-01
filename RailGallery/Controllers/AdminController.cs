using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RailGallery.Data;
using RailGallery.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace RailGallery.Controllers
{
    /// <summary>
    /// <c>AdminController</c>
    /// Contains methods that allow to access and manage data by Moderator users.
    /// 
    /// Author: Maksym Hanushchak
    /// </summary>
    [Authorize(Roles = "Moderator")]
    public class AdminController : Controller
    {

        private readonly ILogger<AdminController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">Reference to the Logger object</param>
        /// <param name="context">Reference to the Context object</param>
        /// <param name="userManager">Reference to the User Manager object</param>
        /// <param name="roleManager">Reference to the Role Manager object</param>
        public AdminController(ILogger<AdminController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        /// <summary>
        /// Index method. Passes an object with pending photos and recent comments to the Admin/Index view.
        /// </summary>
        /// <returns>View (Views/Admin/Index.cshtml)</returns>
        public IActionResult Index()
        {
            // Create ExpandoObject that will be used to pass pending images and recent comments to the view
            dynamic adminModel = new ExpandoObject();

            // Retrieve 15 pending images to pass to the view
            adminModel.PendingImages = _context.Images
                .Where(i => i.ImageStatus == Enums.Status.Pending && i.ImagePrivacy != Enums.Privacy.Private)
                .OrderBy(i => i.ImageUploadedDate)
                .Take(15)
                .Include(c => c.ApplicationUser)
                .AsNoTracking();

            // Retrieve 15 most recent comments to pass to the view
            adminModel.RecentComments = _context.Comments
                .OrderByDescending(c => c.CommentDate)
                .Take(15)
                .Include(c => c.ApplicationUser)
                .Include(c => c.Image)
                .AsNoTracking();

            return View(adminModel);
        }

        /// <summary>
        /// HttpPost method that is called from the photo review form in Views/View.cshtml.
        /// Used to approve or reject a photo.
        /// </summary>
        /// <param name="collection">Values submitted in the form</param>
        /// <returns>Redirects back to the View or returns error.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReviewPhoto([Bind("decision,ImageID")] IFormCollection collection)
        {
            string imageID = collection["ImageID"];

            if (String.IsNullOrEmpty(imageID))
            {
                return Content("Error");
            }

            var image = await _context.Images.FirstOrDefaultAsync(m => m.ImageID.ToString() == imageID);

            // If the submitted decision is to reject the photo - change the photo status to Rejected
            if (collection["decision"] == "Reject")
            {
                image.ImageStatus = Enums.Status.Rejected;
                _context.Update(image);
                await _context.SaveChangesAsync();

                return RedirectToAction("View", "View", new { @id = image.ImageID }); // Redirect back to View
            }

            // If the submitted decision is to approve the photo - change the photo status to Approved
            if (collection["decision"] == "Approve")
            {
                image.ImageStatus = Enums.Status.Published;
                _context.Update(image);
                await _context.SaveChangesAsync();

                return RedirectToAction("View", "View", new { @id = image.ImageID }); // Redirect back to View
            }

            return Content("Error");
        }

        /// <summary>
        /// Method used to to retrieve all users and pass them to the view (Views/Admin/Users.cshtml)
        /// </summary>
        /// <param name="page">Current page</param>
        /// <param name="username">Username/Email filter</param>
        /// <param name="sortOrder">Sort order</param>
        /// <returns>View (Views/Admin/Users.cshtml)</returns>
        public async Task<IActionResult> Users(int? page, string username, string sortOrder)
        {
            // Store username and sortorder in a viewbag to preserve the values between different pages
            ViewBag.SearchString = username;
            ViewBag.SortOrder = sortOrder;

            // Ternary operators to store sort order in the view bag that corresponds to the received GET parameter
            ViewBag.UsernameSort = String.IsNullOrEmpty(sortOrder) ? "username_desc" : "";
            ViewBag.EmailSort = sortOrder == "Email" ? "email_desc" : "Email";
            ViewBag.RegisteredSort = sortOrder == "Registered" ? "registered_desc" : "Registered";
            ViewBag.LastActiveSort = sortOrder == "LastActive" ? "lastactive_desc" : "LastActive";
            ViewBag.NumberPhotosSort = sortOrder == "NumberPhotos" ? "numberphotos_desc" : "NumberPhotos";
            ViewBag.NumberCommentsSort = sortOrder == "NumberComments" ? "numbercomments_desc" : "NumberComments";

            // Get list of users
            var users = await _userManager.Users.Include(u => u.Images).Include(u => u.Comments).ToListAsync();

            // Filter the list of users if the GET filter parameter is not empty
            if (!String.IsNullOrEmpty(username))
            {
                users = users.Where(u => u.UserName.Contains(username) || u.Email.Contains(username)).ToList();
            }

            // Switch to order the list based on the requested sort order
            users = sortOrder switch
            {
                "username_desc" => users.OrderByDescending(u => u.UserName).ToList(),
                "Email" => users.OrderBy(u => u.Email).ToList(),
                "email_desc" => users.OrderByDescending(u => u.Email).ToList(),
                "Registered" => users.OrderBy(u => u.RegisterationDate).ToList(),
                "registered_desc" => users.OrderByDescending(u => u.RegisterationDate).ToList(),
                "LastActive" => users.OrderBy(u => u.LastActivityDate).ToList(),
                "lastactive_desc" => users.OrderByDescending(u => u.LastActivityDate).ToList(),
                "NumberPhotos" => users.OrderBy(u => u.Images.Count).ToList(),
                "numberphotos_desc" => users.OrderByDescending(u => u.Images.Count).ToList(),
                "NumberComments" => users.OrderBy(u => u.Comments.Count).ToList(),
                "numbercomments_desc" => users.OrderByDescending(u => u.Comments.Count).ToList(),
                _ => users.OrderBy(u => u.UserName).ToList(),
            };

            // Size of the list on one page
            int pageSize = 10;

            // Ternary operator to calculate the page number
            int pageNumber = (int)((!page.HasValue || page == 0) ? 1 : page);

            return View(users.ToPagedList(pageNumber, pageSize));
        }

        /// <summary>
        /// GET Method to retrieve a list of roles for the user and pass them to the view.
        /// </summary>
        /// <param name="username">Username of the user</param>
        /// <returns>View (Views/Admin/Roles.cshtml)</returns>
        public async Task<IActionResult> Roles(string username)
        {
            if (username == null)
            {
                return NotFound();
            }

            ViewBag.username = username; // Store username in the view bag to reference in the UI

            // Retrieve the user
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return NotFound();
            }

            // Create a viewmodel to display the roles
            var model = new List<UserRolesViewModel>();

            // Get all roles
            var roles = await _roleManager.Roles.ToListAsync();

            // Iterate through all roles and add them to a list to display in the UI <select> component
            foreach (var role in roles)
            {
                var userRolesViewModel = new UserRolesViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };
                if (await _userManager.IsInRoleAsync(user, role.Name)) // If the user has the role, mark it as selected
                {
                    userRolesViewModel.Selected = true;
                }
                else
                {
                    userRolesViewModel.Selected = false;
                }
                model.Add(userRolesViewModel);
            }
            return View(model);
        }

        /// <summary>
        /// HTTP POST method to update the user's role.
        /// </summary>
        /// <param name="username">The user to be updated.</param>
        /// <param name="model">The list with chosen selections sent by the View (Views/Admin/Roles.cshtml)</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRole(string username, List<UserRolesViewModel> model)
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

            // Retrieve the user's current roles
            var roles = await _userManager.GetRolesAsync(user);

            // Remove the user's current roles
            var result = await _userManager.RemoveFromRolesAsync(user, roles);
            
            // If removal fails, display the error message
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing roles");
                return View(model);
            }

            // Add new selected roles to the user
            result = await _userManager.AddToRolesAsync(user, model.Where(x => x.Selected).Select(y => y.RoleName));
            
            // If update fails, display the error message
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected roles to user");
                return View(model);
            }

            // Return back to the View with the user list
            return RedirectToAction("Users");
        }

        /// <summary>
        /// HTTP GET method to retrieve the user's current account access state (enabled/disabled).
        /// </summary>
        /// <param name="username">The username of the user to update access of.</param>
        /// <returns>View (Views/Admin/Access.cshtml)</returns>
        public async Task<IActionResult> Access(string username)
        {
            if (username == null)
            {
                return NotFound();
            }

            // Store the username in the view bag to reference in the UI
            ViewBag.username = username;

            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return NotFound();
            }

            // Store the current account state in the view bag to display in the UI
            if (user.LockoutEnabled)
            {
                ViewBag.Lockout = true;
            }
            else
            {
                ViewBag.Lockout = false;
            }

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        /// <summary>
        /// HTTP POST method to enable or disable user's account.
        /// </summary>
        /// <param name="username">Username of the user to modify the access of.</param>
        /// <param name="access">String to specify whether the access should be enabled or disabled.</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAccess(string username, string access)
        {
            if (username == null || access == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return NotFound();
            }

            // Update the account's access based on the submitted value in the form
            if (access == "disabled")
            {
                await _userManager.SetLockoutEnabledAsync(user, true); // Enable lockout
                await _userManager.SetLockoutEndDateAsync(user, DateTime.UtcNow.AddYears(1)); // Set lockout end in one year from the current date
                await _userManager.UpdateAsync(user); // Update the account
            }
            else
            {
                await _userManager.SetLockoutEnabledAsync(user, false); // Disable lockout
                await _userManager.SetLockoutEndDateAsync(user, null); // Remove the lockout end date
                await _userManager.UpdateAsync(user); // Update the account
            }

            return RedirectToAction("Users"); // Redirect back to the Users view
        }

        /// <summary>
        /// HTTP GET method to tetrieve a list of all photos to display in the Photo Management view.
        /// </summary>
        /// <param name="page">Page number of the page to display.</param>
        /// <param name="sortOrder">Order in which the list should be sorted.</param>
        /// <returns></returns>
        public async Task<IActionResult> Photos(int? page, string sortOrder)
        {
            // Store sortorder in a viewbag to preserve the values between different pages
            ViewBag.SortOrder = sortOrder;

            // Ternary operators to store sort order in the view bag that corresponds to the received GET parameter
            ViewBag.DateUploadedSort = String.IsNullOrEmpty(sortOrder) ? "DateUploaded_desc" : "";
            ViewBag.DateTakenSort = sortOrder == "DateTaken" ? "DateTaken_desc" : "DateTaken";
            ViewBag.TitleSort = sortOrder == "Title" ? "Title_desc" : "Title";
            ViewBag.PrivacySort = sortOrder == "Privacy" ? "Privacy_desc" : "Privacy";
            ViewBag.StatusSort = sortOrder == "Status" ? "Status_desc" : "Status";
            ViewBag.AuthorSort = sortOrder == "Author" ? "Author_desc" : "Author";
            ViewBag.Views24Sort = sortOrder == "Views24" ? "Views24_desc" : "Views24";
            ViewBag.ViewsWeekSort = sortOrder == "ViewsWeek" ? "ViewsWeek_desc" : "ViewsWeek";
            ViewBag.ViewsMonthSort = sortOrder == "ViewsMonth" ? "ViewsMonth_desc" : "ViewsMonth";
            ViewBag.ViewsSort = sortOrder == "Views" ? "Views_desc" : "Views";

            // Retrieve all photos
            var photos = await _context.Images.Include(u => u.ApplicationUser).Include(u => u.Comments).Include(u => u.ImageViews).ToListAsync();

            // Retrieve current time in EST timezone
            DateTime currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));

            // Switch to order the list based on the requested sort order
            photos = sortOrder switch
            {
                "Title" => photos.OrderBy(u => u.ImageTitle).ToList(),
                "Title_desc" => photos.OrderByDescending(u => u.ImageTitle).ToList(),
                "DateTaken" => photos.OrderBy(u => u.ImageTakenDate).ToList(),
                "DateTaken_desc" => photos.OrderByDescending(u => u.ImageTakenDate).ToList(),
                "DateUploaded" => photos.OrderBy(u => u.ImageUploadedDate).ToList(),
                "DateUploaded_desc" => photos.OrderByDescending(u => u.ImageUploadedDate).ToList(),
                "Privacy" => photos.OrderBy(u => u.ImagePrivacy).ToList(),
                "Privacy_desc" => photos.OrderByDescending(u => u.ImagePrivacy).ToList(),
                "Status" => photos.OrderBy(u => u.ImageStatus).ToList(),
                "Status_desc" => photos.OrderByDescending(u => u.ImageStatus).ToList(),
                "Author" => photos.OrderBy(u => u.ApplicationUser.UserName).ToList(),
                "Author_desc" => photos.OrderByDescending(u => u.ApplicationUser.UserName).ToList(),
                "Views24" => photos.OrderBy(u => u.ImageViews.Where(i => i.DateViewed >= currentTime.AddHours(-24)).Count()).ToList(),
                "Views24_desc" => photos.OrderByDescending(u => u.ImageViews.Where(i => i.DateViewed >= currentTime.AddHours(-24)).Count()).ToList(),
                "ViewsWeek" => photos.OrderBy(u => u.ImageViews.Where(i => i.DateViewed >= currentTime.AddHours(-168)).Count()).ToList(),
                "ViewsWeek_desc" => photos.OrderByDescending(u => u.ImageViews.Where(i => i.DateViewed >= currentTime.AddHours(-168)).Count()).ToList(),
                "ViewsMonth" => photos.OrderBy(u => u.ImageViews.Where(i => i.DateViewed >= currentTime.AddHours(-730)).Count()).ToList(),
                "ViewsMonth_desc" => photos.OrderByDescending(u => u.ImageViews.Where(i => i.DateViewed >= currentTime.AddHours(-730)).Count()).ToList(),
                "Views" => photos.OrderBy(u => u.ImageViews.Count()).ToList(),
                "Views_desc" => photos.OrderByDescending(u => u.ImageViews.Where(i => i.DateViewed >= currentTime.AddHours(-24)).Count()).ToList(),
                _ => photos.OrderBy(u => u.ImageUploadedDate).ToList(),
            };

            // Size of the list on one page
            int pageSize = 15;

            // Ternary operator to calculate the page number
            int pageNumber = (int)((!page.HasValue || page == 0) ? 1 : page);

            return View(photos.ToPagedList(pageNumber, pageSize)); // Redirects to the view with the requested list of photos
        }

    }
}
