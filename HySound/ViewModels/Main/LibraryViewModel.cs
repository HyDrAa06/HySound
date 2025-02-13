using HySound.Models.Models;
using HySound.ViewModels.Album;

namespace HySound.ViewModels.Main
{
    public class LibraryViewModel
    {
        public List<TrackViewModel> Tracks { get; set; }
        public List<AlbumViewModel> Albums { get; set; }
        public List<Playlist> Playlists { get; set; }
    }
}
