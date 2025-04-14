using System.ComponentModel.DataAnnotations;

namespace HySound.ViewModels.Account
{
    public class LoginViewModel
    {
        [Required]

        [EmailAddress]
        [RegularExpression(@"^\S.*$", ErrorMessage = "Полето не трябва да започва с интервал.")]

        public string Email { get; set; }


        [Required]
        [MinLength(8, ErrorMessage = "Полето трябва да съдържа поне 8 символа.")]
        [MaxLength(50, ErrorMessage = "Полето не може да съдържа повече от 50 символа.")]
        [RegularExpression(@"^\S.*$", ErrorMessage = "Полето не трябва да започва с интервал.")]
        [DataType(DataType.Password)]

        public string Password { get; set; }



        [Display(Name = "Remember me?")]

        public bool RememberMe { get; set; }
    }
}
