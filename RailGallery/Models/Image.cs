using Microsoft.AspNetCore.Http;
using RailGallery.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RailGallery.Models
{
    public class Image
    {
        [Display(Name = "ID")]
        public int ImageID { get; set; }
        [Display(Name = "Title"), MaxLength(32), Required]
        public string ImageTitle { get; set; }
        [Display(Name = "Description"), MaxLength(120), Required]
        public string ImageDescription { get; set; }
        [Display(Name = "Date Taken"), DataType(DataType.Date), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}"), Required]
        public DateTime ImageTakenDate { get; set; }
        [Display(Name = "Date Uploaded"), DataType(DataType.Date), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}"), Required]
        public DateTime ImageUploadedDate { get; set; }
        [Display(Name = "Status"), Required]
        public Status ImageStatus { get; set; }
        [Display(Name = "Privacy"), Required]
        public Privacy ImagePrivacy { get; set; }
        public string ImagePath { get; set; }

        [Display(Name = "Image File"), NotMapped, Required]
        public IFormFile ImageFile { get; set; }

        public ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Album> Albums { get; set; }
        public ICollection<Like> Likes { get; set; }
        public ICollection<Favorite> Favorites { get; set; }
        public ICollection<ImageView> ImageViews { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        [Display(Name = "Category")]
        public Category Category { get; set; }
        [Display(Name = "Category"), Required, NotMapped]
        public string ImageCategoryID { get; set; }
        [Display(Name = "Location")]
        public Location Location { get; set; }
        [Display(Name = "Location"), Required, NotMapped]
        public string ImageLocationID { get; set; }
        [Display(Name = "Locomotive")]
        public Locomotive Locomotive { get; set; }
        [Display(Name = "Locomotive"), Required, NotMapped]
        public string ImageLocomotiveID { get; set; }

        public Image()
        {
            ImageUploadedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
            ImageStatus = Status.Pending;

            Albums = new HashSet<Album>();
        }
    }
}