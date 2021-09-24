﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RailGallery.Data;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace RailGallery.Controllers
{
    [Authorize(Roles = "Moderator")]
    public class AdminController : Controller
    {

        private readonly ILogger<AdminController> _logger;
        private readonly ApplicationDbContext _context;

        public AdminController(ILogger<AdminController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            dynamic adminModel = new ExpandoObject();

            // Most recent images
            adminModel.PendingImages = _context.Images
                .Take(15)
                .Where(i => i.ImageStatus == Enums.Status.Pending && i.ImagePrivacy != Enums.Privacy.Private)
                .OrderBy(i => i.ImageUploadedDate)
                .Include(c => c.ApplicationUser)
                .AsNoTracking();

            adminModel.RecentComments = _context.Comments
                .Take(15)
                .OrderByDescending(c => c.CommentDate)
                .Include(c => c.ApplicationUser)
                .Include(c => c.Image)
                .AsNoTracking();

            return View(adminModel);
        }
    }
}
