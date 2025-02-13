namespace HySound.ViewModels.Album
{
    public class AlbumFilterViewModel
    {
        public string ArtistName { get; set; }
        public int ArtistId { get; set; }
        public List<AlbumViewModel> Albums { get; set; }
    }
}
