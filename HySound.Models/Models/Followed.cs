using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HySound.Models.Models
{
    public class Followed
    {
        public int FollowedById { get; set; }
        public int FollowedId { get; set; }
        public User? FollowedByUser { get; set; }
        public User? FollowedUser { get; set; }


    }
}
