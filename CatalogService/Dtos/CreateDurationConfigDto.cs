namespace CatalogService.Dtos
{
    public class CreateDurationConfigDto
    {
        public int DurationHours { get; set; }
        public int? MaxAreaSqm { get; set; }
        public int? MaxRooms { get; set; }
        public decimal PriceMultiplier { get; set; } = 1.0m;
    }
}