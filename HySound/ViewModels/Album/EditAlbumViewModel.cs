﻿using Microsoft.AspNetCore.Mvc.Rendering;

namespace HySound.ViewModels.Album
{
    public class EditAlbumViewModel
    {
        public string Title { get; set; }
        public DateTime ReleaseDate { get; set; }

        public SelectList? UserList { get; set; }
        public int? UserId { get; set; }
        public string? AlbumCover { get; set; }
        public IFormFile? ImageFile {  get; set; }
    }
}
