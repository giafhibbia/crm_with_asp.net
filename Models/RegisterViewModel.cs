using System.ComponentModel.DataAnnotations;

namespace MyAuthDemo.Models
{
    public class RegisterViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = "";

        [Required, MinLength(6)]
        public string Password { get; set; } = "";

        public string? Name { get; set; }

        public int? PositionId { get; set; }

        public int? RoleId { get; set; }

        public IFormFile? Avatar { get; set; }   // Untuk upload avatar
    }

}
