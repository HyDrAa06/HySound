using Microsoft.AspNetCore.Mvc.Rendering;

namespace HySound.Models
{
    public class AddAlbumViewModel
    {
        public string Title { get; set; }
        public DateTime ReleaseDate { get; set; }

        public SelectList? UserList { get; set; }
        public int? UserId { get; set; }
    }
}
