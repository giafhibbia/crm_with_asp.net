using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyAuthDemo.Models
{
    public class Position
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = "";

        public ICollection<User>? Users { get; set; }
    }
}
