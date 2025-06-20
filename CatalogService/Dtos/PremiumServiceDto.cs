namespace CatalogService.Dtos
{
    public class PremiumServiceDto
    {
        public int PremiumServiceId { get; set; }
        public string Name { get; set; }
        public decimal AdditionalPrice { get; set; }
        public bool IsPercentage { get; set; } = false;

    }
}
