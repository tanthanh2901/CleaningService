using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CatalogService.Entities
{
    public class PremiumService
    {
        [Key]
        public int PremiumServiceId { get; set; }

        [Required]
        public int ServiceId { get; set; }

        [Required, MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal AdditionalPrice { get; set; }

        public bool IsPercentage { get; set; } = false;

    }
}