using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RailGallery.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RailGallery.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Image model
        public DbSet<Image> Images { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(
                    new Category { CategoryID = 1, CategoryTitle = "Night Shots" },
                    new Category { CategoryID = 2, CategoryTitle = "Golden Hour" },
                    new Category { CategoryID = 3, CategoryTitle = "Passanger Trains" }
            );
        }

    }
}
