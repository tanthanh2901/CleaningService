namespace TaskerService.Dtos
{
    public class BookingPremiumDto
    {
        public int PremiumServiceId { get; set; }
        public int BookingId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal AdditionalPrice { get; set; }
        public bool IsPercentage { get; set; }
    }
}
