namespace OrderService.Entities
{
    public class DurationConfig
    {
        public int DurationConfigId { get; set; }
        public int ServiceId { get; set; }
        public int DurationHours { get; set; }
        public int? MaxAreaSqm { get; set; }
        public int? MaxRooms { get; set; }
        public decimal PriceMultiplier { get; set; } = 1.0m;
    }
}