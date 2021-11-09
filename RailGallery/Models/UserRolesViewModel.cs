namespace RailGallery.Models
{
    /// <summary>
    /// View Model to represent the select list options to modify the user roles.
    /// Does not represent a table in the database.
    /// 
    /// Author: Maksym Hanushchak
    /// </summary>
    public class UserRolesViewModel
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public bool Selected { get; set; }
    }
}
