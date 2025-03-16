using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HySound.Models.Models
{
    public class Album
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }
        public string? CoverImage { get; set; }

        public int? UserId { get; set; }
        public User? User {  get; set; }
        public ICollection<Like>? Likes { get; set; } = new List<Like>();

        public ICollection<Track>? Tracks { get; set; } = new List<Track>();
    }
}
