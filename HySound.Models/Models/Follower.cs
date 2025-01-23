using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HySound.Models.Models
{
    public class Follower
    {
        [Key]
        public int Id { get; set; }
        public int? FollowedId { get; set; }

        public int? FollowingId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public User? FollowedUser { get; set; }
        public User? FollowingUser { get; set; }

    }
}
