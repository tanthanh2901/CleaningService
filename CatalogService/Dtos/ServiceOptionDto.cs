namespace CatalogService.Dtos
{
    public class ServiceOptionDto
    {
        public string OptionKey { get; set; }
        public string OptionLabel { get; set; }
        public string DataType { get; set; }
        public string? DefaultValue { get; set; }
    }
}
