namespace PaymentService.Entities
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public int BookingId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public string Status { get; set; } = "Pending"; // Pending, Completed, Failed
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? FailureReason { get; set; }
        public string PaymentUrl { get; set; } = string.Empty;
    }
}
