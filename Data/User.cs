using ServiceStack.DataAnnotations;
using System;

namespace ClonaTwitter.Data
{
    public class User
    {
        [AutoId]
        public Guid Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }
    }
}