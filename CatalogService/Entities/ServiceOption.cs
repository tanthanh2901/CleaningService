using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CatalogService.Entities
{
    public class ServiceOption
    {
        //public int Id { get; set; }
        //public int ServiceId { get; set; }
        //public string OptionKey { get; set; }         // VD: "SoLuongTre", "DienTich", "CoBenhLy"
        //public string OptionLabel { get; set; }       // Nhãn hiển thị: "Số lượng trẻ", "Diện tích"
        //public string DataType { get; set; }          // VD: "int", "string", "bool", "decimal"
        //public string? DefaultValue { get; set; }

        //public Service Service { get; set; }

        [Key]
        public int ServiceOptionId { get; set; }

        [Required, MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string OptionType { get; set; } = string.Empty; // house_with_pets, manual_choose_tasker, prioritize_favorite

        [Column(TypeName = "decimal(10,2)")]
        public decimal PriceAdjustment { get; set; } = 0;

        public bool IsPercentage { get; set; } = false;
    }

}