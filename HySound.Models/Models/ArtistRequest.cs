using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HySound.Models.Models
{
    public class ArtistRequest
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string ArtistUsername { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [MinLength(8)]
        [MaxLength(50)]
        public string Password { get; set; }
        public string? ProfilePicture { get; set; }
        [MaxLength(150)]
        public string? Bio { get; set; }

        public string Status { get; set; } = "pending";
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        public int? AdminId { get; set; }
        public User? Admin { get; set; } 
        public User User { get; set; }
        public int? UserId { get; set; }

        public string? IdentityUserId { get; set; }
        public IdentityUser UserIdentity { get; set; }

    }
}
