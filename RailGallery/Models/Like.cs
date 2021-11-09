namespace RailGallery.Models
{
    /// <summary>
    /// Model that defines the Like object to map the liked image with the user who liked it.
    /// Allows Many-to-Many relationship.
    /// 
    /// Properties:
    /// 
    ///     - LikeID - the ID of the Like instance
    ///     - ApplicationUser - the user who liked the image
    ///     - Image - the image that has been liked
    /// 
    /// Author: Maksym Hanushchak
    /// </summary>
    public class Like
    {
        public int LikeID { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public Image Image { get; set; }
    }
}
