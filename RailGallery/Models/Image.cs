﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RailGallery.Models
{
    public enum Status
    {
        Pending, 
        Published, 
        Rejected
    }

    public enum Privacy
    {
        Public, 
        Private
    }

    public class Image
    {
        [Display(Name = "ID")]
        public int ImageID { get; set; }
        [Required]
        [Display(Name = "Title")]
        public string ImageTitle { get; set; }
        [Display(Name = "Description")]
        public string ImageDescription { get; set; }
        [Display(Name = "EXIF")]
        public string ImageMetadata { get; set; }
        [Display(Name = "Date Taken")]
        public DateTime ImageTakenDate { get; set; }
        [Display(Name = "Date Uploaded")]
        public DateTime ImageUploadedDate { get; set; }
        [Display(Name = "Status")]
        public Status ImageStatus { get; set; }
        [Display(Name = "Privacy")]
        public Privacy ImagePrivacy { get; set; }

        public ICollection<Comment> Comments { get; set; }
        public ICollection<Album> Albums { get; set; }
        public ICollection<Like> Likes { get; set; }
        public ICollection<Favorite> Favorites { get; set; }

        public ApplicationUser ApplicationUser { get; set; }
        public Category Category { get; set; }
    }
}