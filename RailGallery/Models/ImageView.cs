using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RailGallery.Models
{
    public class ImageView
    {
        public int ImageViewId { get; set; }

        public DateTime DateViewed { get; set; }

        public int ImageId { get; set; }
        public Image Image { get; set; }
    }
}
