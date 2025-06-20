namespace TaskerService.Dtos
{
    public class BookingDurationDto
    {
        public int DurationConfigId { get; set; }
        public int BookingId { get; set; }
        public int DurationHours { get; set; }
        public int? MaxAreaSqm { get; set; }
        public int? MaxRooms { get; set; }
        public decimal PriceMultiplier { get; set; }
    }
}
