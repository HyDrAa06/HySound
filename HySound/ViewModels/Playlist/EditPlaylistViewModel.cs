using Microsoft.AspNetCore.Mvc.Rendering;

namespace HySound.ViewModels.Playlist
{
    public class EditPlaylistViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? PictureUrl { get; set; }
        public IFormFile? Picture { get; set; }
        public List<SelectListItem>? Tracks { get; set; }
        public List<int>? SelectedTracksIds { get; set; }
        public Dictionary<int, string>? TrackPictures { get; set; }
    }
}
