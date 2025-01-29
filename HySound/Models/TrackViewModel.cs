using Microsoft.AspNetCore.Mvc.Rendering;

namespace HySound.Models
{
    public class TrackViewModel
    {
        public int TrackId { get; set; }
        public string Title { get; set; }
        public string AudioUrl { get; set; }
        public int Duration { get; set; }
        public int Plays { get; set; }
        public string GenreName { get; set; }
        public string UserName { get; set; } 

    }
}
