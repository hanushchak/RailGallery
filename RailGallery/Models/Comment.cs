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
        [Required,Display(Name ="Comment Text")]
        public string CommentText { get; set; }
        [Required, Display(Name = "Comment Date")]
        public DateTime CommentDate { get; set; }

        [Required, Display(Name = "Comment Author")]
        public ApplicationUser ApplicationUser { get; set; }
        [Required]
        public Image Image { get; set; }
    }
}
