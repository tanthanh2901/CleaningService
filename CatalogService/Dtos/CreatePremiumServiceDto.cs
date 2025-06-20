namespace CatalogService.Dtos
{
    public class CreatePremiumServiceDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal AdditionalPrice { get; set; }
        public bool IsPercentage { get; set; } = false;
    }
}