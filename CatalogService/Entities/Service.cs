using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CatalogService.Entities
{
    public class Service
    {
        [Key]
        public int ServiceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string TaskDetails { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal BasePrice { get; set; }
        public string PriceUnit { get; set; } = string.Empty; // per_hour, per_room, per_sqm
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        //public ICollection<ServiceOption> Options { get; set; } = new List<ServiceOption>();
        public ICollection<DurationConfig> DurationConfigs { get; set; } = new List<DurationConfig>();
        public ICollection<PremiumService> PremiumServices { get; set; } = new List<PremiumService>();
    }
}
