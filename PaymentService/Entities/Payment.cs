using System.ComponentModel.DataAnnotations.Schema;

namespace PaymentService.Entities
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public int BookingId { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = "cod";
        public string Status { get; set; } = "Pending"; // Pending, Completed, Failed
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? FailureReason { get; set; }
        public string PaymentUrl { get; set; } = string.Empty;
    }
}
