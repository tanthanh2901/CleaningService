namespace CatalogService.Dtos
{
    public class ServiceDtoForCreate
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal BasePrice { get; set; }
        public string Unit { get; set; }
        public string ImageUrl { get; set; }
        public int CategoryId { get; set; }
        public List<ServiceOptionDto> Options { get; set; }
    }
}
