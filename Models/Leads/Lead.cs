using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Tambahkan ini!
using MyAuthDemo.Models.Region;

namespace MyAuthDemo.Models.Leads
{
    public class Lead
    {
        public int Id { get; set; }

        public int? GroupId { get; set; }
        public virtual Group? Group { get; set; }

        [Required]
        public string CompanyName { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Email { get; set; } = "";

        [Required]
        public string PICName { get; set; } = "";
        public string PICPhone { get; set; } = "";
        public string PICEmail { get; set; } = "";

        public string? ContractNumber { get; set; }
        public ContractStatus ContractStatus { get; set; } = ContractStatus.Inactive;
        public LeadStatus Status { get; set; } = 0;

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User? User { get; set; }

        public string? ReferralName { get; set; }

        [Required(ErrorMessage = "Province is required")]
        public int? ProvinceId { get; set; }

        [Required(ErrorMessage = "Regency is required")]
        public int? RegencyId { get; set; }

        public Province? Province { get; set; }
        public Regency? Regency { get; set; }
    }
}
