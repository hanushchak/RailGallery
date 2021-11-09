using System;

namespace RailGallery.Models
{
    /// <summary>
    /// Model that defines the Image View object to keep track of the each image's statistics.
    /// Allows Many-to-Many relationship.
    /// 
    /// Properties:
    /// 
    ///     - ImageViewId - the ID of the Image View instance
    ///     - DateViewed - the date when the photo was viewed
    ///     - ImageId - the ID of the viewed image
    ///     - Image - the reference to the Image instance
    /// 
    /// Author: Maksym Hanushchak
    /// </summary>
    public class ImageView
    {
        public int ImageViewId { get; set; }

        public DateTime DateViewed { get; set; }

        public int ImageId { get; set; }
        public Image Image { get; set; }
    }
}
