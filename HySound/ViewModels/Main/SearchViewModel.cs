using HySound.ViewModels.Album;
using HySound.ViewModels.Playlist;
using HySound.ViewModels.User;

namespace HySound.ViewModels.Main
{
    public class SearchViewModel
    {
        public string? SearchFilter { get; set; }
        public string? filter { get; set; }

        public List<TrackViewModel>? Tracks { get; set; }
        public List<AlbumViewModel>? Albums { get; set; }
        public List<PlaylistViewModel>? Playlists { get; set; }
        public List<UserViewModel>? User { get; set; }
    }
}
