﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HySound.Models.Models
{
    public class Like
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey(nameof(User))]

        public int? UserId { get; set; }
        [ForeignKey(nameof(Track))]

        public int? TrackId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User? User { get; set; }
        public Track? Track { get; set; }
    }
}
