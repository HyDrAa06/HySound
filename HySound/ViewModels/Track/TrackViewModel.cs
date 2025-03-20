using HySound.Models.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HySound.ViewModels
{
    public class TrackViewModel
    {
        public int TrackId { get; set; }
        public string Title { get; set; }
        public string? AudioUrl { get; set; }
        public bool IsYoutube { get; set; }
        public string? ImageLink { get; set; }
        public string GenreName { get; set; }
        public string UserName { get; set; } 
        

    }
}
