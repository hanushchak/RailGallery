﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RailGallery.Models
{
    public class Like
    {
        public int LikeID { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public Image Image { get; set; }
    }
}
