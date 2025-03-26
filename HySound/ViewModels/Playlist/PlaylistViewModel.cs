using System.ComponentModel.DataAnnotations;

namespace HySound.ViewModels.Playlist
{
    public class PlaylistViewModel
    {
        public int Id { get; set; }
        [MaxLength(100, ErrorMessage = "Полето не може да съдържа повече от 100 символа.")]

        public string? Title { get; set; }
        public string? UserName { get; set; }
        public string? CoverImage { get; set; }
        public IFormFile? Picture { get; set; }
        public List<HySound.Models.Models.Track> Tracks { get; set; } = new List<Models.Models.Track>();
        public string? Description { get; set; }

        public bool? IsLiked {  get; set; }

    }
}
