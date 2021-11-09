using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RailGallery.Models
{
    /// <summary>
    /// Model that defines a User. This model extends the standard Identity User and adds the custom fields defined below. 
    /// 
    /// Properties:
    /// 
    ///     - LastActivityDate - the last time the user logged in
    ///     - RegisterationDate - the registartion date of the user

    ///     - Images - a collection of photos that belong to the user
    ///     - Albums - a collection of albums that belong to the user 
    ///     - Comments - a collection of comments that belong to the user
    ///     - Likes - a collection of like objects that belong to the user
    ///     - Favorites - a collection of favorite objects that belong to the user
    /// 
    /// Author: Maksym Hanushchak
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        public ICollection<Image> Images { get; set; }
        public ICollection<Album> Albums { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Like> Likes { get; set; }
        public ICollection<Favorite> Favorites { get; set; }

        [Display(Name = "Last Seen")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        [Required]
        public DateTime LastActivityDate { get; set; }

        [Display(Name = "Registration Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        [Required]
        public DateTime RegisterationDate { get; set; }

        /// <summary>
        /// Constructor. Initially sets the registration date and the last activity date to the date and time when the user object is created.
        /// </summary>
        public ApplicationUser()
        {
            RegisterationDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
            LastActivityDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
        }
    }
}
