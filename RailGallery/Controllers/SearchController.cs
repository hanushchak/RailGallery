using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RailGallery.Data;
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

        public SearchController(ILogger<SearchController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }


        // GET: SearchController
        public ActionResult Index()
        {
            return View();
        }

        // GET: Results
        public IActionResult Results(int? page,
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
            var model = _context.Images.OrderByDescending(i => i.ImageUploadedDate)
                .Include(c => c.Comments)
                .Include(c => c.ApplicationUser)
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
/*            if (!String.IsNullOrEmpty(LocomotiveModel))
            {
                model = model.Where(i => i.LocomotiveModel.Contains(LocomotiveModel));
                ViewBag.LocomotiveModel = LocomotiveModel;
            }*/
/*            if (!String.IsNullOrEmpty(LocomotiveBuilt))
            {
                model = model.Where(i => i.LocomotiveBuilt.Contains(LocomotiveBuilt));
                ViewBag.LocomotiveBuilt = LocomotiveBuilt;
            }*/
/*            if (!String.IsNullOrEmpty(ImageAlbum))
            {
                model = model.Where(i => i.Albums.Contains(ImageAlbum));
                ViewBag.ImageAlbum = ImageAlbum;
            }*/
/*            if (!String.IsNullOrEmpty(ImageCategory))
            {
                model = model.Where(i => i.Category.Contains(ImageCategory));
                ViewBag.ImageCategory = ImageCategory;
            }*/
            if (!String.IsNullOrEmpty(ImageTakenDate))
            {
                model = model.Where(i => i.ImageTakenDate.ToString().Contains(ImageTakenDate));
                ViewBag.ImageTakenDate = ImageTakenDate;
            }
/*            if (!String.IsNullOrEmpty(ImageLocation))
            {
                model = model.Where(i => i.Location.Contains(ImageLocation));
                ViewBag.ImageLocation = ImageLocation;
            }*/


            int pageSize = 3; // TODO

            int pageNumber = (int)((!page.HasValue || page == 0) ? 1 : page);

            return View(model.ToPagedList(pageNumber, pageSize));
        }
    }
}
