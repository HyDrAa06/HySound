﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HySound.Models.Models
{
    public class Track
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]    
        public string Title { get; set; }
        public string? AudioUrl { get; set; } = string.Empty;
        [Required]
        public bool IsYoutube { get; set; }
        public string? CoverImage { get; set; }
        [Range(0, int.MaxValue)]
        
        public int? UserId { get; set; }

        public int? AlbumId { get; set; }

        public int? GenreId { get; set; }

        public Genre? Genre { get; set; }
        public User? User { get; set; }
        public Album? Album { get; set; }
        public ICollection<PlaylistTrack>? PlaylistTracks { get; set; } = new List<PlaylistTrack>();
        public ICollection<Like>? Likes { get; set; } = new List<Like>();
        public ICollection<Comment>? Comments { get; set; } = new List<Comment>();



    }
}
