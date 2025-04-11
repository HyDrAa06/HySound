using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HySound.ViewModels
{
    public class AddTrackViewModel
    {
        [Required]
        [MaxLength(100)]
        [RegularExpression(@"\S.*", ErrorMessage = "Заглавието не може да е само празни интервали.")]
        public string Title { get; set; }
        public bool IsYoutube { get; set; }
        public string? AudioUrl { get; set; }
        public IFormFile? audioFile { get; set; }
        public IFormFile? imageFile { get; set; }
        public string? ImageLink { get; set; }

        public SelectList? GenresList { get; set; }
        public int? GenreId { get; set; }
  
    }
}
