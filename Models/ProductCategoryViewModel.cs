using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MyAuthDemo.ViewModels
{
    public class ProductCategoryViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(250)]
        public string? Description { get; set; }

        public IFormFile? IconFile { get; set; } // Upload icon/image
        public string? IconUrl { get; set; }     // For edit
        public bool RemoveIcon { get; set; } 
    }
}
