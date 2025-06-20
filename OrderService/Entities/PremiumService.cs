namespace OrderService.Entities
{
    public class PremiumService
    {
        public int PremiumServiceId { get; set; }
        public int ServiceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal AdditionalPrice { get; set; }
        public bool IsPercentage { get; set; } = false;
    }
}