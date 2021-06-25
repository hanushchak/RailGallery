using System;
using System.Collections.Generic;

namespace RailGallery.Models
{
    public enum Status
    {
        Pending, Published, Rejected
    }

    public enum Privacy
    {
        Public, Private
    }

    public class Image
    {
        public int ImageID { get; set; }
        public string ImageTitle { get; set; }
        public string ImageDescription { get; set; }
        public string ImageMetadata { get; set; }
        public DateTime ImageTakenDate { get; set; }
        public DateTime ImageUploadedDate { get; set; }
        public Status ImageStatus { get; set; }
        public Privacy ImagePrivacy { get; set; }

        public ICollection<Comment> Comments { get; set; }
        public ICollection<Album> Albums { get; set; }
        public ICollection<Like> Likes { get; set; }
        public ICollection<Favorite> Favorites { get; set; }
    }
}