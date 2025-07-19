using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyAuthDemo.Models
{
    public class Group
    {
        public int Id { get; set; }

        [Required, StringLength(150)]
        public string Name { get; set; } = "";

        [ForeignKey("UserCreate")]
        public int UserIdCreate { get; set; }
        public User? UserCreate { get; set; }

        [ForeignKey("UserUpdate")]
        public int? UserIdUpdate { get; set; }
        public User? UserUpdate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<Lead> Leads { get; set; } = new List<Lead>();
    }
}
