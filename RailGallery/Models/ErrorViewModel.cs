namespace RailGallery.Models
{
    /// <summary>
    /// View Model to display errors.
    /// 
    /// Author: Maksym Hanushchak
    /// </summary>
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
