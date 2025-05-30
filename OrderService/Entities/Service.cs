using System.ComponentModel.DataAnnotations.Schema;

namespace OrderService.Entities
{
    public class Service
    {
        public int ServiceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string Unit { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal BasePrice { get; set; }
        public int CategoryId { get; set; }

        public ICollection<ServiceOption> Options { get; set; }
    }
}
