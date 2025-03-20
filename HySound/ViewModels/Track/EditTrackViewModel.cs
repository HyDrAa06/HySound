using Microsoft.AspNetCore.Mvc.Rendering;

namespace HySound.ViewModels
{
    public class EditTrackViewModel
    {
        public string Title { get; set; }
        public bool IsYoutube { get; set; }

        public string? AudioUrl { get; set; }
        public IFormFile? ImageFile { get; set; }
        public string? ImageUrl { get; set; }
        public IFormFile? audioFile { get; set; }

        public SelectList? GenresList { get; set; }
        public int? GenreId { get; set; }
        public SelectList? UserList { get; set; }

        public int? UserId { get; set; }
    }
}
