using ServiceStack.DataAnnotations;
using System;

namespace ClonaTwitter.Data
{
    public class Reply
    {

        [AutoIncrement]
        public int Id { get; set; }


        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Your comment needs a body!")]
        [Required]
        public string ReplyBody { get; set; }

        [Required]
        [ForeignKey(typeof(Post), OnDelete = "CASCADE")]
        public int PostId { get; set; }

        [Required]
        [ForeignKey(typeof(User), OnDelete = "CASCADE")]
        public Guid AuthorId { get; set; }

        [Required]
        public string AuthorName { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
