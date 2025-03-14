﻿using System.ComponentModel.DataAnnotations;

namespace HySound.ViewModels.Account
{
    public class RegisterViewModel
    {
        [Required]

        [EmailAddress]

        public string Email { get; set; }

        public string Username { get; set; }

        [Required]

        [DataType(DataType.Password)]

        public string Password { get; set; }



        [Required]

        [DataType(DataType.Password)]

        [Compare("Password", ErrorMessage = "The passwords do not match.")]

        public string ConfirmPassword { get; set; }
    }
}
