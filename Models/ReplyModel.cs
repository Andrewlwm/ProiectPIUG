using System.ComponentModel.DataAnnotations;

namespace ClonaTwitter.Models
{
    public class ReplyModel
    {
        [Required(ErrorMessage = "Your comment needs a body!")]
        public string ReplyBody { get; set; }

        public int PostId { get; set; }
    }
}
