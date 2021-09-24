using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RailGallery.Models
{
    public class Comment
    {
        public int CommentID { get; set; }
        [Required, Display(Name ="Comment Text")]
        [MaxLength(5000)]
        public string CommentText { get; set; }
        [Display(Name = "Comment Date")]
        public DateTime? CommentDate { get; set; }

        [Display(Name = "Comment Author")]
        public ApplicationUser ApplicationUser { get; set; }
        public Image Image { get; set; }
    }
}
