using System.ComponentModel.DataAnnotations;

namespace MyAuthDemo.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        public string PasswordHash { get; set; } = "";

        public string? Name { get; set; }
    }
}
