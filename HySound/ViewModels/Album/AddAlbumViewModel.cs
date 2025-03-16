using Microsoft.AspNetCore.Mvc.Rendering;
using HySound.Models.Models;
using System.ComponentModel.DataAnnotations;
namespace HySound.ViewModels
{
    public class AddAlbumViewModel
    {
        [Required]
        public string Title { get; set; }
        public IFormFile? Picture {  get; set; }
        public DateTime ReleaseDate { get; set; }
        public List<SelectListItem>? Tracks { get; set; }
        public List<int>? SelectedTracksIds { get; set; }    
        public Dictionary<int, string>? TrackPictures { get; set; }
    }
}
