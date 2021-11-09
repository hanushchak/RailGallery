using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RailGallery.Models
{
    /// <summary>
    /// Model that defines a Location. 
    /// 
    /// Properties:
    ///     - LocationID - the ID of the location instance
    ///     - LocationName - the name of the location
    ///     - Images - a collection of images that belong to this location
    ///     
    /// Author: Maksym Hanushchak
    /// </summary>
    public class Location
    {
        public int LocationID { get; set; }
        [Required, Display(Name = "Location")]
        public string LocationName { get; set; }
        public ICollection<Image> Images { get; set; }
    }
}
