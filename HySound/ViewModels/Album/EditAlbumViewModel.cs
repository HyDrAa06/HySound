using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HySound.ViewModels.Album
{
    public class EditAlbumViewModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "Полето не може да съдържа повече от 100 символа.")]
        public string Title { get; set; }
        public SelectList? UserList { get; set; }
        public int? UserId { get; set; }
        public string? AlbumCover { get; set; }
        public IFormFile? ImageFile {  get; set; }
    }
}
