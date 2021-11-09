using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RailGallery.Models
{
    /// <summary>
    /// Model that defines a Category.  
    /// Properties:
    /// 
    ///     - CategoryID - the ID of the category object
    ///     - CategoryTitle - the title of the category
    ///     - Images - a collection of photos that belong to the category
    /// 
    /// Author: Maksym Hanushchak
    /// </summary>
    public class Category
    {
        public int CategoryID { get; set; }
        [Required, Display(Name = "Category")]
        public string CategoryTitle { get; set; }
        public ICollection<Image> Images { get; set; }
    }
}
