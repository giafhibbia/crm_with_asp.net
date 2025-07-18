using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyAuthDemo.Models
{
    public class Role
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; } = "";

        public string? Level { get; set; }

        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
