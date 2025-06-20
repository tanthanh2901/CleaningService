using CatalogService.Entities;

namespace CatalogService.Dtos
{
    public class ServiceDtoForUpdate
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string TaskDetails { get; set; }
        public string ImageUrl { get; set; }
        public decimal BasePrice { get; set; }
        public string PriceUnit { get; set; }
        public int CategoryId { get; set; }
        public List<DurationConfigDto> DurationConfigs { get; set; }
        public List<PremiumServiceDto> PremiumServices { get; set; }
    }
}
