namespace CatalogService.Entities
{
    public class ServiceOption
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public string OptionKey { get; set; }         // VD: "SoLuongTre", "DienTich", "CoBenhLy"
        public string OptionLabel { get; set; }       // Nhãn hiển thị: "Số lượng trẻ", "Diện tích"
        public string DataType { get; set; }          // VD: "int", "string", "bool", "decimal"
        public string? DefaultValue { get; set; }

        public Service Service { get; set; }
    }

}