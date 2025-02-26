namespace HySound.ViewModels.Album
{
    public class AlbumFilterViewModel
    {
        public string Search { get; set; }
        public int ArtistId { get; set; }

        public List<AlbumViewModel> Albums { get; set; }
    }
}
