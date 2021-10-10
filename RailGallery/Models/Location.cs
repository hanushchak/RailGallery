using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RailGallery.Models
{
    public class Location
    {
        public int LocationID { get; set; }
        [Required, Display(Name = "Location")]
        public string LocationName { get; set; }
        public ICollection<Image> Images { get; set; }
    }
}
