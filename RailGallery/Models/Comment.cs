using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RailGallery.Models
{
    /// <summary>
    /// Model that defines a Comment.  
    /// Properties:
    /// 
    ///     - CommentID - the ID of the comment instance
    ///     - CommentText - the text body of the comment
    ///     - CommentDate - the date when the comment was created
    ///     - ApplicationUser - the author of the comment
    ///     - Image - the image to which the comment belongs
    ///     
    ///     - CommentImageID - (not mapped to a table field). The ID of the photo to which the comment belongs
    /// 
    /// Author: Maksym Hanushchak
    /// </summary>
    public class Comment
    {
        public int CommentID { get; set; }
        [Required, MaxLength(5000), Display(Name = "Comment Text")]
        public string CommentText { get; set; }
        [Display(Name = "Comment Date"), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd hh:mm tt}")]
        public DateTime? CommentDate { get; set; }
        [Display(Name = "Comment Author")]
        public ApplicationUser ApplicationUser { get; set; }
        public Image Image { get; set; }
        [Required, NotMapped]
        public string CommentImageID { get; set; }

        /// <summary>
        /// Constructor. Sets the Comment date to the date when the comment object was created.
        /// </summary>
        public Comment()
        {
            CommentDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
        }
    }
}
