using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RailGallery.Models
{
    public class Locomotive
    {
        public int LocomotiveID { get; set; }
        [Required, Display(Name = "Locomotive Model")]
        public string LocomotiveModel { get; set; }
        [Required, Display(Name = "Locomotive Built"), DataType(DataType.DateTime), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:Y}")]
        public DateTime LocomotiveBuilt { get; set; }
        public ICollection<Image> Images { get; set; }
    }
}
