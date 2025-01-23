﻿using System;
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
        public string Title { get; set; }
        public string? CoverImage { get; set; }
        public DateTime ReleaseDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(User))]
        public int? UserId { get; set; }
        public User? User {  get; set; }
        public ICollection<Track>? Tracks { get; set; } = new List<Track>();
    }
}
