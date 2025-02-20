using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HySound.Models.Models
{
    public class PlaylistTrack
    {
        [Key]
        public int Id { get; set; }


        public int? PlaylistId { get; set; }

        public int? TrackId { get; set; }

        public Playlist? Playlist { get; set; }
        public Track? Track { get; set; }
    }
}
