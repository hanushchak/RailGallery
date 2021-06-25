using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RailGallery.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<Image> Images { get; set; }
        public ICollection<Album> Albums { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Like> Likes { get; set; }
        public ICollection<Favorite> Favorites { get; set; }
    }
}
