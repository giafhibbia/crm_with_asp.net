using System.ComponentModel.DataAnnotations;

namespace MyAuthDemo.Models
{
    public class ProductCategory
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(250)]
        public string? Description { get; set; }

        // Store file path or URL
        public string? IconUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        // Tambahkan property relasi ke Machine
        public ICollection<Machine>? Machines { get; set; }
    }
}
