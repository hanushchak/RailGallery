using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RailGallery.Models;
using System;

namespace RailGallery.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Image> Images { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Locomotive> Locomotives { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<ImageView> ImageViews { get; set; }

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

            modelBuilder.Entity<Locomotive>().HasData(
                    new Locomotive { LocomotiveID = 1, LocomotiveModel = "Steam 4-6-0", LocomotiveBuilt = new DateTime(1964, 8, 14) },
                    new Locomotive { LocomotiveID = 2, LocomotiveModel = "GE ES44AC", LocomotiveBuilt = new DateTime(1997, 3, 9) },
                    new Locomotive { LocomotiveID = 3, LocomotiveModel = "GE AC4400CWM", LocomotiveBuilt = new DateTime(2000, 2, 29) },
                    new Locomotive { LocomotiveID = 4, LocomotiveModel = "R160", LocomotiveBuilt = new DateTime(1956, 5, 15) },
                    new Locomotive { LocomotiveID = 5, LocomotiveModel = "GE P42DC", LocomotiveBuilt = new DateTime(1994, 1, 3) },
                    new Locomotive { LocomotiveID = 6, LocomotiveModel = "GC 546", LocomotiveBuilt = new DateTime(2007, 8, 26) },
                    new Locomotive { LocomotiveID = 7, LocomotiveModel = "Steam 2-8-4", LocomotiveBuilt = new DateTime(1956, 1, 5) },
                    new Locomotive { LocomotiveID = 8, LocomotiveModel = "AMT 1325", LocomotiveBuilt = new DateTime(2009, 6, 20) }
            );

            modelBuilder.Entity<Location>().HasData(
                    new Location { LocationID = 1, LocationName = "Ontario, Canada" },
                    new Location { LocationID = 2, LocationName = "New York, USA" },
                    new Location { LocationID = 3, LocationName = "New Jersey, USA" },
                    new Location { LocationID = 4, LocationName = "British Columbia, Canada" },
                    new Location { LocationID = 5, LocationName = "Nova Scotia, Canada" },
                    new Location { LocationID = 6, LocationName = "Virginia, USA" },
                    new Location { LocationID = 7, LocationName = "Manitoba, Canada" },
                    new Location { LocationID = 8, LocationName = "Alberta, Canada" }
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
