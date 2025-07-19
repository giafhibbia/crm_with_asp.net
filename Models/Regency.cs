namespace MyAuthDemo.Models.Region
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("reg_regencies")]
    public class Regency
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("province_id")]
        public int ProvinceId { get; set; }

        [Required]
        [Column("name")]
        [MaxLength(255)]
        public string Name { get; set; }

        // Relasi navigasi ke Province
        [ForeignKey("ProvinceId")]
        public Province Province { get; set; }
    }
}
