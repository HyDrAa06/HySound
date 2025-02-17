using HySound.Models.Models;
using HySound.ViewModels.Album;
using HySound.ViewModels.Playlist;

namespace HySound.ViewModels.Main
{
    public class LibraryViewModel
    {
        public List<TrackViewModel> Tracks { get; set; }
        public List<AlbumViewModel> Albums { get; set; }
        public List<PlaylistViewModel> Playlists { get; set; }
    }
}
