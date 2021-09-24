using RailGallery.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RailGallery.Models
{

    public class Album
    {
        public int AlbumID { get; set; }
        [Required, MaxLength(32), Display(Name = "Album Title")]
        public string AlbumTitle { get; set; }
        [Display(Name = "Album Privacy")]
        [Required]
        public Privacy AlbumPrivacy { get; set; }

        public ICollection<Image> Images { get; set; }

        public ApplicationUser ApplicationUser { get; set; }
    }
}