using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MyAuthDemo.Models.Region;

namespace MyAuthDemo.Models
{
    public class Lead
    {
        public int Id { get; set; }

        public int? GroupId { get; set; }
        [ForeignKey("GroupId")]
        public virtual Group? Group { get; set; }

        public string? CompanyName { get; set; }

        [Required]
        public string City { get; set; } = "";

        public string Address { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Email { get; set; } = "";

        [Required]
        public string ContractNumber { get; set; } = "";
        public string ContractStatus { get; set; } = "Active";

        public string PICName { get; set; } = "";
        public string PICPhone { get; set; } = "";
        public string PICEmail { get; set; } = "";

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User? User { get; set; }

        public string? ReferralName { get; set; }
        
        public int? ProvinceId { get; set; }
        public int? RegencyId { get; set; }

        // Optional navigation properties (kalau kamu pakai EF relasi)
        [ForeignKey("ProvinceId")]
        public Province? Province { get; set; }

        [ForeignKey("RegencyId")]
        public Regency? Regency { get; set; }

    }
}
