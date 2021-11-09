using RailGallery.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RailGallery.Models
{
    /// <summary>
    /// Model that defines an Album. 
    /// 
    /// Properties:
    ///     - AlbumID - the ID of the album instance
    ///     - AlbumTitle - the title of the album
    ///     - AlbumPrivacy - the album's privacy option
    ///     - Images - a collection of photos that belong to the album
    ///     - ApplicationUser - the album's owner
    /// 
    /// Author: Maksym Hanushchak
    /// </summary>
    public class Album
    {
        /// <summary>
        /// Constructor.
        /// </summary>
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