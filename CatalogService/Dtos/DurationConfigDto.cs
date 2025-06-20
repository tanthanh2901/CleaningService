namespace CatalogService.Dtos
{
    public class DurationConfigDto
    {
        public int DurationConfigId { get; set; }
        public int DurationHours { get; set; }
        public int MaxAreaSqm { get; set; }
        public int MaxRooms { get; set; }
        public decimal PriceMultiplier { get; set; }
    }
}
