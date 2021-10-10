using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RailGallery.Models
{
    public class Category
    {
        public int CategoryID { get; set; }
        [Required, Display(Name = "Category")]
        public string CategoryTitle { get; set; }
        public ICollection<Image> Images { get; set; }
    }
}
