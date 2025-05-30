namespace MessageBus.IntegrationEvents
{
    public class BookingCreatedEvent
    {
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public int ServiceId { get; set; }
        public string Address { get; set; }
        public decimal TotalAmount { get; set; }
        public PaymentMethodType PaymentMethod { get; set; }

        public int TaskerId { get; set; }
        public DateTime ScheduleTime { get; set; }
    }
}
