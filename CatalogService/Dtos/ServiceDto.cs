using CatalogService.Entities;

namespace CatalogService.Dtos
{
    public class ServiceDto
    {
        public int ServiceId { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string? Description { get; set; }
        public string? TaskDetails { get; set; }
        public decimal BasePrice { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        //public ICollection<ServiceOptionDto> Options { get; set; }
        public ICollection<DurationConfig> DurationConfigs { get; set; }
        public ICollection<PremiumService> PremiumServices { get; set; }

    }
}
