namespace RailGallery.Models
{
    public class Favorite
    {
        public int FavoriteID { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public Image Image { get; set; }
    }
}
