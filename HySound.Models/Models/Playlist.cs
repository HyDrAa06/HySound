using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HySound.Models.Models
{
    public class Playlist
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        [RegularExpression(@"\S.*", ErrorMessage = "Заглавието не може да е само празни интервали.")]

        public string Title { get; set; }
        public string? CoverImage { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }
        public int? UserId { get; set; }
        public User? User { get; set; }
        public ICollection<Like>? Likes { get; set; } = new List<Like>();

        public ICollection<PlaylistTrack>? PlaylistTracks { get; set; } = new List<PlaylistTrack>();
    }
}
