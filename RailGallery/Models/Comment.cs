using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RailGallery.Models
{
    public class Comment
    {
        public int CommentID { get; set; }
        [Required, MaxLength(5000), Display(Name = "Comment Text")]
        public string CommentText { get; set; }
        [Display(Name = "Comment Date")]
        public DateTime? CommentDate { get; set; }
        [Display(Name = "Comment Author")]
        public ApplicationUser ApplicationUser { get; set; }
        public Image Image { get; set; }
        [Required, NotMapped]
        public string CommentImageID { get; set; }

        public Comment()
        {
            CommentDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
        }
    }
}
