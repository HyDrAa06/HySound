namespace HySound.ViewModels.Playlist
{
    public class PlaylistViewModel
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? UserName { get; set; }
        public string? CoverImage { get; set; }
        public IFormFile? Picture { get; set; }
        public List<HySound.Models.Models.Track> Tracks { get; set; } = new List<Models.Models.Track>();


    }
}
