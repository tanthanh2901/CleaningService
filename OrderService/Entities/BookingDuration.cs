
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderService.Entities
{
    public class BookingDuration
    {
        [Key]
        public int DurationConfigId { get; set; }
        public int BookingId { get; set; }
        public int DurationHours { get; set; }

        public int? MaxAreaSqm { get; set; }

        public int? MaxRooms { get; set; }
        [Column(TypeName = "decimal(3,2)")]
        public decimal PriceMultiplier { get; set; }
    }
}