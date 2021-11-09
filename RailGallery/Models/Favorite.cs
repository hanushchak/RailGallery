namespace RailGallery.Models
{
    /// <summary>
    /// Model that defines the Favorite object to map the favorited image with the user who favorited it.
    /// Allows Many-to-Many relationship.
    /// 
    /// Properties:
    /// 
    ///     - FavoriteID - the ID of the Favorite instance
    ///     - ApplicationUser - the user who favorited the image
    ///     - Image - the image that has been favorited
    /// 
    /// Author: Maksym Hanushchak
    /// </summary>
    public class Favorite
    {
        public int FavoriteID { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public Image Image { get; set; }
    }
}
