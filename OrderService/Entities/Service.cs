using System.ComponentModel.DataAnnotations.Schema;

namespace OrderService.Entities
{
    public class Service
    {
        public int ServiceId { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public decimal BasePrice { get; set; }

        public ICollection<ServiceOption> Options { get; set; }
        public ICollection<DurationConfig> DurationConfigs { get; set; }
        public ICollection<PremiumService> PremiumServices { get; set; }
    }
}
