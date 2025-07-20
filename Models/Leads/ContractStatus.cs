using System.ComponentModel.DataAnnotations;

namespace MyAuthDemo.Models.Leads
{
    public enum ContractStatus
    {
        [Display(Name = "Inactive")]
        Inactive = 0,

        [Display(Name = "Active")]
        Active = 1,

        [Display(Name = "Expired")]
        Expired = 2,

        [Display(Name = "Terminated")]
        Terminated = 3
    }
}
