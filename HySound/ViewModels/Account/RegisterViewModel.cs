using Newtonsoft.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace HySound.ViewModels.Account
{
    public class RegisterViewModel
    {
        [Required]

        [EmailAddress]
        [RegularExpression(@"^\S.*$", ErrorMessage = "Полето не трябва да започва с интервал.")]

        public string Email { get; set; }
        [Required]
        [RegularExpression(@"^\S.*$", ErrorMessage = "Полето не трябва да започва с интервал.")]
        [MaxLength(50, ErrorMessage = "Полето не може да съдържа повече от 50 символа.")]
        public string Username { get; set; }

        [Required]
        [RegularExpression(@"^\S.*$", ErrorMessage = "Полето не трябва да започва с интервал.")]
        [DataType(DataType.Password)]

        public string Password { get; set; }


        [Required]
        [MinLength(8, ErrorMessage = "Полето трябва да съдържа поне 8 символа.")]
        [MaxLength(50, ErrorMessage = "Полето не може да съдържа повече от 50 символа.")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^\S.*$", ErrorMessage = "Полето не трябва да започва с интервал.")]
        [Compare("Password", ErrorMessage = "The passwords do not match.")]

        public string ConfirmPassword { get; set; }
    }
}
