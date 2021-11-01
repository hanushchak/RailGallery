using Microsoft.AspNetCore.Mvc;
using RailGallery.Data;
using RailGallery.Models;
using System.Threading.Tasks;

namespace RailGallery.Controllers
{
    /// <summary>
    /// <c>Location Controller</c>
    /// Contains a method that allows to add new locaton to the database.
    /// 
    /// Author: Maksym Hanushchak
    /// </summary>
    public class LocationController : Controller
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Class constructor.
        /// </summary>
        /// <param name="context">Reference to the current context.</param>
        public LocationController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// HTTP GET method that displays the Create Location view.
        /// 
        /// GET: [host]/Location/Create
        /// </summary>
        /// <returns>Redirects to the view.</returns>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// HTTP POST method that is called upon the Create Location form submission. 
        /// Creates a new location object and saves it to the database.
        /// 
        /// POST: [host]/Location/Create{FormCollection}
        /// </summary>
        /// <param name="location">The new location object submitted by the form.</param>
        /// <returns>Redirects to the view.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LocationID,LocationName")] Location location)
        {
            // If the form submission is valid...
            if (ModelState.IsValid)
            {
                // Add the new location object to the database
                _context.Add(location);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Upload");
            }
            return View(location);
        }
    }
}
