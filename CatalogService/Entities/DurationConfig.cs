using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CatalogService.Entities
{
    public class DurationConfig
    {
        [Key]
        public int DurationConfigId { get; set; }
        [Required]
        public int ServiceId { get; set; }

        [Required]
        public int DurationHours { get; set; }

        public int? MaxAreaSqm { get; set; }

        public int? MaxRooms { get; set; }

        [Column(TypeName = "decimal(3,2)")]
        public decimal PriceMultiplier { get; set; } = 1.0m;

    }
}