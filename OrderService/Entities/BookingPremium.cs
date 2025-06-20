using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderService.Entities
{
    public class BookingPremium
    {
        [Key]
        public int PremiumServiceId { get; set; }
        public int BookingId { get; set; }
        public string Name { get; set; } = string.Empty;

        [Column(TypeName = "decimal(10,2)")]
        public decimal AdditionalPrice { get; set; }
        public bool IsPercentage { get; set; }
    }
}