﻿using Microsoft.AspNetCore.Mvc;
using RailGallery.Data;
using RailGallery.Models;
using System.Threading.Tasks;

namespace RailGallery.Controllers
{
    public class LocomotiveController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LocomotiveController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Locomotive/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Locomotive/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
