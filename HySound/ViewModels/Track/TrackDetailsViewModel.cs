
namespace HySound.ViewModels.Track
{
    public class TrackDetailsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Username { get; set; }
        public string TrackImage { get; set; }
        public int Plays { get; set; }
        public string Genre { get; set; }
        public List<HySound.Models.Models.Comment> Comments { get; set; }
    }
}
