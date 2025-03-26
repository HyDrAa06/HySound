using HySound.Models.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HySound.ViewModels
{
    public class TrackViewModel
    {
        public int TrackId { get; set; }
        [Required]
        [MaxLength(100, ErrorMessage = "Полето не може да съдържа повече от 100 символа.")]
        public string Title { get; set; }
        public string? AudioUrl { get; set; }
        public bool IsYoutube { get; set; }
        public string? ImageLink { get; set; }
        public string GenreName { get; set; }
        public string UserName { get; set; }
        public int LikesCount { get; set; }


    }
}
