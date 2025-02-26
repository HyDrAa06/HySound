using Microsoft.AspNetCore.Mvc.Rendering;
using HySound.Models.Models;
namespace HySound.ViewModels
{
    public class AddAlbumViewModel
    {
        public string Title { get; set; }
        public IFormFile? Picture {  get; set; }
        public DateTime ReleaseDate { get; set; }
        public List<SelectListItem>? Tracks { get; set; }
        public List<int>? SelectedTracksIds { get; set; }    
        public Dictionary<int, string>? TrackPictures { get; set; }
    }
}
