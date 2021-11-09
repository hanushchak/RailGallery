using Microsoft.AspNetCore.Http;
using RailGallery.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RailGallery.Models
{
    /// <summary>
    /// Model that defines an Image.  
    /// Properties:
    /// 
    ///     - ImageID - the ID of the image instance
    ///     - ImageTitle - the title of the photo
    ///     - ImageDescription - the description of the photo
    ///     - ImageTakenDate - the date when the photo was taken
    ///     - ImageUploadedDate - the date when the photo was uploaded
    ///     - ImageStatus - the status of the photo
    ///     - ImagePrivacy - the privacy option of the photo
    ///     - ImagePath - the path to the file in the file system
    ///     - ApplicationUser - the author of the photo
    ///     
    ///     - ImageFile - (Not mapped to a table field) - The file of the photo
    ///     
    ///     - Comments - a collection of comments that belong to this photo
    ///     - Albums - a collection of albums this photo belongs to
    ///     - Likes - a collection of likes that belong to this photo
    ///     - Favorites - a collection of favorites that belong to this photo
    ///     - ImageViews - a collection of views that belong to this photo
    ///     
    ///     - Category - the category this photo belongs to
    ///     - Location - the location of this photo
    ///     - Locomotive - the locomotive in this photo
    /// 
    /// Author: Maksym Hanushchak
    /// </summary>
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

        /// <summary>
        /// Constructor. Initially sets the uploaded date to the date of creation of Image object and sets the status to Pending.
        /// </summary>
        public Image()
        {
            ImageUploadedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
            ImageStatus = Status.Pending;

            Albums = new HashSet<Album>();
        }
    }
}