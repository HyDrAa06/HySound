using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.ComponentModel.DataAnnotations;

namespace HySound.ViewModels
{
    public class EditTrackViewModel
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100, ErrorMessage = "Полето не може да съдържа повече от 100 символа.")]
        public string Title { get; set; }
        
        public bool IsYoutube { get; set; }

        public string? AudioUrl { get; set; }
        public IFormFile? ImageFile { get; set; }
        public string? ImageUrl { get; set; }
        public IFormFile? audioFile { get; set; }

        public SelectList? GenresList { get; set; }
        public int? GenreId { get; set; }
        public SelectList? UserList { get; set; }

        public int? UserId { get; set; }
    }
}
