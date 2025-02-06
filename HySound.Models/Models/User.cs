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
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public string UserIdentityId { get; set; }
        public IdentityUser UserIdentity { get; set; }
        public string? ProfilePicture { get; set; }
        public string? Bio {  get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Track>? Tracks { get; set; } = new List<Track>();
        public ICollection<Playlist>? Playlists { get; set; } = new List<Playlist>();
        public ICollection<Album>? Albums { get; set; } = new List<Album>();
        public ICollection<Comment>? Comments { get; set; } = new List<Comment>();
        public ICollection<Like>? Likes { get; set; } = new List<Like>();

        public ICollection<Follower>? Followers { get; set; }
        public ICollection<Follower>? Following { get; set; }

    }
}
