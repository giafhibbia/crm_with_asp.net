using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyAuthDemo.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyAuthDemo.ViewModels
{
    public class MachineViewModel
    {
        public int Id { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; } = "";
        [Required, StringLength(200)]
        public string FullName { get; set; } = "";
        [StringLength(255)]
        public string? Description { get; set; }
        [Required]
        public int Qty { get; set; }
        public string? AssetCode { get; set; }
        [Required]
        public int CategoryId { get; set; }
        public List<SelectListItem>? CategoryOptions { get; set; }
        [StringLength(255)]
        public string? ImageUrl { get; set; }
        public IFormFile? ImageFile { get; set; }
        public bool RemoveImage { get; set; }
        [Required]
        public MachineCondition Condition { get; set; }
        public List<SelectListItem>? ConditionOptions { get; set; }
        [Required]
        public MachineStatusType StatusType { get; set; }
        public List<SelectListItem>? StatusTypeOptions { get; set; }
        public DateTime? InputAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Property Display untuk List & Detail
        public string? CategoryName { get; set; }
        public string? InputByUserName { get; set; }
        public string? UpdatedByUserName { get; set; }

        public string StatusTypeDisplay =>
        StatusType switch
        {
            MachineStatusType.InUseByCustomer => "Used by Customer",
            MachineStatusType.InUseForOfficeTest => "Used for Office Test",
            MachineStatusType.UnderRepairAtCustomer => "Under Repair at Customer",
            MachineStatusType.UnderRepairAtOffice => "Under Repair at Office",
            MachineStatusType.DamagedUnusable => "Damaged/Unusable",
            _ => StatusType.ToString()
        };

    }
}
