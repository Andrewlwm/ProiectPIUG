using System.ComponentModel.DataAnnotations;

namespace ClonaTwitter.Models
{
    public class UserLogin
    {
        [Required(ErrorMessage = "Username field required.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password field can't be empty!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
