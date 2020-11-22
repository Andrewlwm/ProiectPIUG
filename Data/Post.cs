using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;

namespace ClonaTwitter.Data
{
    public class Post
    {
        [AutoIncrement]
        public int Id { get; set; }

        [Required]
        public string PostTitle { get; set; }

        [Required]
        public string PostBody { get; set; }

        public string MediaUrl { get; set; }

        public int Views { get; set; } = 0;

        [Required]
        public DateTime CreatedAt { get; set; }

        [Reference]
        public List<Reply> Replies { get; set; }

        [Required]
        [ForeignKey(typeof(User), OnDelete = "CASCADE")]
        public Guid AuthorId { get; set; }

        [Required]
        public string AuthorName { get; set; }
    }
}
