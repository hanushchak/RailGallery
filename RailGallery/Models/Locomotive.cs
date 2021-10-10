using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RailGallery.Models
{
    public class Locomotive
    {
        public int LocomotiveID { get; set; }
        [Required, Display(Name = "Locomotive Model")]
        public string LocomotiveModel { get; set; }
        [Required, Display(Name = "Locomotive Built"), DataType(DataType.Date), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/yyyy}")]
        public DateTime LocomotiveBuilt { get; set; }
        public ICollection<Image> Images { get; set; }
    }
}
