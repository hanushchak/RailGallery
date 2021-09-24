﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
