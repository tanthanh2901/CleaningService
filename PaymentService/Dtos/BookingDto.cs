namespace PaymentService.Dtos
{
    public class BookingDto
    {
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public string PaymentMethod { get; set; }
    }

}
