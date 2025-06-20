namespace CatalogService.Dtos
{
    public class ServiceDtoForCreate
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal BasePrice { get; set; }
        public string PriceUnit { get; set; } = string.Empty;
        public string TaskDetails { get; set; }
        public string ImageUrl { get; set; }
        public int CategoryId { get; set; }
        public List<CreateDurationConfigDto> DurationConfigs { get; set; } = new();
        public List<CreatePremiumServiceDto> PremiumServices { get; set; } = new();
    }
}
