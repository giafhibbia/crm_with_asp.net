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
        public string? AvatarUrl { get; set; }

        // Foreign key dan navigation property untuk Position
        public int? PositionId { get; set; }
        public Position? Position { get; set; }

        // Foreign key dan navigation property untuk Role
        public int? RoleId { get; set; }
        public Role? Role { get; set; }
        public ICollection<Lead>? Leads { get; set; }
    }
}
