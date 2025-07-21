// Models/UserViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace MyAuthDemo.Models
{
    public class UserViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; } = string.Empty; // For create/edit, not for displaying hash

        [Display(Name = "Role")]
        [Required(ErrorMessage = "Role is required.")]
        public int RoleId { get; set; }

        [Display(Name = "Position")]
        public int? PositionId { get; set; } // Make it nullable if position is optional
                                              // Or [Required] if it's always mandatory
    }
}