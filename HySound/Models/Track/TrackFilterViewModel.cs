using HySound.Models.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HySound.Models.Track
{
    public class TrackFilterViewModel
    {
        public SelectList? Genres { get; set; }
        public int? GenreId { get; set; }

        public List<TrackViewModel> Tracks { get; set; }
        public string? Title { get; set; }

    }
}
