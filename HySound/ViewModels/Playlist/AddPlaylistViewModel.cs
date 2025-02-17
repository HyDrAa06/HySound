using Microsoft.AspNetCore.Mvc.Rendering;

namespace HySound.ViewModels.Playlist
{
    public class AddPlaylistViewModel
    {
        public string Title { get; set; }
        public string Picture { get; set; }
        public DateTime ReleaseDate { get; set; }
        public SelectList? UserList { get; set; }
        public int? UserId { get; set; }
        public List<SelectListItem>? Tracks { get; set; }
        public List<int>? SelectedTracksIds { get; set; }
        public Dictionary<int, string>? TrackPictures { get; set; }
    }
}
