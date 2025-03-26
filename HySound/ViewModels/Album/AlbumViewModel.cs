using System.ComponentModel.DataAnnotations;

namespace HySound.ViewModels.Album
{
    public class AlbumViewModel
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100, ErrorMessage = "Полето не може да съдържа повече от 100 символа.")]
        public string Title { get; set; }
        [Required]

        public string UserName { get; set; }
        public string CoverImage { get; set; }
        public bool? IsLiked { get; set; }


        public List<HySound.Models.Models.Track> Tracks { get; set; } = new List<Models.Models.Track>();

    }
}
