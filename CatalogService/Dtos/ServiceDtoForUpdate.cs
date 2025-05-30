namespace CatalogService.Dtos
{
    public class ServiceDtoForUpdate
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public decimal BasePrice { get; set; }
        public int Duration { get; set; }
        public int CategoryId { get; set; }
        public List<ServiceOptionDto> Options { get; set; }
    }
}
