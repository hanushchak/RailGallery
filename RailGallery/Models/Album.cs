using RailGallery.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RailGallery.Models
{

    public class Album
    {
        public Album()
        {
            Images = new HashSet<Image>();
        }
        public int AlbumID { get; set; }
        [Required, MaxLength(32), Display(Name = "Album Title")]
        public string AlbumTitle { get; set; }
        [Display(Name = "Album Privacy")]
        [Required]
        public Privacy AlbumPrivacy { get; set; }

        public virtual ICollection<Image> Images { get; set; }

        public ApplicationUser ApplicationUser { get; set; }
    }
}