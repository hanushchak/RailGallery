﻿using System;
using System.Collections.Generic;

namespace RailGallery.Models
{

    public class Album
    {
        public int AlbumID { get; set; }
        public string AlbumTitle { get; set; }
        public Privacy AlbumPrivacy { get; set; }

        public ICollection<Image> Images { get; set; }

        public ApplicationUser ApplicationUser { get; set; }
    }
}