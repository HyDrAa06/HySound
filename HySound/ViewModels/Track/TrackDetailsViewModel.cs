
namespace HySound.ViewModels.Track
{
    public class TrackDetailsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Username { get; set; }
        public string TrackImage { get; set; }
        public int Plays { get; set; }
        public int LikesCount { get; set; }
        public string Genre { get; set; }

        public string AudioUrl { get; set; }
        public bool IsLiked { get; set; }
        public List<HySound.Models.Models.Comment> Comments { get; set; }
    }
}
