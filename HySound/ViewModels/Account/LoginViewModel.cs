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
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?!\s)(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\w\d\s:]).{8,}$",
    ErrorMessage = "Паролата трябва да има малка и главна буква, цифра, символ и да не започва с интервал.")]

        public string Password { get; set; }



        [Display(Name = "Remember me?")]

        public bool RememberMe { get; set; }
    }
}
