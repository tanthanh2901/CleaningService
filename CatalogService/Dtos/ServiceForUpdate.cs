namespace CatalogService.Dtos
{
    public class ServiceForUpdate
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; }
        public int CategoryId { get; set; }
    }
}
