using System.ComponentModel.DataAnnotations;

namespace ClonaTwitter.Models
{
    public class UserRegister
    {
        [Required(ErrorMessage = "Username field required.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password field can't be empty!")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password needs to be at least 6 characters long!")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Password field can't be empty!")]
        [Display(Name = "Repeat password.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords need to match!")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password needs to be at least 6 characters long!")]
        public string ConfirmPassword { get; set; }
    }
}
