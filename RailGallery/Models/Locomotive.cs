using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RailGallery.Models
{
    /// <summary>
    /// Model that defines a Locomotive. 
    /// 
    /// Properties:
    ///     - LocomotiveID - the ID of the locomotive instance
    ///     - LocomotiveModel - the model of the locomotive
    ///     - LocomotiveBuilt - the date when the locomotive was built
    ///     - Images - a collection of images that represent this locomotive
    ///     
    /// Author: Maksym Hanushchak
    /// </summary>
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
