namespace CatalogService.Models
{
    public class ServicePriceCalculationDto
    {
        public int ServiceId { get; set; }
        public int DurationConfigId { get; set; }
        public List<int> PremiumServiceIds { get; set; } = new();
    }
}
