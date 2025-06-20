namespace CatalogService.Models
{
    public class PriceBreakdownItem
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // base, premium, addon, option
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}