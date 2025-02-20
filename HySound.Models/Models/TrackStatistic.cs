using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace HySound.Models.Models
{
    public class TrackStatistic
    {
        [Key]
        public int Id { get; set; }
        public int? TrackId { get; set; }
        [Required]
        [Range(0, int.MaxValue)]
        public int Plays { get; set; }
        [Range(0, int.MaxValue)]

        public int Likes { get; set; }
        [Range(0, int.MaxValue)]

        public int Comments { get; set; } 
        public Track? Track { get; set; }
    }
}
