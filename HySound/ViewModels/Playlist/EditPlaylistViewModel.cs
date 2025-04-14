using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HySound.ViewModels.Playlist
{
    public class EditPlaylistViewModel
    {
        public int Id { get; set; }
        [Required]
        [RegularExpression(@"^\S.*$", ErrorMessage = "Полето не трябва да започва с интервал.")]
        [MaxLength(100, ErrorMessage = "Полето не може да съдържа повече от 100 символа.")]
        public string Title { get; set; }
        public string? PictureUrl { get; set; }
        public IFormFile? Picture { get; set; }
        public List<SelectListItem>? Tracks { get; set; }
        public List<int>? SelectedTracksIds { get; set; }
        public Dictionary<int, string>? TrackPictures { get; set; }
    }
}
