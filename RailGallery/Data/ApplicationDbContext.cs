using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RailGallery.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RailGallery.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
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
        public DbSet<Locomotive> Locomotives { get; set; }
        public DbSet<Location> Locations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(
                    new Category { CategoryID = 1, CategoryTitle = "Night Shots" },
                    new Category { CategoryID = 2, CategoryTitle = "Golden Hour" },
                    new Category { CategoryID = 3, CategoryTitle = "Accidents" },
                    new Category { CategoryID = 4, CategoryTitle = "In-Cab Photos" },
                    new Category { CategoryID = 5, CategoryTitle = "Passenger Trains" },
                    new Category { CategoryID = 6, CategoryTitle = "Freight Trains" },
                    new Category { CategoryID = 7, CategoryTitle = "Steam Locomotives" },
                    new Category { CategoryID = 8, CategoryTitle = "Bridges" }
            );

            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable(name: "Users");
            });
            modelBuilder.Entity<IdentityRole>(entity =>
            {
                entity.ToTable(name: "Roles");
            });
            modelBuilder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("UserRoles");
            });
            modelBuilder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("UserClaims");
            });
            modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("UserLogins");
            });
            modelBuilder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.ToTable("RoleClaims");
            });
            modelBuilder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("UserTokens");
            });
        }

    }
}
