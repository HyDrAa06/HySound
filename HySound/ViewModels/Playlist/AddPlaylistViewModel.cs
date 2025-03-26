﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HySound.ViewModels.Playlist
{
    public class AddPlaylistViewModel
    {
        [Required]
        [MaxLength(100, ErrorMessage = "Полето не може да съдържа повече от 100 символа.")]

        public string Title { get; set; }
        public IFormFile? Picture { get; set; }
        public List<SelectListItem>? Tracks { get; set; }
        public List<int>? SelectedTracksIds { get; set; }
        public Dictionary<int, string>? TrackPictures { get; set; }
    }
}
