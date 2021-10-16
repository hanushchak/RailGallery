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
    [Authorize(Roles = "Moderator")]
    public class AdminController : Controller
    {

        private readonly ILogger<AdminController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(ILogger<AdminController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            dynamic adminModel = new ExpandoObject();

            // Most recent images
            adminModel.PendingImages = _context.Images
                .Where(i => i.ImageStatus == Enums.Status.Pending && i.ImagePrivacy != Enums.Privacy.Private)
                .OrderBy(i => i.ImageUploadedDate)
                .Take(15)
                .Include(c => c.ApplicationUser)
                .AsNoTracking();

            adminModel.RecentComments = _context.Comments
                .OrderByDescending(c => c.CommentDate)
                .Take(15)
                .Include(c => c.ApplicationUser)
                .Include(c => c.Image)
                .AsNoTracking();

            return View(adminModel);
        }

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

            if (collection["decision"] == "Reject")
            {
                image.ImageStatus = Enums.Status.Rejected;
                _context.Update(image);
                await _context.SaveChangesAsync();

                return RedirectToAction("View", "View", new { @id = image.ImageID });
            }

            if (collection["decision"] == "Approve")
            {
                image.ImageStatus = Enums.Status.Published;
                _context.Update(image);
                await _context.SaveChangesAsync();

                return RedirectToAction("View", "View", new { @id = image.ImageID });
            }

            return Content("Error");
        }

        public async Task<IActionResult> Users(int? page, string username, string sortOrder)
        {
            ViewBag.SearchString = username;
            ViewBag.SortOrder = sortOrder;

            ViewBag.UsernameSort = String.IsNullOrEmpty(sortOrder) ? "username_desc" : "";
            ViewBag.EmailSort = sortOrder == "Email" ? "email_desc" : "Email";
            ViewBag.RegisteredSort = sortOrder == "Registered" ? "registered_desc" : "Registered";
            ViewBag.LastActiveSort = sortOrder == "LastActive" ? "lastactive_desc" : "LastActive";
            ViewBag.NumberPhotosSort = sortOrder == "NumberPhotos" ? "numberphotos_desc" : "NumberPhotos";
            ViewBag.NumberCommentsSort = sortOrder == "NumberComments" ? "numbercomments_desc" : "NumberComments";

            var users = await _userManager.Users.Include(u => u.Images).Include(u => u.Comments).ToListAsync();

            if (!String.IsNullOrEmpty(username))
            {
                users = users.Where(u => u.UserName.Contains(username) || u.Email.Contains(username)).ToList();
            }

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

            int pageSize = 2;

            int pageNumber = (int)((!page.HasValue || page == 0) ? 1 : page);

            return View(users.ToPagedList(pageNumber, pageSize));
        }

        public async Task<IActionResult> Roles(string username)
        {
            if (username == null)
            {
                return NotFound();
            }

            ViewBag.username = username;

            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return NotFound();
            }

            var model = new List<UserRolesViewModel>();

            var roles = await _roleManager.Roles.ToListAsync();

            foreach (var role in roles)
            {
                var userRolesViewModel = new UserRolesViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };
                if (await _userManager.IsInRoleAsync(user, role.Name))
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
            var roles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, roles);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing roles");
                return View(model);
            }
            result = await _userManager.AddToRolesAsync(user, model.Where(x => x.Selected).Select(y => y.RoleName));
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected roles to user");
                return View(model);
            }
            return RedirectToAction("Users");
        }

        public async Task<IActionResult> Access(string username)
        {
            if (username == null)
            {
                return NotFound();
            }

            ViewBag.username = username;

            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return NotFound();
            }

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
            if (access == "disabled")
            {
                await _userManager.SetLockoutEnabledAsync(user, true);
                await _userManager.SetLockoutEndDateAsync(user, DateTime.UtcNow.AddYears(1));
                await _userManager.UpdateAsync(user);
            }
            else
            {
                await _userManager.SetLockoutEnabledAsync(user, false);
                await _userManager.SetLockoutEndDateAsync(user, null);
                await _userManager.UpdateAsync(user);
            }

            return RedirectToAction("Users");
        }

        public async Task<IActionResult> Photos(int? page, string sortOrder)
        {
            ViewBag.SortOrder = sortOrder;

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

            var photos = await _context.Images.Include(u => u.ApplicationUser).Include(u => u.Comments).Include(u => u.ImageViews).ToListAsync();

            DateTime currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));

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

            int pageSize = 15;

            int pageNumber = (int)((!page.HasValue || page == 0) ? 1 : page);

            return View(photos.ToPagedList(pageNumber, pageSize));
        }

    }
}
