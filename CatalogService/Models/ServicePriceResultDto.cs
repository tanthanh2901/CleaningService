namespace CatalogService.Models
{
    public class ServicePriceResultDto
    {
        public decimal BaseAmount { get; set; }
        public decimal PremiumAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public List<PriceBreakdownItem> Breakdown { get; set; } = new();
    }
}
