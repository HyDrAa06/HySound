using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HySound.ViewModels
{
    public class AddTrackViewModel
    {
        [Required]
        [MaxLength(100)]
        [RegularExpression(@"^\S.*$", ErrorMessage = "Полето не трябва да започва с интервал.")]
        public string Title { get; set; }
        public bool IsYoutube { get; set; }
        [RegularExpression(@"^\S.*$", ErrorMessage = "Полето не трябва да започва с интервал.")]

        public string? AudioUrl { get; set; }
        public IFormFile? audioFile { get; set; }
        public IFormFile? imageFile { get; set; }
        public string? ImageLink { get; set; }

        public SelectList? GenresList { get; set; }
        public int? GenreId { get; set; }
  
    }
}
