using System.ComponentModel.DataAnnotations;

namespace HySound.ViewModels.Comment
{
    public class AddCommentViewModel
    {
        [Required]
        [MaxLength(500, ErrorMessage = "Полето не може да съдържа повече от 500 символа.")]
        public string Content { get; set; }
        public int TrackId { get; set; }
    }
}
