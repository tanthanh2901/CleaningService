namespace CatalogService.Dtos
{
    public class ServiceOptionDtoForUpdate
    {
        public int? Id { get; set; }
        public string OptionKey { get; set; }
        public string OptionLabel { get; set; }
        public string DataType { get; set; }
        public string? DefaultValue { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
