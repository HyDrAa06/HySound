using Microsoft.AspNetCore.Mvc.Rendering;

namespace HySound.Models
{
    public class EditTrackViewModel
    {
        public string Title { get; set; }
        public string? AudioUrl { get; set; }
        public int Plays { get; set; }
        public string? ImageLink { get; set; }

        public SelectList? GenresList { get; set; }
        public int? GenreId { get; set; }
        public SelectList? UserList { get; set; }

        public int? UserId { get; set; }
    }
}
