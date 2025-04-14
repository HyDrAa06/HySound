using Microsoft.AspNetCore.Mvc.Rendering;
using HySound.Models.Models;
using System.ComponentModel.DataAnnotations;
namespace HySound.ViewModels
{
    public class AddAlbumViewModel
    {
        [Required]
        [MaxLength(100, ErrorMessage = "Полето не може да съдържа повече от 100 символа.")]
        [RegularExpression(@"^\S.*$", ErrorMessage = "Полето не трябва да започва с интервал.")]

        public string Title { get; set; }
        public IFormFile? Picture {  get; set; }
        public List<SelectListItem>? Tracks { get; set; }
        public List<int>? SelectedTracksIds { get; set; }    
        public Dictionary<int, string>? TrackPictures { get; set; }
    }
}
