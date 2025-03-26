using HySound.Models.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HySound.ViewModels.Track
{
    public class TrackFilterViewModel
    {
        public SelectList? Genres { get; set; }
        public int? GenreId { get; set; }

        public List<TrackViewModel> Tracks { get; set; }
        [Required]
        [MaxLength(100, ErrorMessage = "Полето не може да съдържа повече от 100 символа.")]
        public string? Title { get; set; }
        public List<HySound.Models.Models.Playlist> Playlists { get; set; }
    }
}
