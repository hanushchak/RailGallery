using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RailGallery.Data;
using RailGallery.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

#nullable enable

namespace RailGallery.Controllers
{
    public class SearchController : Controller
    {
        private readonly ILogger<SearchController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SearchController(ILogger<SearchController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }


        // GET: SearchController
        public ActionResult Index()
        {
            return View();
        }

        // GET: Results
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

            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            IList<string> currentUserRoles = null;
            if (currentUser != null)
            {
                currentUserRoles = await _userManager.GetRolesAsync(currentUser);
            }

            bool userLoggedIn = currentUser != null;
            bool userIsModerator = userLoggedIn && currentUserRoles.Contains(Enums.Roles.Moderator.ToString());

                var model = _context.Images
                .Where(i => !((i.ImageStatus == Enums.Status.Pending) || (i.ImageStatus == Enums.Status.Rejected) || (i.ImagePrivacy == Enums.Privacy.Private && !(userLoggedIn && i.ApplicationUser.UserName.Equals(currentUser.UserName)))))
                .OrderByDescending(i => i.ImageUploadedDate)
                .Include(c => c.Comments)
                .Include(c => c.Likes)
                .Include(c => c.ApplicationUser)
                .Include(c => c.Category)
                .Include(c => c.Location)
                .Include(c => c.Locomotive)
                .AsNoTracking();

            if (!String.IsNullOrEmpty(ImageTitle))
            {
                model = model.Where(i => i.ImageTitle.Contains(ImageTitle));
                ViewBag.ImageTitle = ImageTitle;
            }
            if (!String.IsNullOrEmpty(ImageAuthor))
            {
                model = model.Where(i => i.ApplicationUser.UserName.Contains(ImageAuthor));
                ViewBag.ImageAuthor = ImageAuthor;
            }
            if (!String.IsNullOrEmpty(ImageDescription))
            {
                model = model.Where(i => i.ImageDescription.Contains(ImageDescription));
                ViewBag.ImageDescription = ImageDescription;
            }
            if (!String.IsNullOrEmpty(LocomotiveModel))
            {
                model = model.Where(i => i.Locomotive.LocomotiveModel.Contains(LocomotiveModel));
                ViewBag.LocomotiveModel = LocomotiveModel;
            }
            if (!String.IsNullOrEmpty(LocomotiveBuilt))
            {
                model = model.Where(i => i.Locomotive.LocomotiveBuilt.ToString().Contains(LocomotiveBuilt));
                ViewBag.LocomotiveBuilt = LocomotiveBuilt;
            }
            /*if (!String.IsNullOrEmpty(ImageAlbum))
            {
                model = model.Where(i => i.Albums.Contains(ImageAlbum));
                ViewBag.ImageAlbum = ImageAlbum;
            }*/
            if (!String.IsNullOrEmpty(ImageCategory))
            {
                model = model.Where(i => i.Category.CategoryTitle.Contains(ImageCategory));
                ViewBag.ImageCategory = ImageCategory;
            }
            if (!String.IsNullOrEmpty(ImageTakenDate))
            {
                model = model.Where(i => i.ImageTakenDate.ToString().Contains(ImageTakenDate));
                ViewBag.ImageTakenDate = ImageTakenDate;
            }
            if (!String.IsNullOrEmpty(ImageLocation))
            {
                model = model.Where(i => i.Location.LocationName.Contains(ImageLocation));
                ViewBag.ImageLocation = ImageLocation;
            }


            int pageSize = 10; // TODO

            int pageNumber = (int)((!page.HasValue || page == 0) ? 1 : page);

            return View(model.ToPagedList(pageNumber, pageSize));
        }
    }
}
