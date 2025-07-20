using System.ComponentModel.DataAnnotations;

namespace MyAuthDemo.Models.Leads
{
    public enum LeadStatus
    {
        [Display(Name = "Pending")]
        Pending = 0,

        [Display(Name = "Trial")]
        Trial = 1,

        [Display(Name = "Active")]
        Active = 2,

        [Display(Name = "Inactive")]
        Inactive = 3
    }
}
