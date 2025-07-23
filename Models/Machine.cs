using System;
using System.ComponentModel.DataAnnotations;

namespace MyAuthDemo.Models
{
    public enum MachineStatusType
    {
        StockReady,
        InUseByCustomer,
        InUseForOfficeTest,
        UnderRepairAtCustomer,
        UnderRepairAtOffice,
        DamagedUnusable
    }

    public enum MachineCondition
    {
        New,
        Used
    }

    public class Machine
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public ProductCategory? Category { get; set; }
        public string Name { get; set; } = "";
        public string FullName { get; set; } = "";
        public string? Description { get; set; }
        public int Qty { get; set; }
        public string AssetCode { get; set; } = "";
        public string? ImageUrl { get; set; }
        public MachineCondition Condition { get; set; }
        public MachineStatusType StatusType { get; set; }
        public int? InputByUserId { get; set; }
        public User? InputByUser { get; set; }
        public int? UpdatedByUserId { get; set; }
        public User? UpdatedByUser { get; set; }

        public DateTime InputAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
    }
}
