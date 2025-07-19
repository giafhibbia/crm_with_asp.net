using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyAuthDemo.Models
{
    public class Permission
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = "";

        public ICollection<RolePermission>? RolePermissions { get; set; }
    }
}
