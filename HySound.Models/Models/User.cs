using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HySound.Models.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Username { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [MinLength(8)]
        [MaxLength(50)]
        public string Password { get; set; }
        public string? ProfilePicture { get; set; }
        [MaxLength(500)]
        public string? Bio {  get; set; }

        public IdentityUser UserIdentity { get; set; }
        public string UserIdentityId { get; set; }
        public ICollection<Track>? Tracks { get; set; }
        public ICollection<Playlist>? Playlists { get; set; }
        public ICollection<Album>? Albums { get; set; }
        public ICollection<Comment>? Comments { get; set; }
        public ICollection<Like>? Likes { get; set; } 

        public ICollection<Followed>? FollowedBy { get; set; }
        public ICollection<Followed>? Following { get; set; }
        public ICollection<ArtistRequest>? ArtistRequests { get; set; }

    }
}
