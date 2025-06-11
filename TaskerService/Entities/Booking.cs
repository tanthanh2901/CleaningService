using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskerService.Entities
{
    public class Booking
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int BookingId { get; set; }
        public int TaskerId { get; set; }
        public DateTime ScheduleTime { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string ServiceName { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; }
        public BookingStatus BookingStatus { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public Tasker Tasker { get; set; }
    }
} 