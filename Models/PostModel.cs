using System;
using System.ComponentModel.DataAnnotations;

namespace ClonaTwitter.Models
{
    public class PostModel
    {
        [Required(ErrorMessage = "Your post needs a title.")]
        [Display(Name = "Post title")]
        public string PostTitle { get; set; }

        [Required(ErrorMessage = "Your post needs a body.")]
        [Display(Name = "Post body")]
        public string PostBody { get; set; }

        [Display(Name = "Media URL")]
        public string MediaUrl { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public Guid AuthorId { get; set; }
    }
}
