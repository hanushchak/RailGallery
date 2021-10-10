namespace RailGallery.Models
{
    public class Like
    {
        public int LikeID { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public Image Image { get; set; }
    }
}
