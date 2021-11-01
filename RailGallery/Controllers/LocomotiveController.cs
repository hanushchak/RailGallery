using Microsoft.AspNetCore.Mvc;
using RailGallery.Data;
using RailGallery.Models;
using System.Threading.Tasks;

namespace RailGallery.Controllers
{
    /// <summary>
    /// <c>Locomotive Controller</c>
    /// Contains a method that allows to add new locomotive to the database.
    /// 
    /// Author: Maksym Hanushchak 
    /// </summary>
    public class LocomotiveController : Controller
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Class constructor.
        /// </summary>
        /// <param name="context">Reference to the current context.</param>
        public LocomotiveController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// HTTP GET method that displays the Create Locomotive view.
        /// 
        /// GET: [host]/Locomotive/Create
        /// </summary>
        /// <returns>Redirects to the view.</returns>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// HTTP POST method that is called upon the Create Locomotive form submission. 
        /// Creates a new location object and saves it to the database.
        /// 
        /// POST: [host]/Locomotive/Create{FormCollection}
        /// </summary>
        /// <param name="location">The new locomotive object submitted by the form.</param>
        /// <returns>Redirects to the view.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LocomotiveID,LocomotiveModel,LocomotiveBuilt")] Locomotive locomotive)
        {
            if (ModelState.IsValid)
            {
                _context.Add(locomotive);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Upload");
            }
            return View(locomotive);
        }
    }
}
