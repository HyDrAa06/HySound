﻿using HySound.Models.Models;
using System.ComponentModel.DataAnnotations;

namespace HySound.ViewModels.User
{
    public class UserViewModel
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        [RegularExpression(@"^\S.*$", ErrorMessage = "Полето не трябва да започва с интервал.")]

        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [MaxLength(150)]
        public string? Bio { get; set; }
        public string? ProfilePicture {  get; set; }

        public bool? IsFollowed { get; set; }
        public int? FollowingCount { get; set; }
        public int? FollowersCount { get; set; }
        public IFormFile? ImageFile {  get; set; }
        public ICollection<Followed>? Followers { get; set; }
        public ICollection<Followed>? Following { get; set; }

        public ICollection<HySound.Models.Models.User>? FollowersAsUsers { get; set; }
        public ICollection<HySound.Models.Models.User>? FollowingAsUsers { get; set; }
    }
}
